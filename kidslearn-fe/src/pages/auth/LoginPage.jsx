import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import toast from "react-hot-toast";
import { useAuth } from "../../context/AuthContext";
import Icon from "../../components/Icon";

export default function LoginPage() {
  const { login, user } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [form, setForm] = useState({ username: "", password: "" });
  const [showPw, setShowPw] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  if (user) {
    const dest = user.role === "Admin" ? "/admin" : "/";
    navigate(dest, { replace: true });
    return null;
  }

  const onSubmit = async (e) => {
    e.preventDefault();
    if (!form.username || !form.password) return toast.error("Vui lòng nhập đủ thông tin nhé!");
    setSubmitting(true);
    try {
      const data = await login(form);
      toast.success(data.role === "Admin" ? "Chào mừng Admin! 🛡️" : "Chào mừng quay lại! 🎉");
      const fallback = data.role === "Admin" ? "/admin" : "/";
      navigate(location.state?.from?.pathname || fallback, { replace: true });
    } catch (err) {
      toast.error(err.userMessage || "Đăng nhập thất bại");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen flex bg-background">
      {/* LEFT — Brand panel */}
      <div className="hidden lg:flex w-1/2 relative bg-gradient-to-br from-primary-fixed to-tertiary-fixed-dim overflow-hidden">
        <div className="absolute inset-0 dotted-texture opacity-60" />
        {/* Decorative blobs */}
        <div className="absolute -top-20 -left-20 w-64 h-64 bg-secondary-fixed rounded-full mix-blend-multiply filter blur-3xl opacity-50 animate-pulse" />
        <div className="absolute -bottom-20 -right-20 w-80 h-80 bg-primary-container rounded-full mix-blend-multiply filter blur-3xl opacity-50 animate-pulse" style={{ animationDelay: "2s" }} />

        <div className="relative z-10 flex flex-col justify-center items-center w-full p-container-margin-desktop text-center">
          <div className="mb-12 ">
            <div className="w-64 h-64 rounded-full bg-surface-container-lowest shadow-2xl border-8 border-white/50 flex items-center justify-center overflow-hidden">
              <img src="/logo.png" alt="KidsLearn mascot" className="w-full h-full object-contain" />
            </div>
          </div>
          <h1 className="font-display text-display-lg text-on-primary-fixed mb-6 drop-shadow-sm">
            Học tiếng Anh <span className="text-tertiary">thật vui!</span>
          </h1>
          <p className="font-body text-body-lg text-on-primary-fixed-variant mb-8 max-w-md">
            Khám phá hành trình học tập đầy màu sắc cùng hàng ngàn bài học tương tác thú vị dành cho bé.
          </p>
          <div className="flex flex-wrap justify-center gap-3">
           
          </div>
        </div>
      </div>

      {/* RIGHT — Form */}
      <div className="w-full lg:w-1/2 flex flex-col justify-center items-center bg-surface-container-lowest p-8 relative">
        <div className="lg:hidden mb-8 text-center">
          <h2 className="font-display text-headline-lg text-primary font-extrabold">KidsLearn</h2>
        </div>

        <div className="w-full max-w-md space-y-8">
          <div className="text-left">
            <span className="inline-block px-4 py-1.5 rounded-full bg-secondary-container/30 text-on-secondary-container font-body font-bold text-label-lg mb-4">
              ✨ Chào mừng quay lại
            </span>
            <h2 className="font-display text-display-lg text-on-background tracking-tight">Đăng nhập</h2>
            
          </div>

          <form onSubmit={onSubmit} className="space-y-6">
            <div className="space-y-4">
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <Icon name="person" size={22} className="text-outline" />
                </div>
                <input
                  className="chunky-input pl-12"
                  placeholder="Tên đăng nhập"
                  autoFocus
                  value={form.username}
                  onChange={(e) => setForm({ ...form, username: e.target.value })}
                />
              </div>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <Icon name="lock" size={22} className="text-outline" />
                </div>
                <input
                  className="chunky-input pl-12 pr-12"
                  type={showPw ? "text" : "password"}
                  placeholder="Mật khẩu"
                  value={form.password}
                  onChange={(e) => setForm({ ...form, password: e.target.value })}
                />
                <button
                  type="button"
                  onClick={() => setShowPw((v) => !v)}
                  className="absolute right-4 top-1/2 -translate-y-1/2 text-outline hover:text-on-surface"
                >
                  <Icon name={showPw ? "visibility_off" : "visibility"} size={22} />
                </button>
              </div>
            </div>

            <div className="flex items-center justify-between">
              <label className="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" className="h-5 w-5 text-primary border-surface-variant rounded-md" />
                <span className="font-body text-body-md text-on-surface-variant">Nhớ tài khoản</span>
              </label>
              <a href="#" className="font-body font-bold text-label-lg text-primary hover:underline">
                Quên mật khẩu?
              </a>
            </div>

            <button
              type="submit"
              disabled={submitting}
              className="btn-primary w-full h-16 text-headline-md font-display font-bold rounded-[24px] border-on-primary-fixed-variant"
            >
              <Icon name="login" size={22} filled />
              {submitting ? "Đang đăng nhập..." : "Đăng nhập"}
            </button>
          </form>

          <div className="text-center">
            <p className="font-body text-body-md text-on-surface-variant">
              Chưa có tài khoản?{" "}
              <Link to="/register" className="font-body font-bold text-tertiary hover:underline">
                Đăng ký ngay cho bé
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
