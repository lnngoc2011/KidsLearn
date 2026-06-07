import { Plus } from "lucide-react";

export function AdminPageHeader({ title, subtitle, action }) {
  return (
    <div className="flex items-start justify-between gap-4 mb-6">
      <div>
        <h1 className="font-display text-2xl font-bold text-slate-900">{title}</h1>
        {subtitle && <p className="text-sm text-slate-500 mt-1">{subtitle}</p>}
      </div>
      {action}
    </div>
  );
}

export function PrimaryButton({ onClick, children, icon: Icon = Plus, disabled }) {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      className="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-sky-500 text-white font-semibold hover:bg-sky-600 disabled:opacity-50 disabled:cursor-not-allowed"
    >
      {Icon && <Icon className="w-4 h-4" />}
      {children}
    </button>
  );
}

export function SecondaryButton({ onClick, children, icon: Icon }) {
  return (
    <button
      onClick={onClick}
      className="inline-flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm bg-slate-100 hover:bg-slate-200 text-slate-700 font-semibold"
    >
      {Icon && <Icon className="w-4 h-4" />}
      {children}
    </button>
  );
}

export function DangerButton({ onClick, children, icon: Icon }) {
  return (
    <button
      onClick={onClick}
      className="inline-flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm bg-rose-50 hover:bg-rose-100 text-rose-600 font-semibold"
    >
      {Icon && <Icon className="w-4 h-4" />}
      {children}
    </button>
  );
}

export function AdminInput({ label, error, ...props }) {
  return (
    <div>
      {label && <label className="block text-sm font-semibold text-slate-700 mb-1.5">{label}</label>}
      <input
        {...props}
        className={`w-full rounded-lg border px-3 py-2 outline-none focus:ring-2 focus:ring-sky-200 focus:border-sky-400 ${
          error ? "border-rose-300" : "border-slate-200"
        }`}
      />
      {error && <p className="text-xs text-rose-600 mt-1">{error}</p>}
    </div>
  );
}

export function AdminTextarea({ label, error, ...props }) {
  return (
    <div>
      {label && <label className="block text-sm font-semibold text-slate-700 mb-1.5">{label}</label>}
      <textarea
        {...props}
        className={`w-full rounded-lg border px-3 py-2 outline-none focus:ring-2 focus:ring-sky-200 focus:border-sky-400 ${
          error ? "border-rose-300" : "border-slate-200"
        }`}
      />
      {error && <p className="text-xs text-rose-600 mt-1">{error}</p>}
    </div>
  );
}

export function AdminSelect({ label, children, error, ...props }) {
  return (
    <div>
      {label && <label className="block text-sm font-semibold text-slate-700 mb-1.5">{label}</label>}
      <select
        {...props}
        className={`w-full rounded-lg border px-3 py-2 outline-none focus:ring-2 focus:ring-sky-200 focus:border-sky-400 bg-white ${
          error ? "border-rose-300" : "border-slate-200"
        }`}
      >
        {children}
      </select>
      {error && <p className="text-xs text-rose-600 mt-1">{error}</p>}
    </div>
  );
}

export function EmptyTable({ message = "Chưa có dữ liệu" }) {
  return (
    <tr>
      <td colSpan={99} className="text-center py-10 text-slate-400 font-semibold">
        {message}
      </td>
    </tr>
  );
}

export function LoadingRow({ cols = 4 }) {
  return (
    <tr>
      <td colSpan={cols} className="text-center py-10 text-slate-400 font-semibold">
        Đang tải...
      </td>
    </tr>
  );
}
