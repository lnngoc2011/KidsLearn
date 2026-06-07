import { useEffect, useMemo, useState } from "react";
import { Pencil, Trash2, Volume2 } from "lucide-react";
import toast from "react-hot-toast";
import { gradeApi } from "../../api/content";
import { unitAdminApi, vocabAdminApi } from "../../api/admin";
import { useTTS } from "../../hooks/useTTS";
import Modal from "../../components/Modal";
import ConfirmDialog from "../../components/ConfirmDialog";
import ImageUpload from "../../components/ImageUpload";
import {
  AdminPageHeader, PrimaryButton, SecondaryButton, DangerButton,
  AdminInput, AdminTextarea, AdminSelect, EmptyTable, LoadingRow,
} from "../../components/AdminUI";

const empty = { unitId: 0, word: "", mean: "", ipa: "", imageFile: null, imageUrl: "", example: "" };

export default function AdminVocabulariesPage() {
  const [grades, setGrades] = useState([]);
  const [units, setUnits] = useState([]);
  const [gradeFilter, setGradeFilter] = useState("");
  const [unitFilter, setUnitFilter] = useState("");
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(false);
  const [editing, setEditing] = useState(null);
  const [deleting, setDeleting] = useState(null);
  const [saving, setSaving] = useState(false);
  const { speak, supported } = useTTS();

  useEffect(() => {
    (async () => {
      const [g, u] = await Promise.all([gradeApi.getAll(), unitAdminApi.list()]);
      setGrades(g || []);
      setUnits(u || []);
      if (u?.length) setUnitFilter(String(u[0].unitId));
    })();
  }, []);

  const filteredUnits = useMemo(() => {
    if (!gradeFilter) return units;
    return units.filter((u) => String(u.gradeId) === String(gradeFilter));
  }, [units, gradeFilter]);

  useEffect(() => {
    if (gradeFilter && !filteredUnits.find((u) => String(u.unitId) === String(unitFilter))) {
      setUnitFilter(filteredUnits[0] ? String(filteredUnits[0].unitId) : "");
    }
    // eslint-disable-next-line
  }, [gradeFilter, filteredUnits]);

  useEffect(() => {
    if (!unitFilter) { setItems([]); return; }
    setLoading(true);
    vocabAdminApi.list(unitFilter)
      .then((v) => setItems(v || []))
      .finally(() => setLoading(false));
  }, [unitFilter]);

  const reload = () => {
    if (!unitFilter) return;
    setLoading(true);
    vocabAdminApi.list(unitFilter).then((v) => setItems(v || [])).finally(() => setLoading(false));
  };

  const openCreate = () => setEditing({ ...empty, unitId: Number(unitFilter) });
  const openEdit = (v) => setEditing({ ...v, unitId: Number(unitFilter), imageFile: null });

  const save = async () => {
    if (!editing.word.trim()) return toast.error("Vui lòng nhập từ");
    if (!editing.mean.trim()) return toast.error("Vui lòng nhập nghĩa");
    setSaving(true);
    try {
      const formData = new FormData();
      formData.append("UnitId", Number(editing.unitId));
      formData.append("Word", editing.word);
      formData.append("Mean", editing.mean);
      formData.append("Ipa", editing.ipa || "");
      formData.append("Example", editing.example || "");
      if (editing.imageFile) {
        formData.append("ImageFile", editing.imageFile);
      }

      if (editing.vocabId) {
        await vocabAdminApi.update(editing.vocabId, formData);
        toast.success("Đã cập nhật từ vựng");
      } else {
        await vocabAdminApi.create(formData);
        toast.success("Đã thêm từ vựng");
      }
      setEditing(null);
      reload();
    } catch (e) {
      toast.error(e.userMessage || "Thao tác thất bại");
    } finally { setSaving(false); }
  };

  const confirmDelete = async () => {
    setSaving(true);
    try {
      await vocabAdminApi.delete(deleting.vocabId);
      toast.success("Đã xóa từ vựng");
      setDeleting(null);
      reload();
    } catch (e) {
      toast.error(e.userMessage || "Xóa thất bại");
    } finally { setSaving(false); }
  };

  return (
    <div>
      <AdminPageHeader
        title="Quản lý từ vựng"
        subtitle="Chọn Unit để xem và quản lý từ vựng bên trong."
        action={<PrimaryButton onClick={openCreate} disabled={!unitFilter}>Thêm từ vựng</PrimaryButton>}
      />

      <div className="bg-white border border-slate-200 rounded-xl mb-4 p-4 grid grid-cols-1 sm:grid-cols-3 gap-3">
        <AdminSelect label="Khối lớp" value={gradeFilter} onChange={(e) => setGradeFilter(e.target.value)}>
          <option value="">Tất cả</option>
          {grades.map((g) => <option key={g.gradeId} value={g.gradeId}>{g.gradeName}</option>)}
        </AdminSelect>
        <AdminSelect label="Unit *" value={unitFilter} onChange={(e) => setUnitFilter(e.target.value)}>
          <option value="">— Chọn Unit —</option>
          {filteredUnits.map((u) => <option key={u.unitId} value={u.unitId}>{u.title}</option>)}
        </AdminSelect>
        <div className="flex items-end">
          <div className="text-sm text-slate-500 font-semibold">{items.length} từ vựng</div>
        </div>
      </div>

      <div className="bg-white border border-slate-200 rounded-xl overflow-x-auto">
        <table className="w-full min-w-[800px]">
          <thead className="bg-slate-50 text-xs uppercase text-slate-500 font-bold tracking-wider">
            <tr>
              <th className="text-left px-4 py-3">Từ</th>
              <th className="text-left px-4 py-3">Nghĩa</th>
              <th className="text-left px-4 py-3">IPA</th>
              <th className="text-left px-4 py-3">Hình</th>
              <th className="text-left px-4 py-3">Ví dụ</th>
              <th className="text-right px-4 py-3">Thao tác</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 text-sm">
            {!unitFilter ? <EmptyTable message="Chọn Unit để xem từ vựng" />
              : loading ? <LoadingRow cols={6} />
              : items.length === 0 ? <EmptyTable message="Chưa có từ vựng nào" />
              : items.map((v) => (
                <tr key={v.vocabId} className="hover:bg-slate-50">
                  <td className="px-4 py-3">
                    <div className="flex items-center gap-2">
                      <span className="font-bold text-slate-900">{v.word}</span>
                      {supported && (
                        <button onClick={() => speak(v.word)} className="p-1 rounded text-sky-500 hover:bg-sky-50" title="Phát âm">
                          <Volume2 className="w-4 h-4" />
                        </button>
                      )}
                    </div>
                  </td>
                  <td className="px-4 py-3 text-slate-700">{v.mean}</td>
                  <td className="px-4 py-3 text-slate-500 font-mono text-xs">{v.ipa || "—"}</td>
                  <td className="px-4 py-3">
                    {v.imageUrl
                      ? <img src={v.imageUrl} className="w-10 h-10 rounded object-cover bg-slate-100" alt="" />
                      : <span className="text-slate-300">—</span>}
                  </td>
                  <td className="px-4 py-3 text-slate-600 italic text-xs line-clamp-1">{v.example || "—"}</td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end gap-2">
                      <SecondaryButton icon={Pencil} onClick={() => openEdit(v)}>Sửa</SecondaryButton>
                      <DangerButton icon={Trash2} onClick={() => setDeleting(v)}>Xóa</DangerButton>
                    </div>
                  </td>
                </tr>
              ))}
          </tbody>
        </table>
      </div>

      <Modal
        open={!!editing}
        title={editing?.vocabId ? "Sửa từ vựng" : "Thêm từ vựng mới"}
        size="lg"
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
          <div className="space-y-3">
            <div className="grid grid-cols-2 gap-3">
              <AdminInput label="Từ tiếng Anh *" placeholder="apple" value={editing.word}
                onChange={(e) => setEditing({ ...editing, word: e.target.value })} />
              <AdminInput label="Nghĩa tiếng Việt *" placeholder="quả táo" value={editing.mean}
                onChange={(e) => setEditing({ ...editing, mean: e.target.value })} />
            </div>
            <div>
              <label className="block text-sm font-semibold text-slate-700 mb-1">Hình ảnh</label>
              <ImageUpload
                value={editing.imageFile}
                currentUrl={editing.imageUrl}
                onChange={(file) => setEditing({ ...editing, imageFile: file })}
              />
            </div>
            <AdminTextarea label="Câu ví dụ" rows={2} placeholder="An apple a day keeps the doctor away."
              value={editing.example || ""}
              onChange={(e) => setEditing({ ...editing, example: e.target.value })} />
          </div>
        )}
      </Modal>

      <ConfirmDialog
        open={!!deleting}
        title="Xóa từ vựng"
        message={`Xóa từ "${deleting?.word}"? Hành động này không thể hoàn tác.`}
        confirmText="Xóa"
        danger
        loading={saving}
        onConfirm={confirmDelete}
        onClose={() => setDeleting(null)}
      />
    </div>
  );
}