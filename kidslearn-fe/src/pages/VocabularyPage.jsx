import { useEffect, useState, useMemo } from "react";
import { Link, useParams } from "react-router-dom";
import { vocabApi, unitApi } from "../api/content";
import { activityApi } from "../api/activity"; // ✅ THÊM
import { useTTS } from "../hooks/useTTS";
import LoadingScreen from "../components/LoadingScreen";
import EmptyState from "../components/EmptyState";
import Icon from "../components/Icon";

export default function VocabularyPage() {
  const { unitId } = useParams();
  const [unit, setUnit] = useState(null);
  const [vocabs, setVocabs] = useState([]);
  const [idx, setIdx] = useState(0);
  const [flipped, setFlipped] = useState(false);
  const [loading, setLoading] = useState(true);
  const { speak, supported } = useTTS();

  // ✅ Load unit, vocab, và vị trí học cũ
  useEffect(() => {
    (async () => {
      try {
        const [u, vs] = await Promise.all([
          unitApi.getById(unitId),
          vocabApi.getByUnit(unitId),
        ]);
        setUnit(u);
        setVocabs(vs || []);

        // ✅ Lấy vị trí học gần nhất (nếu có)
        try {
          const activity = await activityApi.getByUnit(unitId);
          if (activity && activity.lastVocabIndex >= 0 && activity.lastVocabIndex < (vs?.length || 0)) {
            setIdx(activity.lastVocabIndex);
          }
        } catch {
          // Lần đầu vào Unit → không có activity, bỏ qua
        }
      } finally {
        setLoading(false);
      }
    })();
  }, [unitId]);

  useEffect(() => setFlipped(false), [idx]);

  // ✅ Lưu vị trí mới mỗi khi đổi từ (không block UI)
  useEffect(() => {
    if (!loading && vocabs.length > 0) {
      activityApi.updateMyActivity(Number(unitId), idx).catch(() => {
        // Lỗi mạng — bỏ qua, không ảnh hưởng học
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [idx]);

  const current = vocabs[idx];
  const progress = useMemo(() => (vocabs.length ? Math.round(((idx + 1) / vocabs.length) * 100) : 0), [idx, vocabs.length]);

  if (loading) return <LoadingScreen />;
  if (!vocabs.length) {
    return (
      <div>
        <EmptyState
          title={unit?.title ? `${unit.title} chưa có từ vựng` : "Chưa có từ vựng"}
          hint="Hãy quay lại sau hoặc thử Unit khác!"
          action={<Link to="/grades" className="btn-primary"><Icon name="menu_book" size={18} /> Về danh sách lớp</Link>}
        />
      </div>
    );
  }

  return (
    <div className="min-h-[80vh] flex flex-col items-center relative">
      {/* Decorative bg icons */}
      <div className="fixed top-32 left-10 text-primary-container opacity-10 -z-10 pointer-events-none hidden md:block">
        <Icon name="school" size={120} filled />
      </div>
      <div className="fixed bottom-20 right-10 text-secondary-container opacity-10 -z-10 pointer-events-none hidden md:block">
        <Icon name="toys" size={150} filled />
      </div>

      {/* Header with exit + quiz */}
      <div className="w-full max-w-2xl flex items-center justify-between mb-6">
        <Link
          to={unit ? `/grades/${unit.gradeId}` : "/grades"}
          className="w-12 h-12 bg-surface-container-lowest border-2 border-outline-variant rounded-full flex items-center justify-center text-on-surface-variant hover:bg-error-container hover:text-on-error-container hover:border-error-container transition-colors border-b-4 border-b-outline"
          aria-label="Thoát bài học"
        >
          <Icon name="close" size={22} />
        </Link>
      </div>

      {/* Theme & progress */}
      <div className="w-full max-w-2xl text-center mb-8">
        <h1 className="font-display text-headline-lg text-primary mb-6">{unit?.title || "Học từ vựng"}</h1>
        <div className="w-full max-w-md mx-auto flex items-center gap-4">
          <Icon name="auto_stories" size={22} className="text-outline-variant" />
          <div className="flex-1 h-4 bg-surface-variant rounded-full overflow-hidden border-2 border-surface-container-high relative">
            <div className="absolute top-0 left-0 h-full bg-secondary-container rounded-full transition-all duration-500" style={{ width: `${progress}%` }} />
          </div>
          <span className="font-body font-bold text-label-lg text-on-surface-variant whitespace-nowrap">
            {idx + 1} / {vocabs.length}
          </span>
        </div>
      </div>

      {/* Flashcard with 3D flip */}
      <div
        className="relative w-full max-w-sm aspect-[3/4] perspective-1000 cursor-pointer"
        onClick={() => setFlipped((v) => !v)}
      >
        <div
          className="relative w-full h-full transition-transform duration-700 ease-in-out transform-style-preserve-3d shadow-lg rounded-[2rem]"
          style={{ transform: flipped ? "rotateY(180deg)" : "rotateY(0)" }}
        >
          {/* FRONT — English */}
          <div className="absolute inset-0 backface-hidden bg-surface-container-lowest rounded-[2rem] border-4 border-primary-container flex flex-col items-center overflow-hidden">
            <div className="w-[85%] aspect-square mt-5 rounded-2xl bg-surface-container-low border-2 border-surface-variant flex items-center justify-center overflow-hidden relative">
              {current.imageUrl ? (
                <img src={current.imageUrl} alt={current.word} className="w-full h-full object-cover" />
              ) : (
                <Icon name="image" size={64} className="text-outline" />
              )}
              <span className="absolute top-2 right-2 text-secondary-container animate-pulse">
                <Icon name="auto_awesome" size={20} filled />
              </span>
            </div>
            <div className="w-full px-8 mt-3 flex justify-center">
              <div className="flex items-center gap-6">
                <div>
                  <h2 className="font-display text-headline-lg text-primary drop-shadow-sm tracking-wide">{current.word}</h2>
                  {current.ipa && (
                    <p className="font-body text-body-sm text-on-surface-variant italic">/{current.ipa.replace(/^\/|\/$/g, "")}/</p>
                  )}
                </div>
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    speak(current.word);
                  }}
                  disabled={!supported}
                  className="bg-secondary-container text-on-secondary-container w-14 h-14 rounded-full border-b-4 border-secondary-fixed-dim hover:bg-secondary-fixed active:border-b-0 active:translate-y-1 transition-all flex items-center justify-center disabled:opacity-50 shrink-0"
                  aria-label="Phát âm"
                >
                  <Icon name="volume_up" size={28} filled />
                </button>
              </div>
            </div>
            <div className="absolute bottom-4 flex items-center gap-1 text-outline font-body text-label-lg opacity-70">
              <Icon name="touch_app" size={16} /> Bấm để lật thẻ
            </div>
          </div>

          {/* BACK — Vietnamese meaning */}
          <div
            className="absolute inset-0 backface-hidden bg-surface-container-lowest rounded-[2rem] border-4 border-secondary-container flex flex-col items-center justify-center p-8"
            style={{ transform: "rotateY(180deg)" }}
          >
            <div className="mb-4 px-4 py-1 bg-primary-container/20 text-on-primary-container rounded-full font-body font-bold text-label-lg uppercase tracking-wider">
              Tiếng Việt
            </div>
            <h2 className="font-display text-display-lg text-on-surface mb-6 text-center drop-shadow-sm">{current.mean}</h2>
            {current.example && (
              <div className="px-4 py-3 bg-primary-fixed/40 rounded-lg max-w-xs text-center">
                <p className="font-body text-body-md text-on-surface italic">"{current.example}"</p>
              </div>
            )}
            <div className="absolute bottom-4 flex items-center gap-1 text-outline font-body text-label-lg opacity-70">
              <Icon name="replay" size={16} /> Bấm để xem lại
            </div>
          </div>
        </div>
      </div>

      {/* Nav controls */}
      <div className="w-full max-w-sm mx-auto flex items-center justify-between mt-10 gap-4">
        <button
          onClick={() => setIdx((i) => Math.max(0, i - 1))}
          disabled={idx === 0}
          className="btn-outline flex-1 py-4 disabled:opacity-50"
        >
          <Icon name="arrow_back_ios_new" size={18} /> Trước
        </button>
        {idx < vocabs.length - 1 ? (
          <button
            onClick={() => setIdx((i) => i + 1)}
            className="btn-primary flex-[1.5] py-4 text-headline-md"
          >
            Tiếp <Icon name="arrow_forward_ios" size={20} />
          </button>
        ) : (
          <Link to={`/units/${unitId}/quiz`} className="btn-secondary flex-[1.5] py-4 text-headline-md">
            <Icon name="quiz" size={20} filled /> Làm Quiz!
          </Link>
        )}
      </div>
    </div>
  );
}