import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import toast from "react-hot-toast";
import { useAuth } from "../../context/AuthContext";
import Icon from "../../components/Icon";

export default function RegisterPage() {
  const { register, user } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ username: "", password: "", confirmPassword: "" });
  const [showPw, setShowPw] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  if (user) {
    navigate("/", { replace: true });
    return null;
  }

  const onSubmit = async (e) => {
    e.preventDefault();
    if (form.username.length < 3) return toast.error("Tên đăng nhập tối thiểu 3 ký tự");
    if (form.password.length < 6) return toast.error("Mật khẩu tối thiểu 6 ký tự");
    if (form.password !== form.confirmPassword) return toast.error("Mật khẩu xác nhận không khớp");
    setSubmitting(true);
    try {
      await register(form);
      toast.success("Tạo tài khoản thành công! 🎉");
      navigate("/", { replace: true });
    } catch (err) {
      toast.error(err.userMessage || "Đăng ký thất bại");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen flex bg-background">
      {/* LEFT — Brand */}
      <div className="hidden lg:flex w-1/2 relative bg-gradient-to-br from-secondary-fixed to-primary-fixed overflow-hidden">
        <div className="absolute inset-0 dotted-texture opacity-60" />
        <div className="absolute -top-20 -right-20 w-72 h-72 bg-tertiary-fixed rounded-full mix-blend-multiply filter blur-3xl opacity-50 animate-pulse" />
        <div className="absolute -bottom-20 -left-20 w-80 h-80 bg-primary-container rounded-full mix-blend-multiply filter blur-3xl opacity-50 animate-pulse" style={{ animationDelay: "1.5s" }} />

        <div className="relative z-10 flex flex-col justify-center items-center w-full p-container-margin-desktop text-center">
          <div className="mb-12 ">
            <div className="w-64 h-64 rounded-full bg-surface-container-lowest shadow-2xl border-8 border-white/50 flex items-center justify-center overflow-hidden">
              <img src="/logo.png" alt="" className="w-full h-full object-contain" />
            </div>
          </div>
          <h1 className="font-display text-display-lg text-on-primary-fixed mb-6 drop-shadow-sm">
            Bắt đầu <span className="text-tertiary">chuyến phiêu lưu</span>!
          </h1>
          <p className="font-body text-body-lg text-on-primary-fixed-variant max-w-md">
            Tạo tài khoản miễn phí để học từ vựng, chơi Quiz, và sưu tầm huy hiệu.
          </p>
        </div>
      </div>

      {/* RIGHT — Form */}
      <div className="w-full lg:w-1/2 flex flex-col justify-center items-center bg-surface-container-lowest p-8">
        <div className="w-full max-w-md space-y-8">
          <div>
            
            <h2 className="font-display text-display-lg text-on-background tracking-tight">Đăng ký</h2>
            
          </div>

          <form onSubmit={onSubmit} className="space-y-5">
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                <Icon name="person" size={22} className="text-outline" />
              </div>
              <input
                className="chunky-input pl-12"
                placeholder="Tên đăng nhập (3-50 ký tự)"
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
                placeholder="Mật khẩu (tối thiểu 6 ký tự)"
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
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                <Icon name="lock_reset" size={22} className="text-outline" />
              </div>
              <input
                className="chunky-input pl-12"
                type="password"
                placeholder="Xác nhận mật khẩu"
                value={form.confirmPassword}
                onChange={(e) => setForm({ ...form, confirmPassword: e.target.value })}
              />
            </div>

            <button
              type="submit"
              disabled={submitting}
              className="btn-primary w-full h-16 text-headline-md font-display font-bold rounded-[24px] border-on-primary-fixed-variant"
            >
              <Icon name="person_add" size={22} filled />
              {submitting ? "Đang tạo..." : "Tạo tài khoản"}
            </button>
          </form>

          <p className="text-center font-body text-body-md text-on-surface-variant">
            Đã có tài khoản?{" "}
            <Link to="/login" className="font-bold text-primary hover:underline">Đăng nhập</Link>
          </p>
        </div>
      </div>
    </div>
  );
}
