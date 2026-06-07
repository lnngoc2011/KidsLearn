import { useEffect, useMemo, useState } from "react";
import { Trash2, KeyRound, Shield, Search } from "lucide-react";
import toast from "react-hot-toast";
import { userAdminApi } from "../../api/admin";
import { useAuth } from "../../context/AuthContext";
import ConfirmDialog from "../../components/ConfirmDialog";
import Modal from "../../components/Modal";
import {
  AdminPageHeader, SecondaryButton, DangerButton,
  AdminInput, AdminSelect, EmptyTable, LoadingRow,
} from "../../components/AdminUI";

export default function AdminUsersPage() {
  const { user: currentUser } = useAuth();
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [roleFilter, setRoleFilter] = useState("");
  const [deleting, setDeleting] = useState(null);
  const [resetting, setResetting] = useState(null);
  const [editingRole, setEditingRole] = useState(null);
  const [resetPassword, setResetPassword] = useState("Kid@123");
  const [saving, setSaving] = useState(false);

  const load = async () => {
    setLoading(true);
    try { setItems(await userAdminApi.list() || []); }
    finally { setLoading(false); }
  };
  useEffect(() => { load(); }, []);

  const filtered = useMemo(() => {
    return items.filter((u) => {
      if (roleFilter && u.role !== roleFilter) return false;
      if (search) {
        const q = search.toLowerCase();
        return (u.username || "").toLowerCase().includes(q) ||
               (u.fullName || "").toLowerCase().includes(q);
      }
      return true;
    });
  }, [items, search, roleFilter]);

  const doDelete = async () => {
    setSaving(true);
    try {
      await userAdminApi.delete(deleting.userId);
      toast.success("Đã xóa người dùng");
      setDeleting(null);
      load();
    } catch (e) {
      toast.error(e.userMessage || "Xóa thất bại");
    } finally { setSaving(false); }
  };

  const doResetPassword = async () => {
    if (resetPassword.length < 6) return toast.error("Mật khẩu tối thiểu 6 ký tự");
    setSaving(true);
    try {
      await userAdminApi.resetPassword(resetting.userId, resetPassword);
      toast.success(`Đã reset mật khẩu cho ${resetting.username}`);
      setResetting(null);
      setResetPassword("Kid@123");
    } catch (e) {
      toast.error(e.userMessage || "Reset thất bại");
    } finally { setSaving(false); }
  };

  const doChangeRole = async () => {
    setSaving(true);
    try {
      await userAdminApi.changeRole(editingRole.userId, editingRole.role);
      toast.success("Đã đổi vai trò");
      setEditingRole(null);
      load();
    } catch (e) {
      toast.error(e.userMessage || "Đổi vai trò thất bại");
    } finally { setSaving(false); }
  };

  const roleBadge = (role) => {
    if (role === "Admin") return <span className="px-2 py-0.5 rounded text-xs font-bold bg-violet-100 text-violet-700">Admin</span>;
    return <span className="px-2 py-0.5 rounded text-xs font-bold bg-sky-100 text-sky-700">Student</span>;
  };

  return (
    <div>
      <AdminPageHeader title="Quản lý người dùng" subtitle="Xem danh sách, đổi vai trò, reset mật khẩu, xóa tài khoản." />

      <div className="bg-white border border-slate-200 rounded-xl mb-4 p-4 grid grid-cols-1 sm:grid-cols-3 gap-3">
        <div>
          <label className="block text-sm font-semibold text-slate-700 mb-1.5">Tìm kiếm</label>
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
            <input
              className="w-full rounded-lg border border-slate-200 pl-9 pr-3 py-2 focus:outline-none focus:ring-2 focus:ring-sky-200"
              placeholder="Tên đăng nhập hoặc họ tên..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
        </div>
        <AdminSelect label="Vai trò" value={roleFilter} onChange={(e) => setRoleFilter(e.target.value)}>
          <option value="">Tất cả</option>
          <option value="Student">Student</option>
          <option value="Admin">Admin</option>
        </AdminSelect>
        <div className="flex items-end">
          <div className="text-sm text-slate-500 font-semibold">{filtered.length}/{items.length} người dùng</div>
        </div>
      </div>

      <div className="bg-white border border-slate-200 rounded-xl overflow-x-auto">
        <table className="w-full min-w-[800px]" className="w-full">
          <thead className="bg-slate-50 text-xs uppercase text-slate-500 font-bold tracking-wider">
            <tr>
              <th className="text-left px-4 py-3">#</th>
              <th className="text-left px-4 py-3">Người dùng</th>
              <th className="text-left px-4 py-3">Vai trò</th>
              <th className="text-center px-4 py-3">XP / Level</th>
              <th className="text-left px-4 py-3">Ngày tạo</th>
              <th className="text-right px-4 py-3">Thao tác</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 text-sm">
            {loading ? <LoadingRow cols={6} />
              : filtered.length === 0 ? <EmptyTable message="Không có người dùng nào khớp" />
              : filtered.map((u) => (
                <tr key={u.userId} className={`hover:bg-slate-50 ${u.userId === currentUser?.userId ? "bg-amber-50/40" : ""}`}>
                  <td className="px-4 py-3 text-slate-500 font-bold">#{u.userId}</td>
                  <td className="px-4 py-3">
                    <div className="flex items-center gap-3">
                      {u.avatarUrl ? (
                        <img src={u.avatarUrl} alt="" className="w-9 h-9 rounded-full object-cover bg-slate-100" />
                      ) : (
                        <div className="w-9 h-9 rounded-full bg-gradient-to-br from-sky-400 to-violet-400 flex items-center justify-center text-white font-bold text-sm">
                          {(u.username || "?").slice(0, 1).toUpperCase()}
                        </div>
                      )}
                      <div>
                        <div className="font-semibold text-slate-900">
                          {u.fullName || u.username}
                          {u.userId === currentUser?.userId && <span className="ml-2 text-xs text-amber-600 font-bold">(bạn)</span>}
                        </div>
                        <div className="text-xs text-slate-500">@{u.username}</div>
                      </div>
                    </div>
                  </td>
                  <td className="px-4 py-3">{roleBadge(u.role)}</td>
                  <td className="px-4 py-3 text-center">
                    <div className="text-sm font-semibold text-slate-700">{u.totalXP} XP</div>
                    <div className="text-xs text-slate-500">Lv {u.level}</div>
                  </td>
                  <td className="px-4 py-3 text-slate-500 text-xs">
                    {u.createdAt ? new Date(u.createdAt).toLocaleDateString("vi-VN") : "—"}
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end gap-2">
                      <SecondaryButton icon={Shield} onClick={() => setEditingRole({ ...u, role: u.role })}>
                        Đổi vai trò
                      </SecondaryButton>
                      <SecondaryButton icon={KeyRound} onClick={() => setResetting(u)}>
                        Reset MK
                      </SecondaryButton>
                      <DangerButton icon={Trash2} onClick={() => setDeleting(u)}>Xóa</DangerButton>
                    </div>
                  </td>
                </tr>
              ))}
          </tbody>
        </table>
      </div>

      {/* Modal change role */}
      <Modal
        open={!!editingRole}
        title="Đổi vai trò người dùng"
        onClose={() => setEditingRole(null)}
        footer={
          <>
            <button onClick={() => setEditingRole(null)} className="px-4 py-2 rounded-lg bg-slate-100 hover:bg-slate-200 font-semibold text-slate-700">Hủy</button>
            <button onClick={doChangeRole} disabled={saving} className="px-4 py-2 rounded-lg bg-sky-500 hover:bg-sky-600 text-white font-semibold disabled:opacity-50">
              {saving ? "..." : "Lưu"}
            </button>
          </>
        }
      >
        {editingRole && (
          <div className="space-y-3">
            <div className="text-sm text-slate-600">
              Đang đổi vai trò của: <span className="font-bold text-slate-900">{editingRole.fullName || editingRole.username}</span>
            </div>
            <AdminSelect
              label="Vai trò mới"
              value={editingRole.role}
              onChange={(e) => setEditingRole({ ...editingRole, role: e.target.value })}
            >
              <option value="Student">Student (Học sinh)</option>
              <option value="Admin">Admin (Quản trị viên)</option>
            </AdminSelect>
          </div>
        )}
      </Modal>

      {/* Modal reset password */}
      <Modal
        open={!!resetting}
        title="Đặt lại mật khẩu"
        onClose={() => { setResetting(null); setResetPassword("Kid@123"); }}
        footer={
          <>
            <button onClick={() => { setResetting(null); setResetPassword("Kid@123"); }} className="px-4 py-2 rounded-lg bg-slate-100 hover:bg-slate-200 font-semibold text-slate-700">Hủy</button>
            <button onClick={doResetPassword} disabled={saving} className="px-4 py-2 rounded-lg bg-sky-500 hover:bg-sky-600 text-white font-semibold disabled:opacity-50">
              {saving ? "..." : "Đặt lại"}
            </button>
          </>
        }
      >
        {resetting && (
          <div className="space-y-3">
            <div className="text-sm text-slate-600">
              Đặt lại mật khẩu cho: <span className="font-bold text-slate-900">{resetting.fullName || resetting.username}</span>
            </div>
            <AdminInput
              label="Mật khẩu mới"
              type="text"
              value={resetPassword}
              onChange={(e) => setResetPassword(e.target.value)}
            />
            <p className="text-xs text-slate-500">Mặc định: <code className="bg-slate-100 px-1.5 py-0.5 rounded">Kid@123</code></p>
          </div>
        )}
      </Modal>

      <ConfirmDialog
        open={!!deleting}
        title="Xóa người dùng"
        message={`Xóa tài khoản "${deleting?.username}"? Toàn bộ tiến độ học, huy hiệu, lịch sử quiz của user này sẽ bị xóa.`}
        confirmText="Xóa"
        danger
        loading={saving}
        onConfirm={doDelete}
        onClose={() => setDeleting(null)}
      />
    </div>
  );
}
