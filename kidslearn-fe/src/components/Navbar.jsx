import { Link, NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import Icon from "./Icon";
import { useState } from "react";
import { unitApi } from "../api/content";


const navItems = [
  { to: "/", label: "Trang chính", end: true },
  { to: "/grades", label: "Học", end: false },
  { to: "/progress", label: "Tiến độ", end: false },
  { to: "/badges", label: "Huy hiệu", end: false },
];

export default function Navbar() {
  const { user, profile, logout } = useAuth();
  const navigate = useNavigate();
  const [keyword, setKeyword] = useState("");
const [results, setResults] = useState([]);
const handleSearch = async (e) => {
  const value = e.target.value;
  setKeyword(value);

  if (value.trim().length < 2) {
    setResults([]);
    return;
  }

  try {
    const data = await unitApi.search(value);
    setResults(data);
  } catch {
    setResults([]);
  }
};
  return (
    <header className="sticky top-0 z-40 h-20 bg-background/95 backdrop-blur border-b-4 border-surface-variant shadow-sm">
      <div className="h-full max-w-[1440px] mx-auto px-container-margin-mobile md:px-container-margin-desktop flex items-center justify-between gap-4">
        {/* Brand */}
        <Link to="/" className="flex items-center gap-3 shrink-0">
          <img
            src="/logo.png"
            alt="KidsLearn Logo"
            className="w-24 h-24 object-contain"
            onError={(e) => { e.target.style.display = 'none'; }}
          />
          <span className="font-display font-extrabold text-headline-md text-primary leading-none">KidsLearn</span>
        </Link>

        {/* Search */}
        <div className="hidden lg:flex flex-1 max-w-md mx-8 relative">
          <div className="flex items-center bg-surface-container-highest rounded-full px-4 py-2 w-full border-2 border-transparent focus-within:border-primary-container transition-colors">
            <Icon name="search" size={20} className="text-outline mr-2" />
            <input
  type="text"
  value={keyword}
  onChange={handleSearch}
  placeholder="Tìm kiếm bài học..."
  className="bg-transparent border-none outline-none w-full font-body text-body-md text-on-surface placeholder-outline"
/>
{results.length > 0 && (
  <div className="absolute top-full left-0 right-0 mt-2 bg-white rounded-xl shadow-lg border z-50">
    {results.map((u) => (
      <button
        key={u.unitId}
        onClick={() => {
          navigate(`/units/${u.unitId}/learn`);
          setResults([]);
          setKeyword("");
        }}
        className="w-full text-left px-4 py-3 hover:bg-slate-100"
      >
        {u.title}
      </button>
    ))}
  </div>
)}
          </div>
        </div>

        {/* Nav links */}
        <nav className="hidden md:flex items-center gap-8">
          {navItems.map(({ to, label, end }) => (
            <NavLink
              key={to}
              to={to}
              end={end}
              className={({ isActive }) =>
                `font-body font-bold transition-all pb-2 ${
                  isActive
                    ? "text-primary border-b-4 border-primary"
                    : "text-on-surface-variant hover:text-primary"
                }`
              }
            >
              {label}
            </NavLink>
          ))}
        </nav>

        {/* Right cluster */}
        <div className="flex items-center gap-2">

          {/* Avatar */}
          <button onClick={() => navigate("/profile")} className="w-10 h-10 rounded-full border-2 border-primary-container overflow-hidden ml-2 hover:scale-105 transition-transform">
            {profile?.avatarUrl ? (
              <img src={profile.avatarUrl} alt="" className="w-full h-full object-cover" />
            ) : (
              <div className="w-full h-full bg-secondary-fixed flex items-center justify-center font-display font-extrabold text-on-secondary-container">
                {(profile?.fullName || user?.username || "U").slice(0, 1).toUpperCase()}
              </div>
            )}
          </button>

          {/* Logout */}
          <button onClick={logout} className="w-10 h-10 rounded-full flex items-center justify-center text-on-surface-variant hover:bg-error-container hover:text-on-error-container transition-colors" title="Đăng xuất">
            <Icon name="logout" size={20} />
          </button>
        </div>
      </div>

      {/* Mobile nav */}
      <nav className="md:hidden absolute bottom-0 left-0 right-0 bg-surface-container-lowest border-t-2 border-surface-variant">
        <div className="max-w-[1440px] mx-auto px-4 py-1 flex items-center justify-between">
          {navItems.map(({ to, label, end }) => {
            const iconMap = { "/": "home", "/grades": "menu_book", "/progress": "trending_up", "/badges": "military_tech" };
            return (
              <NavLink
                key={to}
                to={to}
                end={end}
                className={({ isActive }) =>
                  `flex-1 flex flex-col items-center gap-0.5 py-2 rounded-lg ${
                    isActive ? "text-primary" : "text-on-surface-variant"
                  }`
                }
              >
                <Icon name={iconMap[to]} size={22} filled />
                <span className="text-[11px] font-bold">{label}</span>
              </NavLink>
            );
          })}
        </div>
      </nav>
    </header>
  );
}