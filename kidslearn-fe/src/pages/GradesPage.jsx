import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { gradeApi } from "../api/content";
import LoadingScreen from "../components/LoadingScreen";
import PageHeader from "../components/PageHeader";
import EmptyState from "../components/EmptyState";
import Icon from "../components/Icon";

const palettes = [
  { bg: "bg-primary-fixed", emoji: "🐣", chipBg: "bg-primary-container", chipText: "text-on-primary" },
  { bg: "bg-secondary-fixed", emoji: "🦊", chipBg: "bg-secondary-container", chipText: "text-on-secondary-container" },
  { bg: "bg-tertiary-fixed", emoji: "🐰", chipBg: "bg-tertiary-container", chipText: "text-on-tertiary" },
  { bg: "bg-primary-fixed-dim", emoji: "🦉", chipBg: "bg-primary", chipText: "text-on-primary" },
  { bg: "bg-tertiary-fixed-dim", emoji: "🐯", chipBg: "bg-tertiary", chipText: "text-on-tertiary" },
];

export default function GradesPage() {
  const [grades, setGrades] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try { setGrades((await gradeApi.getAll()) || []); }
      finally { setLoading(false); }
    })();
  }, []);

  if (loading) return <LoadingScreen />;

  return (
    <div>
      <PageHeader
        title="Chọn lớp để bắt đầu"
        subtitle="Bài học được sắp xếp theo chương trình từ Lớp 1 đến Lớp 5."
      />

      {grades.length === 0 ? (
        <EmptyState title="Chưa có khối lớp nào" hint="Hãy quay lại sau nhé!" />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {grades.map((g, i) => {
            const p = palettes[i % palettes.length];
            return (
              <Link
                key={g.gradeId}
                to={`/grades/${g.gradeId}`}
                className={`group rounded-xl ${p.bg} p-6 border-2 border-surface-variant border-b-[6px] hover:-translate-y-1 transition-transform relative overflow-hidden min-h-[200px]`}
              >
                <div className="absolute -top-4 -right-2 text-9xl opacity-90 group-hover:scale-110 transition-transform">
                  {p.emoji}
                </div>
                <div className="relative z-10">
                  <span className={`chip ${p.chipBg} ${p.chipText} mb-3`}>
                    <Icon name="menu_book" size={14} /> {g.unitCount} Unit
                  </span>
                  <h3 className="font-display text-headline-lg text-on-surface mt-2">{g.gradeName}</h3>
                  {g.description && (
                    <p className="font-body text-body-md text-on-surface-variant mt-1">{g.description}</p>
                  )}
                  <div className="mt-6 inline-flex items-center gap-1 font-body font-bold text-on-surface">
                    Bắt đầu <Icon name="arrow_forward" size={20} />
                  </div>
                </div>
              </Link>
            );
          })}
        </div>
      )}
    </div>
  );
}
