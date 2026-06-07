import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { progressApi } from "../api/progress";
import { gradeApi } from "../api/content";
import { activityApi } from "../api/activity";
import LoadingScreen from "../components/LoadingScreen";
import Icon from "../components/Icon";

export default function HomePage() {
  const { profile, loadingProfile, user } = useAuth();
  const [grades, setGrades] = useState([]);
  const [recent, setRecent] = useState([]);
  const [latestUnit, setLatestUnit] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try {
        const [g, h, latest] = await Promise.all([
          gradeApi.getAll(),
          progressApi.getMyHistory(),
          activityApi.getLatest(),
        ]);
        setGrades(g || []);
        setRecent((h || []).slice(0, 3));
        setLatestUnit(latest || null);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  if (loadingProfile || loading) return <LoadingScreen />;

  const name = profile?.fullName || user?.username || "bạn";
  const totalXP = profile?.totalXP ?? 0;
  const level = profile?.level ?? 1;
  const xpInLevel = totalXP % 2000;
  const xpPct = Math.round((xpInLevel / 2000) * 100);
  const xpToNext = 2000 - xpInLevel;

  return (
    <div className="flex flex-col gap-12">
      {/* HERO + STATS */}
      <section className="grid grid-cols-1 lg:grid-cols-12 gap-gutter">

        {/* Greeting hero (8 cols) */}
        <div className="lg:col-span-8 bg-gradient-to-br from-primary-fixed to-inverse-primary rounded-xl p-8 border-b-[6px] border-primary-container relative overflow-hidden min-h-[320px] flex flex-col justify-between">
          <div className="absolute -top-10 -right-10 w-64 h-64 bg-white opacity-20 rounded-full blur-3xl" />
          <div className="absolute -bottom-20 left-20 w-48 h-48 bg-secondary-fixed opacity-30 rounded-full blur-2xl" />

          <div className="relative z-10">
            <span className="inline-flex items-center gap-1 bg-white/50 text-on-primary-container font-body font-bold text-label-lg px-4 py-1 rounded-full mb-4 border border-white/40 backdrop-blur-sm">
              <Icon name="auto_awesome" size={16} />
              Trở lại học tập
            </span>
            <h1 className="font-display text-display-lg text-on-primary-container mb-2 drop-shadow-sm">
              Chào mừng bạn,<br />
              <span className="text-primary">{name}!</span>
            </h1>
            <p className="font-body text-body-lg text-on-primary-container opacity-90 mb-8">
              Học tiếng Anh thật vui! Tiếp tục cuộc hành trình của bạn ngay hôm nay.
            </p>

            <Link
              to={latestUnit ? `/units/${latestUnit.unitId}/learn` : "/grades"}
              className="btn-tactile bg-secondary-container text-on-secondary-container px-8 py-4 rounded-full border-b-[6px] border-[#d4a822] shadow-sm text-[18px] font-display font-bold inline-flex items-center gap-2"
            >
              <Icon name="play_arrow" size={22} filled />
              {latestUnit ? (
                <span>
                  Tiếp tục: <span className="underline">{latestUnit.unitTitle}</span>
                  <span className="ml-2 text-label-lg opacity-80">
                    ({Math.min(latestUnit.lastVocabIndex + 1, latestUnit.totalVocabs)}/{latestUnit.totalVocabs})
                  </span>
                </span>
              ) : (
                "Bắt đầu học"
              )}
            </Link>
          </div>
        </div>

        {/* Stats stack (4 cols) */}
        <div className="lg:col-span-4 flex flex-col gap-6">
          {/* Level widget */}
          <div className="bg-surface-container-lowest rounded-lg p-6 border-2 border-surface-variant border-b-[4px] flex-1 flex flex-col justify-center">
            <div className="flex justify-between items-end mb-4">
              <div>
                <p className="font-body font-bold text-label-lg text-outline mb-1 uppercase tracking-wider">Cấp Độ Hiện Tại</p>
                <h2 className="font-display text-headline-lg text-primary flex items-center gap-2">
                  <Icon name="star" size={36} filled className="text-secondary-container" />
                  Level {level}
                </h2>
              </div>
              <span className="font-body font-bold text-label-lg text-primary-container">
                {xpInLevel} / 2000 XP
              </span>
            </div>
            <div className="w-full bg-surface-container-highest rounded-full h-6 border-2 border-surface-variant overflow-hidden relative">
              <div className="bg-primary h-full rounded-full relative transition-all" style={{ width: `${xpPct}%` }}>
                <div className="absolute top-0 left-0 right-0 h-1/2 bg-white/20 rounded-t-full" />
              </div>
            </div>
            <p className="font-body text-body-md text-on-surface-variant mt-3 text-center">
              Còn {xpToNext} XP để lên cấp {level + 1}! Cố lên!
            </p>
          </div>

          {/* Streak widget */}
          <div className="bg-surface-container-lowest rounded-lg p-6 border-2 border-tertiary-fixed border-b-[4px] flex items-center gap-6">
            <div className="w-16 h-16 rounded-full bg-tertiary-fixed flex items-center justify-center border-4 border-white shrink-0">
              <Icon name="local_fire_department" size={36} filled className="text-tertiary" />
            </div>
            <div>
              <h3 className="font-display text-headline-md text-on-surface mb-1">Chuỗi Ngày Học</h3>
              <p className="font-body text-body-lg text-tertiary font-bold">
                {profile?.currentStreak ?? 0} Ngày Liên Tiếp
              </p>
            </div>
          </div>
        </div>

      </section>

      {/* GRADES QUICK ACCESS */}
      <section>
        <div className="flex justify-between items-center mb-6">
          <h2 className="font-display text-headline-lg text-on-surface">Chọn lớp để học</h2>
          <Link to="/grades" className="font-body font-bold text-label-lg text-primary hover:underline flex items-center gap-1">
            Tất cả <Icon name="arrow_forward" size={18} />
          </Link>
        </div>
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-5 gap-4">
          {grades.map((g, i) => {
            const emojis = ["🐣", "🦊", "🐰", "🦉", "🐯"];
            const bgs = ["bg-primary-fixed", "bg-secondary-fixed", "bg-tertiary-fixed", "bg-primary-fixed-dim", "bg-tertiary-fixed-dim"];
            return (
              <Link
                key={g.gradeId}
                to={`/grades/${g.gradeId}`}
                className={`card-interactive ${bgs[i % bgs.length]} text-center border-2 border-surface-variant border-b-[6px]`}
              >
                <div className="text-5xl mb-2">{emojis[i % emojis.length]}</div>
                <div className="font-display font-bold text-on-surface leading-tight">{g.gradeName}</div>
                <div className="text-label-lg text-on-surface-variant font-body font-bold mt-1">{g.unitCount} Unit</div>
              </Link>
            );
          })}
        </div>
      </section>

      {/* RECENT ACTIVITY */}
      <section>
        <div className="flex justify-between items-center mb-6">
          <h2 className="font-display text-headline-lg text-on-surface">Hoạt động gần đây</h2>
          <Link to="/progress" className="font-body font-bold text-label-lg text-primary hover:underline flex items-center gap-1">
            <Icon name="history" size={18} /> Lịch sử
          </Link>
        </div>
        {recent.length === 0 ? (
          <div className="card text-center py-10">
            <Icon name="menu_book" size={48} className="text-outline mx-auto mb-2" />
            <p className="font-body font-bold text-on-surface-variant">Chưa có hoạt động nào. Bắt đầu Unit đầu tiên nào!</p>
          </div>
        ) : (
          <div className="space-y-3">
            {recent.map((r) => (
              <div key={r.progressId} className="card flex items-center gap-4 py-3">
                <div className="w-12 h-12 rounded-full bg-primary-fixed flex items-center justify-center">
                  <Icon name="menu_book" size={24} className="text-primary" />
                </div>
                <div className="flex-1 min-w-0">
                  <div className="font-display font-bold text-on-surface truncate">{r.unitTitle}</div>
                  <div className="font-body text-label-lg text-on-surface-variant">
                    {new Date(r.completedAt).toLocaleDateString("vi-VN")}
                  </div>
                </div>
                <div className="text-right">
                  <div className="font-display text-headline-md text-primary">{Math.round(r.score)}</div>
                  <div className="font-body text-label-lg text-outline">điểm</div>
                </div>
              </div>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}