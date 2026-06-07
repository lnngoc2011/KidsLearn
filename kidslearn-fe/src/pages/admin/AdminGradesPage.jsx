import { useEffect, useState } from "react";
import { Pencil, Trash2, BookOpen } from "lucide-react";
import toast from "react-hot-toast";
import { gradeApi } from "../../api/content";
import { gradeAdminApi } from "../../api/admin";
import Modal from "../../components/Modal";
import ConfirmDialog from "../../components/ConfirmDialog";
import {
  AdminPageHeader, PrimaryButton, SecondaryButton, DangerButton,
  AdminInput, EmptyTable, LoadingRow,
} from "../../components/AdminUI";

const empty = { gradeName: "", description: "", orderIndex: 1 };

export default function AdminGradesPage() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(null); // null | { gradeId?, ...form }
  const [deleting, setDeleting] = useState(null);
  const [saving, setSaving] = useState(false);

  const load = async () => {
    setLoading(true);
    try { setItems(await gradeApi.getAll() || []); }
    finally { setLoading(false); }
  };
  useEffect(() => { load(); }, []);

  const openCreate = () => setEditing({ ...empty, orderIndex: (items.length + 1) });
  const openEdit = (g) => setEditing({ gradeId: g.gradeId, gradeName: g.gradeName, description: g.description || "", orderIndex: g.orderIndex });

  const save = async () => {
    if (!editing.gradeName.trim()) return toast.error("Vui lòng nhập tên khối lớp");
    setSaving(true);
    try {
      const dto = { gradeName: editing.gradeName, description: editing.description, orderIndex: Number(editing.orderIndex) || 1 };
      if (editing.gradeId) {
        await gradeAdminApi.update(editing.gradeId, dto);
        toast.success("Đã cập nhật khối lớp");
      } else {
        await gradeAdminApi.create(dto);
        toast.success("Đã tạo khối lớp");
      }
      setEditing(null);
      load();
    } catch (e) {
      toast.error(e.userMessage || "Thao tác thất bại");
    } finally { setSaving(false); }
  };

  const confirmDelete = async () => {
    setSaving(true);
    try {
      await gradeAdminApi.delete(deleting.gradeId);
      toast.success("Đã xóa khối lớp");
      setDeleting(null);
      load();
    } catch (e) {
      toast.error(e.userMessage || "Xóa thất bại");
    } finally { setSaving(false); }
  };

  return (
    <div>
      <AdminPageHeader
        title="Quản lý khối lớp"
        subtitle="Thêm, sửa hoặc xóa các khối lớp (Grade) trong hệ thống."
        action={<PrimaryButton onClick={openCreate}>Thêm khối lớp</PrimaryButton>}
      />

      <div className="bg-white border border-slate-200 rounded-xl overflow-x-auto">
        <table className="w-full min-w-[800px]" >
          <thead className="bg-slate-50 text-xs uppercase text-slate-500 font-bold tracking-wider">
            <tr>
              <th className="text-left px-4 py-3">#</th>
              <th className="text-left px-4 py-3">Tên khối lớp</th>
              <th className="text-left px-4 py-3">Mô tả</th>
              <th className="text-center px-4 py-3">Số Unit</th>
              <th className="text-center px-4 py-3">Thứ tự</th>
              <th className="text-right px-4 py-3">Thao tác</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 text-sm">
            {loading ? <LoadingRow cols={6} />
              : items.length === 0 ? <EmptyTable message="Chưa có khối lớp nào" />
              : items.map((g) => (
                <tr key={g.gradeId} className="hover:bg-slate-50">
                  <td className="px-4 py-3 text-slate-500 font-bold">#{g.gradeId}</td>
                  <td className="px-4 py-3 font-semibold text-slate-900">{g.gradeName}</td>
                  <td className="px-4 py-3 text-slate-600">{g.description || "—"}</td>
                  <td className="px-4 py-3 text-center">
                    <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full bg-sky-50 text-sky-700 font-semibold text-xs">
                      <BookOpen className="w-3 h-3" /> {g.unitCount}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-center text-slate-600 font-mono">{g.orderIndex}</td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end gap-2">
                      <SecondaryButton icon={Pencil} onClick={() => openEdit(g)}>Sửa</SecondaryButton>
                      <DangerButton icon={Trash2} onClick={() => setDeleting(g)}>Xóa</DangerButton>
                    </div>
                  </td>
                </tr>
              ))}
          </tbody>
        </table>
      </div>

      <Modal
        open={!!editing}
        title={editing?.gradeId ? "Sửa khối lớp" : "Thêm khối lớp mới"}
        onClose={() => setEditing(null)}
        footer={
          <>
            <button onClick={() => setEditing(null)} className="px-4 py-2 rounded-lg bg-slate-100 hover:bg-slate-200 font-semibold text-slate-700">Hủy</button>
            <button onClick={save} disabled={saving} className="px-4 py-2 rounded-lg bg-sky-500 hover:bg-sky-600 text-white font-semibold disabled:opacity-50">
              {saving ? "Đang lưu..." : "Lưu"}
            </button>
          </>
        }
      >
        {editing && (
          <div className="space-y-4">
            <AdminInput
              label="Tên khối lớp *"
              placeholder="VD: Lớp 1, Lớp 2..."
              value={editing.gradeName}
              onChange={(e) => setEditing({ ...editing, gradeName: e.target.value })}
            />
            <AdminInput
              label="Mô tả"
              placeholder="Mô tả ngắn về khối lớp này"
              value={editing.description}
              onChange={(e) => setEditing({ ...editing, description: e.target.value })}
            />
            <AdminInput
              label="Thứ tự hiển thị"
              type="number"
              min={1}
              value={editing.orderIndex}
              onChange={(e) => setEditing({ ...editing, orderIndex: e.target.value })}
            />
          </div>
        )}
      </Modal>

      <ConfirmDialog
        open={!!deleting}
        title="Xóa khối lớp"
        message={`Bạn chắc chắn muốn xóa "${deleting?.gradeName}"? Hệ thống sẽ xóa TẤT CẢ Unit, từ vựng và câu hỏi bên trong (cascade). Hành động này không thể hoàn tác.`}
        confirmText="Xóa"
        danger
        loading={saving}
        onConfirm={confirmDelete}
        onClose={() => setDeleting(null)}
      />
    </div>
  );
}
