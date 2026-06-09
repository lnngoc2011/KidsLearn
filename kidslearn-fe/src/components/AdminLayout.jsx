import { Outlet, NavLink, useNavigate, Link } from "react-router-dom";
import {
  LayoutDashboard, GraduationCap, BookOpen, BookMarked, PencilLine,
  Users, LogOut, Settings, ChevronLeft
} from "lucide-react";
import Logo from "./Logo";
import { useAuth } from "../context/AuthContext";

const navItems = [
  { to: "/admin", label: "Tổng quan", icon: LayoutDashboard, end: true },
  { to: "/admin/grades", label: "Khối lớp", icon: GraduationCap },
  { to: "/admin/units", label: "Chủ đề", icon: BookOpen },
  { to: "/admin/vocabularies", label: "Từ vựng", icon: BookMarked },
  { to: "/admin/quizzes", label: "Câu hỏi Quiz", icon: PencilLine },
  { to: "/admin/users", label: "Người dùng", icon: Users },
];

export default function AdminLayout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  return (
    <div className="min-h-screen flex bg-slate-50">
      {/* Sidebar */}
      <aside className="w-64 shrink-0 bg-slate-900 text-slate-100 flex flex-col sticky top-0 h-screen">
        <div className="px-5 py-5 border-b border-slate-800">
          <Link to="/admin" className="flex items-center gap-2">
            <img src="/logo.png" className="w-9 h-9 rounded-lg object-contain bg-white p-0.5" alt="" />
            <div>
              <div className="font-display font-bold text-lg leading-tight">KidsLearn</div>
              <div className="text-[11px] text-slate-400 uppercase tracking-wider font-bold">Admin Panel</div>
            </div>
          </Link>
        </div>
        <nav className="flex-1 p-3 space-y-1">
          {navItems.map(({ to, label, icon: Icon, end }) => (
            <NavLink
              key={to}
              to={to}
              end={end}
              className={({ isActive }) =>
                `flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-semibold transition ${
                  isActive
                    ? "bg-sky-500 text-white"
                    : "text-slate-300 hover:bg-slate-800 hover:text-white"
                }`
              }
            >
              <Icon className="w-4 h-4" />
              {label}
            </NavLink>
          ))}
        </nav>
        <div className="p-3 border-t border-slate-800 space-y-1">
          
          <button
            onClick={logout}
            className="w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-semibold text-slate-300 hover:bg-rose-600 hover:text-white"
          >
            <LogOut className="w-4 h-4" /> Đăng xuất
          </button>
        </div>
      </aside>

      {/* Main */}
      <div className="flex-1 flex flex-col min-w-0">
        <header className="h-14 bg-white border-b border-slate-200 px-6 flex items-center justify-between">
          <div className="text-sm text-slate-500 font-semibold">Bảng điều khiển quản trị</div>
          <button
            onClick={() => navigate("/profile")}
            className="flex items-center gap-2 px-3 py-1.5 rounded-lg hover:bg-slate-100"
          >
            <div className="w-8 h-8 rounded-full bg-gradient-to-br from-sky-500 to-violet-500 flex items-center justify-center text-white font-bold text-xs">
              {(user?.username || "A").slice(0, 1).toUpperCase()}
            </div>
            <div className="text-left">
              <div className="text-sm font-semibold text-slate-800 leading-tight">{user?.username}</div>
              <div className="text-[11px] text-slate-500 font-bold">{user?.role}</div>
            </div>
          </button>
        </header>
        <main className="flex-1 p-6 overflow-auto">
          <div className="max-w-7xl mx-auto">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
}
