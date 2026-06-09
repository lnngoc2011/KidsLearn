import { useEffect, useState } from "react";
import toast from "react-hot-toast";
import { useAuth } from "../context/AuthContext";
import { userApi } from "../api/user";
import LoadingScreen from "../components/LoadingScreen";
import PageHeader from "../components/PageHeader";
import Icon from "../components/Icon";
import ImageUpload from "../components/ImageUpload";
import { getLevelInfo } from "../utils/helpers"; // ⭐ IMPORT

export default function ProfilePage() {
  const { profile, loadingProfile, refreshProfile, user } = useAuth();
  const [tab, setTab] = useState("info");
  const [form, setForm] = useState({ fullName: "", avatarFile: null, avatarUrl: "" });
  const [pwForm, setPwForm] = useState({ oldPassword: "", newPassword: "" });
  const [savingProfile, setSavingProfile] = useState(false);
  const [savingPw, setSavingPw] = useState(false);

  useEffect(() => {
    if (profile) setForm({ fullName: profile.fullName || "", avatarFile: null, avatarUrl: profile.avatarUrl || "" });
  }, [profile]);

  if (loadingProfile || !profile) return <LoadingScreen />;

  const saveProfile = async (e) => {
    e.preventDefault();
    setSavingProfile(true);
    try {
      // ✅ Gọi 2 API riêng: 1 cho thông tin, 1 cho avatar
      // Update fullName (JSON)
      await userApi.updateMyProfile({ fullName: form.fullName });

      // Update avatar nếu có file mới (multipart)
      if (form.avatarFile) {
        await userApi.updateAvatar(form.avatarFile);
      }

      await refreshProfile();
      toast.success("Đã cập nhật hồ sơ!");
      setForm({ ...form, avatarFile: null }); // reset file sau khi upload
    } catch (err) {
      toast.error(err.userMessage || "Cập nhật thất bại");
    } finally {
      setSavingProfile(false);
    }
  };

  const changePw = async (e) => {
    e.preventDefault();
    if (pwForm.newPassword.length < 6) return toast.error("Mật khẩu mới tối thiểu 6 ký tự");
    setSavingPw(true);
    try {
      await userApi.changePassword(pwForm);
      toast.success("Đổi mật khẩu thành công!");
      setPwForm({ oldPassword: "", newPassword: "" });
    } catch (err) { toast.error(err.userMessage || "Đổi mật khẩu thất bại"); }
    finally { setSavingPw(false); }
  };

  // ⭐ TÍNH LEVEL ĐÚNG — thay cho 3 dòng cũ dùng % 2000
  const lvl = getLevelInfo(profile.totalXP ?? 0);

  return (
    <div>
      <PageHeader title="Hồ sơ của tôi" back="/" />

      {/* Hero profile card */}
      <div className="rounded-xl bg-gradient-to-br from-primary-fixed to-secondary-fixed p-8 border-b-[6px] border-primary-container relative overflow-hidden mb-6">
        <div className="absolute -top-10 -right-10 w-48 h-48 bg-white opacity-20 rounded-full blur-3xl" />
        <div className="relative flex flex-col sm:flex-row items-center gap-6">
          <div className="w-28 h-28 rounded-full border-4 border-white shadow-lg overflow-hidden bg-surface-container-lowest">
            {profile.avatarUrl ? (
              <img src={profile.avatarUrl} alt="" className="w-full h-full object-cover" />
            ) : (
              <div className="w-full h-full bg-gradient-to-br from-primary to-tertiary flex items-center justify-center text-on-primary font-display font-extrabold text-display-lg">
                {(profile.fullName || user?.username || "U").slice(0, 1).toUpperCase()}
              </div>
            )}
          </div>
          <div className="flex-1 text-center sm:text-left">
            <h2 className="font-display text-headline-lg text-on-primary-container">{profile.fullName || user?.username}</h2>
            <p className="font-body text-body-md text-on-primary-container/80">@{profile.username}</p>
            <div className="mt-3 flex flex-wrap gap-2 justify-center sm:justify-start">
              <span className="chip bg-tertiary-fixed text-on-tertiary-container">
                <Icon name="local_fire_department" size={14} filled className="text-tertiary" /> {profile.currentStreak} ngày
              </span>
              <span className="chip bg-secondary-fixed text-on-secondary-container">
                <Icon name="star" size={14} filled className="text-secondary-container" /> {profile.totalXP} XP
              </span>
              <span className="chip bg-primary-fixed text-on-primary-container">
                Lv {profile.level} · {profile.levelName}
              </span>
            </div>
          </div>
        </div>

        {/* XP bar */}
        <div className="mt-6 relative">
          <div className="flex justify-between mb-1.5">
            <span className="font-body font-bold text-label-lg text-on-primary-container">
              Cấp độ {lvl.level}
            </span>
            {/* ⭐ XP / NGƯỠNG LEVEL KẾ TIẾP */}
            <span className="font-body font-bold text-label-lg text-on-primary-container">
              {lvl.isMaxLevel
                ? `${profile.totalXP} XP — MAX`
                : `${profile.totalXP} / ${lvl.nextLevelXP} XP`}
            </span>
          </div>
          <div className="w-full bg-surface-container-highest rounded-full h-4 border-2 border-surface-variant overflow-hidden">
            <div className="bg-primary h-full rounded-full relative" style={{ width: `${lvl.progress}%` }}>
              <div className="absolute top-0 left-0 right-0 h-1/2 bg-white/20 rounded-t-full" />
            </div>
          </div>
          {!lvl.isMaxLevel && (
            <p className="font-body text-label-lg text-on-primary-container/80 mt-1 text-right">
              Còn {lvl.remainingXP} XP để lên cấp {lvl.level + 1}
            </p>
          )}
        </div>
      </div>

      {/* Stats row */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
        <StatCard icon="menu_book" label="Unit đã thử" value={profile.totalUnitsAttempted} color="text-primary" bg="bg-primary-fixed" />
        <StatCard icon="emoji_events" label="Hoàn thành" value={profile.totalUnitsCompleted} color="text-tertiary" bg="bg-tertiary-fixed" />
        <StatCard icon="grade" label="Điểm TB" value={Math.round(profile.averageScore)} color="text-secondary" bg="bg-secondary-fixed" />
        <StatCard icon="military_tech" label="Huy hiệu" value={profile.badgesEarned} color="text-tertiary" bg="bg-tertiary-fixed-dim" />
      </div>

      {/* Tabs */}
      <div className="flex gap-2 mb-4">
        <button
          onClick={() => setTab("info")}
          className={`px-4 py-2 rounded-full font-body font-bold text-label-lg transition ${
            tab === "info" ? "bg-primary text-on-primary border-b-2 border-on-primary-fixed-variant" : "bg-surface-container hover:bg-surface-container-high text-on-surface-variant"
          }`}
        >
          <Icon name="person" size={16} className="inline mr-1" /> Thông tin
        </button>
        <button
          onClick={() => setTab("password")}
          className={`px-4 py-2 rounded-full font-body font-bold text-label-lg transition ${
            tab === "password" ? "bg-primary text-on-primary border-b-2 border-on-primary-fixed-variant" : "bg-surface-container hover:bg-surface-container-high text-on-surface-variant"
          }`}
        >
          <Icon name="key" size={16} className="inline mr-1" /> Đổi mật khẩu
        </button>
      </div>

      {tab === "info" && (
        <form onSubmit={saveProfile} className="card max-w-2xl space-y-4 border-b-[3px]">
          <div>
            <label className="block font-body font-bold text-label-lg text-on-surface-variant mb-2">Họ và tên</label>
            <input className="chunky-input" placeholder="Nhập họ và tên"
              value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} />
          </div>
          <div>
            <label className="block font-body font-bold text-label-lg text-on-surface-variant mb-2">Ảnh đại diện</label>
            <ImageUpload
              value={form.avatarFile}
              currentUrl={form.avatarUrl}
              onChange={(file) => setForm({ ...form, avatarFile: file })}
            />
          </div>
          <button type="submit" disabled={savingProfile} className="btn-primary">
            <Icon name="save" size={20} /> {savingProfile ? "Đang lưu..." : "Lưu thay đổi"}
          </button>
        </form>
      )}

      {tab === "password" && (
        <form onSubmit={changePw} className="card max-w-2xl space-y-4 border-b-[3px]">
          <div>
            <label className="block font-body font-bold text-label-lg text-on-surface-variant mb-2">Mật khẩu hiện tại</label>
            <input type="password" className="chunky-input"
              value={pwForm.oldPassword} onChange={(e) => setPwForm({ ...pwForm, oldPassword: e.target.value })} required />
          </div>
          <div>
            <label className="block font-body font-bold text-label-lg text-on-surface-variant mb-2">Mật khẩu mới</label>
            <input type="password" className="chunky-input" placeholder="Tối thiểu 6 ký tự"
              value={pwForm.newPassword} onChange={(e) => setPwForm({ ...pwForm, newPassword: e.target.value })} required />
          </div>
          <button type="submit" disabled={savingPw} className="btn-primary">
            <Icon name="key" size={20} /> {savingPw ? "Đang đổi..." : "Đổi mật khẩu"}
          </button>
        </form>
      )}
    </div>
  );
}

function StatCard({ icon, label, value, color, bg }) {
  return (
    <div className="card flex items-center gap-3 border-b-[3px]">
      <div className={`shrink-0 w-12 h-12 rounded-full ${bg} flex items-center justify-center`}>
        <Icon name={icon} size={24} className={color} filled />
      </div>
      <div className="min-w-0">
        <div className="font-display text-headline-md text-on-surface leading-none">{value ?? 0}</div>
        <div className="font-body text-label-lg text-on-surface-variant font-bold mt-1">{label}</div>
      </div>
    </div>
  );
}