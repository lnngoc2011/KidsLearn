import { useEffect, useMemo, useState } from "react";
import { Link, useLocation, useNavigate, useParams } from "react-router-dom";
import Confetti from "react-confetti";
import { useAuth } from "../context/AuthContext";
import Icon from "../components/Icon";

export default function QuizResultPage() {
  const { unitId } = useParams();
  const navigate = useNavigate();
  const { state } = useLocation();
  const { refreshProfile } = useAuth();

  const gradeId     = state?.gradeId;
  const result      = state?.result;
  const unitTitle   = state?.unitTitle || "Unit";
  const quizzes     = state?.quizzes     || [];  
  const userAnswers = state?.userAnswers || {};   
  const [showConfetti, setShowConfetti] = useState(true);
  const [size, setSize] = useState({ w: window.innerWidth, h: window.innerHeight });

  const isReviewMode = !unitId;

  useEffect(() => {
    if (!result) {
      if (isReviewMode) {
        navigate("/grades", { replace: true });
      } else {
        navigate(`/units/${unitId}/quiz`, { replace: true });
      }
      return;
    }
    refreshProfile();
    const t = setTimeout(() => setShowConfetti(false), 5000);
    const onResize = () => setSize({ w: window.innerWidth, h: window.innerHeight });
    window.addEventListener("resize", onResize);
    return () => { clearTimeout(t); window.removeEventListener("resize", onResize); };
    // eslint-disable-next-line
  }, []);

  const stars = result?.starCount ?? 0;
  const score = Math.round(result?.score ?? 0);

  const tier = useMemo(() => {
    if (score >= 90) return { label: "Xuất sắc!", emoji: "🏆", gradient: "from-secondary-fixed to-tertiary-fixed", border: "border-secondary-container" };
    if (score >= 70) return { label: "Tốt lắm!",  emoji: "🌟", gradient: "from-primary-fixed to-primary-fixed-dim", border: "border-primary-container" };
    if (score >= 50) return { label: "Khá rồi!",  emoji: "💪", gradient: "from-primary-fixed to-secondary-fixed", border: "border-secondary-container" };
    return { label: "Cố lên nhé!", emoji: "🌱", gradient: "from-tertiary-fixed to-primary-fixed", border: "border-tertiary-container" };
  }, [score]);

  if (!result) return null;

  return (
    <div>
      {showConfetti && stars >= 2 && (
        <Confetti width={size.w} height={size.h} numberOfPieces={250} recycle={false} gravity={0.25} />
      )}

      {/* Hero celebration */}
      <div className={`rounded-xl bg-gradient-to-br ${tier.gradient} p-8 lg:p-12 text-center relative overflow-hidden border-4 ${tier.border} border-b-[8px]`}>
        <div className="absolute inset-0 dotted-texture opacity-30" />
        <div className="relative z-10">
          <div className="text-7xl mb-4">{tier.emoji}</div>
          <span className="inline-flex items-center gap-1 chip bg-white/80 text-on-primary-container backdrop-blur mb-2">
            <Icon name="auto_awesome" size={14} filled /> {unitTitle}
          </span>
          <h1 className="font-display text-display-lg text-on-primary-container">{tier.label}</h1>
          <p className="font-body text-body-lg text-on-primary-container/80 mt-1">
            {result.motivationalMessage || "Tuyệt vời! Em đã làm rất tốt!"}
          </p>

          {/* Stars */}
          <div className="mt-6 flex items-center justify-center gap-2">
            {[0, 1, 2].map((i) => (
              <Icon
                key={i}
                name="star"
                size={56}
                filled={i < stars}
                className={i < stars ? "text-secondary-container animate-ringPop" : "text-on-surface/15"}
                style={{ animationDelay: `${i * 150}ms` }}
              />
            ))}
          </div>

          {/* Score */}
          <div className="mt-4 inline-block bg-surface-container-lowest rounded-lg px-8 py-3 border-2 border-surface-variant border-b-4">
            <span className="font-display text-display-lg text-primary">{score}</span>
            <span className="font-body text-body-md text-outline ml-1">/ 100</span>
          </div>
          <div className="mt-3 font-body font-bold text-body-md text-on-primary-container">
            Đúng <span className="text-primary">{result.correctAnswers}</span> / {result.totalQuestions} câu
          </div>
        </div>
      </div>

      {/* Rewards grid */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mt-6">
        <RewardCard icon="bolt"                 label="XP nhận được" value={`+${result.xpGained}`}              sub={`Tổng ${result.totalXP} XP`}                  color="bg-secondary-fixed text-on-secondary-container" />
        <RewardCard icon="emoji_events"        label="Cấp độ"       value={`Lv ${result.level}`}               sub={result.leveledUp ? "🎉 Lên cấp!" : result.levelName} highlight={result.leveledUp} color="bg-tertiary-fixed text-on-tertiary-container" />
        <RewardCard icon="local_fire_department" label="Streak"     value={`${result.currentStreak} ngày`}     sub={result.isStreakMilestone ? "🔥 Mốc mới!" : "Tiếp tục nhé"} highlight={result.isStreakMilestone} color="bg-primary-fixed text-on-primary-container" />
        <RewardCard icon="military_tech"       label="Huy hiệu"     value={result.newBadges?.length || 0}       sub={result.newBadges?.length ? "Mở khóa!" : "Lần này chưa có"} highlight={(result.newBadges?.length || 0) > 0} color="bg-secondary-fixed text-on-secondary-container" />
      </div>

      {/* New badges */}
      {result.newBadges?.length > 0 && (
        <div className="mt-6 card border-b-[6px] border-secondary-container bg-gradient-to-br from-secondary-fixed to-tertiary-fixed">
          <h3 className="font-display text-headline-md flex items-center gap-2 text-on-secondary-container">
            <Icon name="military_tech" size={28} filled className="text-tertiary" /> Huy hiệu mới mở khóa
          </h3>
          <div className="mt-4 grid grid-cols-2 md:grid-cols-4 gap-4">
            {result.newBadges.map((b) => (
              <div key={b.badgeId} className="card bg-surface-container-lowest text-center border-b-[4px] border-secondary-fixed-dim">
                <div className="w-14 h-14 mx-auto rounded-full bg-secondary-fixed flex items-center justify-center mb-2">
                  {b.iconUrl
                    ? <img src={b.iconUrl} alt="" className="w-10 h-10" />
                    : <Icon name="military_tech" size={28} filled className="text-tertiary" />}
                </div>
                <div className="font-display font-bold text-body-md text-on-surface">{b.name}</div>
                {b.description && <div className="font-body text-label-lg text-on-surface-variant mt-1">{b.description}</div>}
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Streak milestone */}
      {result.streakMessage && (
        <div className="mt-6 card bg-tertiary-fixed border-tertiary-container border-b-[6px] text-center">
          <div className="text-4xl">🔥</div>
          <p className="font-display text-headline-md text-tertiary mt-2">{result.streakMessage}</p>
        </div>
      )}

      {/* Answer review */}
      {quizzes.length > 0 && (
        <div className="mt-6 card">
          <h3 className="font-display text-headline-md flex items-center gap-2 text-on-surface mb-4">
            <Icon name="fact_check" size={24} filled className="text-primary" />
            Xem lại đáp án
          </h3>

          <div className="space-y-3">
            {quizzes.map((quiz, idx) => {
              const detail = result.details?.find(
                (d) => d.quizId === quiz.quizId
              );

              const isCorrect = detail?.isCorrect;

              // Tìm thông tin đáp án từ quiz.answers gốc theo yêu cầu của bạn
              const selectedAnswer = quiz.answers?.find(
                a => a.answerId === detail?.selectedAnswerId
              );

              const correctAnswer = quiz.answers?.find(
                a => a.answerId === detail?.correctAnswerId
              );

              return (
                <div
                  key={quiz.quizId}
                  className={`flex items-start gap-3 p-4 rounded-xl border-2 ${
                    isCorrect
                      ? "border-green-300 bg-green-50"
                      : "border-red-300 bg-red-50"
                  }`}
                >
                  {/* Icon */}
                  <span className="text-xl mt-0.5">
                    {isCorrect ? "✅" : "❌"}
                  </span>

                  {/* Content */}
                  <div className="flex-1 min-w-0">
                    <p className="font-body font-bold text-body-md text-on-surface">
                      Câu {idx + 1}
                    </p>

                    <p className="text-sm text-slate-700 mt-1">
                      {detail?.questionText}
                    </p>

                    {/* TRƯỜNG HỢP LÀM SAI */}
                    {!isCorrect && detail && (
                      <div className="mt-2 space-y-3">
                        <div>
                          <div className="text-red-700 text-sm">
                            Bạn chọn:
                          </div>
                          {selectedAnswer?.imageUrl ? (
                            <img
                              src={selectedAnswer?.imageUrl}
                              alt="selected"
                              className="h-16 rounded border mt-1" 
                            />
                          ) : (
                            <div className="font-bold text-red-700">
                              {selectedAnswer?.answerText}
                            </div>
                          )}
                        </div>

                        <div>
                          <div className="text-green-700 text-sm">
                            Đáp án đúng:
                          </div>
                          {correctAnswer?.imageUrl ? (
                            <img
                              src={correctAnswer?.imageUrl}
                              alt="correct"
                              className="h-16 rounded border border-green-400 mt-1" 
                            />
                          ) : (
                            <div className="font-bold text-green-700">
                              {correctAnswer?.answerText}
                            </div>
                          )}
                        </div>
                      </div>
                    )}

                    {/* TRƯỜNG HỢP LÀM ĐÚNG */}
                    {isCorrect && (
                      <div className="mt-2">
                        <div className="text-green-700 text-sm">
                          Đáp án đúng bạn đã chọn:
                        </div>
                        {selectedAnswer?.imageUrl ? (
                          <img
                            src={selectedAnswer?.imageUrl}
                            alt="correct-selected"
                            className="h-16 rounded border border-green-400 mt-1" 
                          />
                        ) : (
                          <div className="font-bold text-green-700 mt-1 text-sm">
                            {selectedAnswer?.answerText}
                          </div>
                        )}
                      </div>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* Actions */}
      <div className="mt-8 flex flex-wrap gap-3 justify-center">
        {!isReviewMode ? (
          <>
            <Link to={`/units/${unitId}/quiz`} className="btn-primary">
              <Icon name="restart_alt" size={20} /> Làm lại Quiz
            </Link>
            <Link to={`/units/${unitId}/learn`} className="btn-outline">
              <Icon name="menu_book" size={20} /> Học lại từ vựng
            </Link>
          </>
        ) : (
          <button onClick={() => navigate(-1)} className="btn-primary">
            <Icon name="restart_alt" size={20} /> Thử sức bài khác
          </button>
        )}
        
        <Link to="/progress" className="btn-ghost px-6 py-3">
          <Icon name="history" size={20} /> Xem lịch sử
        </Link>
        <button onClick={() => navigate(gradeId ? `/grades/${gradeId}` : "/grades")} className="btn-secondary">
          <Icon name="auto_awesome" size={20} />
          Học Unit khác
        </button>
      </div>
    </div>
  );
}

function RewardCard({ icon, label, value, sub, highlight, color }) {
  return (
    <div className={`card text-center ${highlight ? "ring-4 ring-secondary-container/40" : ""}`}>
      <div className={`w-12 h-12 mx-auto rounded-full ${color} flex items-center justify-center mb-2`}>
        <Icon name={icon} size={24} filled />
      </div>
      <div className="font-body font-bold text-label-lg text-outline uppercase">{label}</div>
      <div className="font-display text-headline-md text-on-surface mt-1">{value}</div>
      <div className="font-body text-label-lg text-on-surface-variant mt-0.5 truncate">{sub}</div>
    </div>
  );
}