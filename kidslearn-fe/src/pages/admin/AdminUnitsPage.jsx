import { useEffect, useMemo, useState } from "react";
import { Pencil, Trash2 } from "lucide-react";
import toast from "react-hot-toast";
import { gradeApi } from "../../api/content";
import { unitAdminApi } from "../../api/admin";
import Modal from "../../components/Modal";
import ConfirmDialog from "../../components/ConfirmDialog";
import ImageUpload from "../../components/ImageUpload";
import {
  AdminPageHeader, PrimaryButton, SecondaryButton, DangerButton,
  AdminInput, AdminTextarea, AdminSelect, EmptyTable, LoadingRow,
} from "../../components/AdminUI";

const empty = { gradeId: 0, title: "", description: "", avatarFile: null, orderIndex: 1 };

export default function AdminUnitsPage() {
  const [grades, setGrades] = useState([]);
  const [items, setItems] = useState([]);
  const [filter, setFilter] = useState(""); // gradeId filter (string "" or number)
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(null);
  const [deleting, setDeleting] = useState(null);
  const [saving, setSaving] = useState(false);

  const load = async () => {
    setLoading(true);
    try {
      const [g, u] = await Promise.all([gradeApi.getAll(), unitAdminApi.list(filter || undefined)]);
      setGrades(g || []);
      setItems(u || []);
    } finally { setLoading(false); }
  };
  useEffect(() => { load(); /* eslint-disable-next-line */ }, [filter]);

  const gradeName = useMemo(() => {
    const m = {};
    grades.forEach((g) => (m[g.gradeId] = g.gradeName));
    return m;
  }, [grades]);

  const openCreate = () => setEditing({ ...empty, gradeId: grades[0]?.gradeId || 0, orderIndex: items.length + 1 });
  const openEdit = (u) => setEditing({ ...u });

  const save = async () => {
  if (!editing.title.trim()) return toast.error("Vui lòng nhập tiêu đề");
  if (!editing.gradeId) return toast.error("Chọn khối lớp");
  setSaving(true);
  try {
    const dto = {
      gradeId: Number(editing.gradeId),
      title: editing.title,
      description: editing.description,
      avatarFile: editing.avatarFile, // File object hoặc null
      orderIndex: Number(editing.orderIndex) || 1,
    };
    if (editing.unitId) {
      await unitAdminApi.update(editing.unitId, dto);
      toast.success("Đã cập nhật Unit");
    } else {
      await unitAdminApi.create(dto);
      toast.success("Đã tạo Unit");
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
      await unitAdminApi.delete(deleting.unitId);
      toast.success("Đã xóa Unit");
      setDeleting(null);
      load();
    } catch (e) {
      toast.error(e.userMessage || "Xóa thất bại");
    } finally { setSaving(false); }
  };

  return (
    <div>
      <AdminPageHeader
        title="Quản lý chủ đề (Unit)"
        subtitle="Mỗi chủ đề thuộc về một khối lớp và chứa từ vựng, câu hỏi Quiz."
        action={<PrimaryButton onClick={openCreate} disabled={!grades.length}>Thêm Unit</PrimaryButton>}
      />

      <div className="bg-white border border-slate-200 rounded-xl mb-4 p-4 flex items-center gap-3">
        <label className="text-sm font-semibold text-slate-600">Lọc theo khối lớp:</label>
        <select
          className="rounded-lg border border-slate-200 px-3 py-1.5 text-sm font-semibold focus:outline-none focus:ring-2 focus:ring-sky-200"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        >
          <option value="">Tất cả khối lớp</option>
          {grades.map((g) => <option key={g.gradeId} value={g.gradeId}>{g.gradeName}</option>)}
        </select>
        <div className="ml-auto text-sm text-slate-500 font-semibold">{items.length} Unit</div>
      </div>

      <div className="bg-white border border-slate-200 rounded-xl overflow-x-auto">
        <table className="w-full min-w-[800px]" >
          <thead className="bg-slate-50 text-xs uppercase text-slate-500 font-bold tracking-wider">
            <tr>
              <th className="text-left px-4 py-3">#</th>
              <th className="text-left px-4 py-3">Tiêu đề</th>
              <th className="text-left px-4 py-3">Khối lớp</th>
              <th className="text-center px-4 py-3">Từ vựng</th>
              <th className="text-center px-4 py-3">Câu hỏi</th>
              <th className="text-center px-4 py-3">Thứ tự</th>
              <th className="text-right px-4 py-3">Thao tác</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 text-sm">
            {loading ? <LoadingRow cols={7} />
              : items.length === 0 ? <EmptyTable message="Chưa có Unit nào" />
              : items.map((u) => (
                <tr key={u.unitId} className="hover:bg-slate-50">
                  <td className="px-4 py-3 text-slate-500 font-bold">#{u.unitId}</td>
                  <td className="px-4 py-3">
                    <div className="flex items-center gap-2">
                      {u.imageUrl && <img src={u.imageUrl} alt="" className="w-8 h-8 rounded object-cover bg-slate-100" />}
                      <div>
                        <div className="font-semibold text-slate-900">{u.title}</div>
                        {u.description && <div className="text-xs text-slate-500 line-clamp-1">{u.description}</div>}
                      </div>
                    </div>
                  </td>
                  <td className="px-4 py-3 text-slate-600">{gradeName[u.gradeId] || `#${u.gradeId}`}</td>
                  <td className="px-4 py-3 text-center font-mono">{u.vocabCount}</td>
                  <td className="px-4 py-3 text-center font-mono">{u.quizCount}</td>
                  <td className="px-4 py-3 text-center text-slate-600 font-mono">{u.orderIndex}</td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end gap-2">
                      <SecondaryButton icon={Pencil} onClick={() => openEdit(u)}>Sửa</SecondaryButton>
                      <DangerButton icon={Trash2} onClick={() => setDeleting(u)}>Xóa</DangerButton>
                    </div>
                  </td>
                </tr>
              ))}
          </tbody>
        </table>
      </div>

      <Modal
        open={!!editing}
        title={editing?.unitId ? "Sửa Unit" : "Thêm Unit mới"}
        onClose={() => setEditing(null)}
        size="lg"
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
            <AdminSelect
              label="Khối lớp *"
              value={editing.gradeId}
              onChange={(e) => setEditing({ ...editing, gradeId: Number(e.target.value) })}
            >
              {grades.map((g) => <option key={g.gradeId} value={g.gradeId}>{g.gradeName}</option>)}
            </AdminSelect>
            <AdminInput
              label="Tiêu đề *"
              placeholder="VD: Greetings, Family, Animals..."
              value={editing.title}
              onChange={(e) => setEditing({ ...editing, title: e.target.value })}
            />
            <AdminTextarea
              label="Mô tả"
              rows={3}
              placeholder="Mô tả ngắn nội dung Unit"
              value={editing.description || ""}
              onChange={(e) => setEditing({ ...editing, description: e.target.value })}
            />
            <div>
            <label className="block text-sm font-semibold text-slate-700 mb-1">
              Hình ảnh đại diện
            </label>
            <ImageUpload
              value={editing.avatarFile}
              currentUrl={editing.imageUrl}
              onChange={(file) => setEditing({ ...editing, avatarFile: file })}
            />
            </div>
            <AdminInput
              label="Thứ tự hiển thị trong khối"
              type="number" min={1}
              value={editing.orderIndex}
              onChange={(e) => setEditing({ ...editing, orderIndex: e.target.value })}
            />
          </div>
        )}
      </Modal>

      <ConfirmDialog
        open={!!deleting}
        title="Xóa Unit"
        message={`Xóa Unit "${deleting?.title}"? Toàn bộ từ vựng, câu hỏi và đáp án bên trong sẽ bị xóa (cascade).`}
        confirmText="Xóa"
        danger
        loading={saving}
        onConfirm={confirmDelete}
        onClose={() => setDeleting(null)}
      />
    </div>
  );
}
