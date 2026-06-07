import { useEffect, useState } from "react";
import { badgeApi } from "../api/progress";
import LoadingScreen from "../components/LoadingScreen";
import PageHeader from "../components/PageHeader";
import Icon from "../components/Icon";
import { formatDate } from "../utils/helpers";

export default function BadgesPage() {
  const [badges, setBadges] = useState([]);
  const [loading, setLoading] = useState(true);
  const [tab, setTab] = useState("all");

  useEffect(() => {
    (async () => {
      try { setBadges((await badgeApi.getAllWithStatus()) || []); }
      finally { setLoading(false); }
    })();
  }, []);

  if (loading) return <LoadingScreen />;

  const filtered = tab === "earned" ? badges.filter(b => b.isEarned)
                  : tab === "locked" ? badges.filter(b => !b.isEarned)
                  : badges;
  const earnedCount = badges.filter(b => b.isEarned).length;

  return (
    <div>
      <PageHeader
        title="Bộ sưu tập huy hiệu"
        subtitle={`Đã đạt ${earnedCount}/${badges.length} huy hiệu`}
        back="/"
        right={<Icon name="military_tech" size={48} filled className="text-secondary-container" />}
      />

      {/* Tabs */}
      <div className="flex gap-2 mb-6">
        {[
          { id: "all", label: `Tất cả (${badges.length})` },
          { id: "earned", label: `Đã đạt (${earnedCount})` },
          { id: "locked", label: `Chưa đạt (${badges.length - earnedCount})` },
        ].map((t) => (
          <button
            key={t.id}
            onClick={() => setTab(t.id)}
            className={`px-4 py-2 rounded-full font-body font-bold text-label-lg transition ${
              tab === t.id ? "bg-primary text-on-primary border-b-2 border-on-primary-fixed-variant" : "bg-surface-container hover:bg-surface-container-high text-on-surface-variant"
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {filtered.length === 0 ? (
        <div className="card text-center py-12">
          <Icon name={tab === "earned" ? "military_tech" : "lock"} size={64} className="text-outline mx-auto mb-3" />
          <p className="font-display text-headline-md text-on-surface-variant">
            {tab === "earned" ? "Chưa có huy hiệu nào — hãy bắt đầu làm Quiz!" : "Bạn đã sưu tầm hết rồi! Tuyệt vời!"}
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-4">
          {filtered.map((b) => (
            <div
              key={b.badgeId}
              className={`card-interactive text-center relative ${
                b.isEarned
                  ? "bg-gradient-to-br from-secondary-fixed to-tertiary-fixed border-secondary-container"
                  : "opacity-70"
              }`}
            >
              {!b.isEarned && (
                <div className="absolute top-2 right-2 w-7 h-7 rounded-full bg-surface-container flex items-center justify-center">
                  <Icon name="lock" size={14} className="text-outline" />
                </div>
              )}
              <div className={`w-20 h-20 mx-auto rounded-full flex items-center justify-center ${
                b.isEarned ? "bg-surface-container-lowest" : "bg-surface-container grayscale"
              }`}>
                {b.iconUrl
                  ? <img src={b.iconUrl} alt="" className="w-14 h-14" />
                  : <Icon name="military_tech" size={40} filled className={b.isEarned ? "text-tertiary" : "text-outline"} />}
              </div>
              <div className="font-display font-bold text-body-md mt-3 text-on-surface">{b.name}</div>
              {b.description && (
                <div className="font-body text-label-lg text-on-surface-variant mt-1 line-clamp-2">{b.description}</div>
              )}
              {b.isEarned && b.earnedAt && (
                <div className="font-body font-bold text-label-lg text-primary mt-2 flex items-center justify-center gap-1">
                  <Icon name="check_circle" size={14} filled /> {formatDate(b.earnedAt)}
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
