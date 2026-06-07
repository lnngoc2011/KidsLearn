import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { progressApi } from "../api/progress";
import LoadingScreen from "../components/LoadingScreen";
import PageHeader from "../components/PageHeader";
import EmptyState from "../components/EmptyState";
import Icon from "../components/Icon";
import { formatDateTime, scoreColor } from "../utils/helpers";

export default function ProgressPage() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try { setItems((await progressApi.getMyHistory()) || []); }
      finally { setLoading(false); }
    })();
  }, []);

  if (loading) return <LoadingScreen />;

  // Compute simple stats
  const total = items.length;
  const avg = total ? Math.round(items.reduce((s, p) => s + p.score, 0) / total) : 0;
  const best = total ? Math.round(Math.max(...items.map((p) => p.score))) : 0;

  return (
    <div>
      <PageHeader title="Tiến độ học tập" subtitle="Lịch sử các Quiz bạn đã hoàn thành." back="/" />

      {/* Stat cards */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-8">
        <div className="card border-b-[4px] border-primary-container">
          <div className="font-body font-bold text-label-lg text-outline uppercase">Tổng lượt</div>
          <div className="font-display text-display-lg text-primary mt-1">{total}</div>
          <div className="font-body text-label-lg text-on-surface-variant">lượt làm bài</div>
        </div>
        <div className="card border-b-[4px] border-secondary-container">
          <div className="font-body font-bold text-label-lg text-outline uppercase">Điểm trung bình</div>
          <div className="font-display text-display-lg text-secondary mt-1">{avg}</div>
          <div className="font-body text-label-lg text-on-surface-variant">/ 100</div>
        </div>
        <div className="card border-b-[4px] border-tertiary-container">
          <div className="font-body font-bold text-label-lg text-outline uppercase">Điểm cao nhất</div>
          <div className="font-display text-display-lg text-tertiary mt-1">{best}</div>
          <div className="font-body text-label-lg text-on-surface-variant">điểm</div>
        </div>
      </div>

      {/* History list */}
      {items.length === 0 ? (
        <EmptyState
          title="Chưa có lượt làm bài nào"
          hint="Bắt đầu Unit đầu tiên để thấy lịch sử ở đây nhé!"
          action={<Link to="/grades" className="btn-primary"><Icon name="menu_book" size={18} /> Bắt đầu học</Link>}
        />
      ) : (
        <div className="space-y-3">
          {items.map((p, i) => (
            <div key={p.progressId} className="card flex items-center gap-4 border-b-[3px]">
              <div className="shrink-0 w-12 h-12 rounded-full bg-primary-fixed flex items-center justify-center font-display font-bold text-primary">
                #{items.length - i}
              </div>
              <div className="flex-1 min-w-0">
                <div className="font-display text-headline-md text-on-surface truncate">{p.unitTitle}</div>
                <div className="font-body text-label-lg text-on-surface-variant">{formatDateTime(p.completedAt)}</div>
              </div>
              <div className="text-right">
                <div className={`font-display text-headline-lg ${scoreColor(p.score)}`}>{Math.round(p.score)}</div>
                <div className="font-body text-label-lg text-outline">/ 100</div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
