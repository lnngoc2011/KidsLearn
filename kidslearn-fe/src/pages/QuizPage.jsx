import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams, Link } from "react-router-dom";
import toast from "react-hot-toast";
import { quizApi, unitApi } from "../api/content";
import { useTTS } from "../hooks/useTTS";
import LoadingScreen from "../components/LoadingScreen";
import EmptyState from "../components/EmptyState";
import Icon from "../components/Icon";

export default function QuizPage() {

  const params = useParams();
  const rawUnitId = params.unitId || params.id;
  const gradeId = params.gradeId;
  const reviewNumber = params.reviewNumber;


  const parsedUnitId = rawUnitId ? parseInt(rawUnitId, 10) : 0;

  const navigate = useNavigate();
  const [unit, setUnit] = useState(null);
  const [quizzes, setQuizzes] = useState([]);
  const [answers, setAnswers] = useState({});
  const [idx, setIdx] = useState(0);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const { speak, supported } = useTTS();

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);

        // Quiz của một Unit cụ thể
        if (parsedUnitId > 0) {
          const [u, q] = await Promise.all([
            unitApi.getById(parsedUnitId),
            quizApi.getByUnit(parsedUnitId),
          ]);
          setUnit(u);
          setQuizzes(q || []);
        }
        // Mid Review
        else if (reviewNumber) {
          const q = await quizApi.getMidReview(gradeId, reviewNumber);
          setUnit({ title: `Review ${reviewNumber}`, gradeId });
          setQuizzes(q || []);
        }
        // Final Review
        else {
          const q = await quizApi.getFinalReview(gradeId);
          setUnit({ title: "Final Review", gradeId });
          setQuizzes(q || []);
        }
      } catch (err) {
        console.error("Lỗi tải dữ liệu:", err);
        toast.error("Không thể tải câu hỏi. Vui lòng thử lại!");
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [parsedUnitId, gradeId, reviewNumber]);

  const current = quizzes[idx];
  const totalAnswered = useMemo(() => Object.keys(answers).length, [answers]);
  const progress = quizzes.length ? Math.round(((idx + 1) / quizzes.length) * 100) : 0;
  const allAnswered = quizzes.length > 0 && totalAnswered === quizzes.length;

  if (loading) return <LoadingScreen />;

  if (!quizzes.length) {
    return (
      <EmptyState
        title={unit?.title ? `${unit.title} chưa có câu hỏi` : "Chưa có câu hỏi"}
        hint="Hãy thử bài học khác hoặc quay lại sau nhé!"
        action={<Link to="/grades" className="btn-primary">Về danh sách lớp</Link>}
      />
    );
  }

  const pick = (answerId) => setAnswers((prev) => ({ ...prev, [current.quizId]: answerId }));

  const goNext = () => {
    if (!answers[current.quizId]) return toast.error("Bạn chưa chọn đáp án nào!");
    if (idx < quizzes.length - 1) setIdx(idx + 1);
  };

  // ⭐ FIX: xử lý 2 trường hợp — Quiz Unit thường và Review
  const submit = async () => {
    if (!allAnswered) {
      return toast.error(`Còn ${quizzes.length - totalAnswered} câu chưa trả lời!`);
    }

    setSubmitting(true);

    try {
      let result;

      if (parsedUnitId > 0) {
        // CASE 1: Quiz Unit bình thường
        result = await quizApi.submit({
          unitId: parsedUnitId,
          answers,
        });

        navigate(`/units/${parsedUnitId}/result`, {
          state: {
            result,
            quizzes,
            userAnswers: answers,
            unitTitle: unit?.title,
            gradeId: unit?.gradeId,
          },
        });
      } else {
        // CASE 2 + 3: Review (Mid hoặc Final) — gọi endpoint review/submit
        result = await quizApi.submitReview(answers);

        navigate("/units/review/result", {
          state: {
            result,
            quizzes,
            userAnswers: answers,
            unitTitle: unit?.title,
            gradeId: unit?.gradeId || parseInt(gradeId, 10),
          },
        });
      }
    } catch (err) {
      console.error(err);
      toast.error(
        err?.response?.data?.message || err?.userMessage || "Nộp bài thất bại"
      );
    } finally {
      setSubmitting(false);
    }
  };

  const selected = answers[current.quizId];
  const isAudio = current.questionType === "audio";
  const isImage = current.questionType === "image";

  return (
    <div className="max-w-3xl mx-auto">
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <Link
          to={unit ? `/grades/${unit.gradeId}` : "/grades"}
          className="w-12 h-12 bg-surface-container-lowest border-2 border-outline-variant rounded-full flex items-center justify-center text-on-surface-variant hover:bg-error-container hover:text-on-error-container hover:border-error-container transition-colors border-b-4 border-b-outline"
        >
          <Icon name="close" size={22} />
        </Link>
        <div className="text-center">
          <h2 className="font-display text-headline-md text-on-surface">{unit?.title || "Quiz"}</h2>
          <p className="font-body text-label-lg text-on-surface-variant">
            Câu {idx + 1}/{quizzes.length} · Đã trả lời {totalAnswered}/{quizzes.length}
          </p>
        </div>
        <div className="w-12" /> {/* spacer */}
      </div>

      {/* Progress */}
      <div className="flex items-center gap-3 mb-6">
        <div className="flex-1 h-4 bg-surface-variant rounded-full overflow-hidden border-2 border-surface-container-high relative">
          <div className="absolute top-0 left-0 h-full bg-gradient-to-r from-secondary-container to-tertiary-container rounded-full transition-all duration-500" style={{ width: `${progress}%` }} />
        </div>
        <span className="font-body font-bold text-label-lg text-on-surface-variant">{progress}%</span>
      </div>

      {/* Question card */}
      <div className="card border-b-[6px] border-primary-container">
        <div className="flex items-center gap-2 mb-4">
          <span className="chip bg-tertiary-container/30 text-on-tertiary-container">
            <Icon name="quiz" size={14} /> Câu {idx + 1}
          </span>
          <span className="chip bg-surface-container text-on-surface-variant">
            {isImage ? "Chọn hình" : isAudio ? "Nghe & chọn" : "Trắc nghiệm"}
          </span>
        </div>

        {isAudio ? (
          <div className="flex items-center gap-4 py-6 justify-center">
            <button
              onClick={() => speak(current.ttsText || current.questionText)}
              disabled={!supported}
              className="w-24 h-24 rounded-full bg-primary text-on-primary border-b-4 border-on-primary-fixed-variant hover:bg-primary-container active:translate-y-1 disabled:opacity-50 flex items-center justify-center"
            >
              <Icon name="volume_up" size={40} filled />
            </button>
            <div>
              <p className="font-display text-headline-md">Nghe và chọn đáp án đúng</p>
              <p className="font-body text-body-md text-on-surface-variant mt-1">Bấm vào loa để nghe lại</p>
            </div>
          </div>
        ) : (
          <>
            <h2 className="font-display text-headline-lg text-on-surface">
              {current.questionText}
            </h2>

            {isImage && current.imageUrl && (
              <div className="mt-6 flex justify-center">
                <img
                  src={current.imageUrl}
                  alt=""
                  className="w-64 h-64 object-cover rounded-3xl shadow-lg"
                />
              </div>
            )}
          </>
        )}

        {/* Answers */}
        <div className={`mt-6 grid ${isImage ? "grid-cols-2 sm:grid-cols-4" : "grid-cols-1 sm:grid-cols-2"} gap-3`}>
          {current.answers.map((a, i) => {
            const isSelected = selected === a.answerId;
            const letter = String.fromCharCode(65 + i);
            return (
              <button
                key={a.answerId}
                onClick={() => pick(a.answerId)}
                className={`relative text-left p-4 rounded-lg border-2 transition-all ${
                  isSelected
                    ? "bg-primary text-on-primary border-primary border-b-4 border-b-on-primary-fixed-variant -translate-y-0.5"
                    : "bg-surface-container-lowest border-surface-variant border-b-4 border-b-outline-variant hover:border-primary-container hover:-translate-y-0.5"
                }`}
              >
                <span
                  className={`absolute top-2 right-2 w-7 h-7 rounded-full flex items-center justify-center font-display font-bold text-label-lg ${
                    isSelected ? "bg-white text-primary" : "bg-secondary-fixed text-on-secondary-container"
                  }`}
                >
                  {letter}
                </span>
                {a.answerType === "image" ? (
                  <div className="flex justify-center items-center">
                    <img
                      src={a.imageUrl}
                      alt={a.answerText}
                      className="w-32 h-32 object-contain rounded-md bg-surface-container"
                    />
                  </div>
                ) : a.answerType === "audio" ? (
                  <div className="flex items-center gap-2">
                    <Icon name="volume_up" size={20} />
                    <span>Nghe đáp án</span>
                  </div>
                ) : (
                  <p
                    className={`font-display font-bold pr-9 ${
                      isSelected ? "text-on-primary" : "text-on-surface"
                    }`}
                  >
                    {a.answerText}
                  </p>
                )}
              </button>
            );
          })}
        </div>
      </div>

      {/* Footer controls */}
      <div className="mt-6 flex items-center justify-between gap-3 flex-wrap">
        <div className="flex flex-wrap items-center gap-1.5">
          {quizzes.map((q, i) => {
            const answered = !!answers[q.quizId];
            const isCurrent = i === idx;
            return (
              <button
                key={q.quizId}
                onClick={() => setIdx(i)}
                className={`w-8 h-8 rounded-lg font-display font-bold text-label-lg transition ${
                  isCurrent
                    ? "bg-primary text-on-primary border-b-2 border-on-primary-fixed-variant"
                    : answered
                    ? "bg-secondary-container/30 text-on-secondary-container"
                    : "bg-surface-container-lowest border-2 border-surface-variant text-outline"
                }`}
              >
                {i + 1}
              </button>
            );
          })}
        </div>

        {idx < quizzes.length - 1 ? (
          <button onClick={goNext} className="btn-primary text-body-md">
            Câu tiếp <Icon name="arrow_forward_ios" size={18} />
          </button>
        ) : (
          <button onClick={submit} disabled={submitting} className="btn-secondary text-body-md">
            <Icon name="auto_awesome" size={18} filled />
            {submitting ? "Đang nộp..." : "Nộp bài!"}
          </button>
        )}
      </div>
    </div>
  );
}