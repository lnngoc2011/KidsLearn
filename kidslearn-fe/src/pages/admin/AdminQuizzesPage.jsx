import { useEffect, useMemo, useState } from "react";
import { Pencil, Trash2, Plus, CheckCircle2, Circle, X } from "lucide-react";
import toast from "react-hot-toast";
import { gradeApi } from "../../api/content";
import { unitAdminApi, quizAdminApi } from "../../api/admin";
import Modal from "../../components/Modal";
import ConfirmDialog from "../../components/ConfirmDialog";
import ImageUpload from "../../components/ImageUpload";
import {
  AdminPageHeader, PrimaryButton, SecondaryButton, DangerButton,
  AdminInput, AdminSelect, EmptyTable, LoadingRow,
} from "../../components/AdminUI";

const emptyAnswer = () => ({
  answerText: "",
  answerType: "text",
  imageUrl: "",
  imageFile: null,
  isCorrect: false
});
const empty = {
  unitId: 0, questionText: "", questionType: "text", imageUrl: "", imageFile: null, ttsText: "",
  answers: [{ ...emptyAnswer(), isCorrect: true }, emptyAnswer(), emptyAnswer(), emptyAnswer()],
};

export default function AdminQuizzesPage() {
  const [grades, setGrades] = useState([]);
  const [units, setUnits] = useState([]);
  const [gradeFilter, setGradeFilter] = useState("");
  const [unitFilter, setUnitFilter] = useState("");
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(false);
  const [editing, setEditing] = useState(null);
  const [deleting, setDeleting] = useState(null);
  const [saving, setSaving] = useState(false);

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
    quizAdminApi.list(unitFilter).then((q) => setItems(q || [])).finally(() => setLoading(false));
  }, [unitFilter]);

  const reload = () => {
    if (!unitFilter) return;
    setLoading(true);
    quizAdminApi.list(unitFilter).then((q) => setItems(q || [])).finally(() => setLoading(false));
  };

  const openCreate = () => setEditing({ ...empty, unitId: Number(unitFilter) });
  const openEdit = (q) => {
    setEditing({
      ...q,
      unitId: Number(unitFilter),
      imageFile: null,
      answers: (q.answers || []).map((a, i) => ({
  answerId: a.answerId,
  answerText: a.answerText,
  answerType: a.answerType || "text",
  imageUrl: a.imageUrl || "",
  imageFile: null,
  isCorrect: a.isCorrect ?? (i === 0),
})),
    });
  };

  const setAnswerCorrect = (idx) => {
    setEditing((prev) => ({
      ...prev,
      answers: prev.answers.map((a, i) => ({ ...a, isCorrect: i === idx })),
    }));
  };

  const addAnswer = () => {
    if (editing.answers.length >= 6) return toast.error("Tối đa 6 đáp án");
    setEditing({ ...editing, answers: [...editing.answers, emptyAnswer()] });
  };

  const removeAnswer = (idx) => {
    if (editing.answers.length <= 2) return toast.error("Tối thiểu 2 đáp án");
    const next = editing.answers.filter((_, i) => i !== idx);
    if (!next.some((a) => a.isCorrect)) next[0].isCorrect = true;
    setEditing({ ...editing, answers: next });
  };

  const save = async () => {
    if (!editing.questionText.trim()) return toast.error("Vui lòng nhập nội dung câu hỏi");
    if (editing.answers.length < 2) return toast.error("Tối thiểu 2 đáp án");
    const correctCount = editing.answers.filter((a) => a.isCorrect).length;
    if (correctCount !== 1) return toast.error("Phải có ĐÚNG 1 đáp án đúng");
    if (
  editing.answers.some(
    (a) =>
      (a.answerType === "text" && !a.answerText.trim()) ||
      (a.answerType === "image" && !a.imageFile && !a.imageUrl)
  )
) {
  return toast.error("Tất cả đáp án phải có nội dung");
}
    if (editing.questionType === "audio" && !editing.ttsText.trim()) return toast.error("Câu hỏi Audio cần nhập TTS Text");

    setSaving(true);
    try {
      const formData = new FormData();
      formData.append("UnitId", Number(editing.unitId));
      formData.append("QuestionText", editing.questionText);
      formData.append("QuestionType", editing.questionType);
      formData.append("TtsText", editing.ttsText || "");
      if (editing.imageFile) {
        formData.append("ImageFile", editing.imageFile);
      }
      // Answers as JSON (backend parse từ string), file ảnh đáp án gửi riêng theo index
      editing.answers.forEach((a, idx) => {
        formData.append(`Answers[${idx}].AnswerText`, a.answerText);

        formData.append(
  `Answers[${idx}].AnswerType`,
  a.answerType
);

        formData.append(
          `Answers[${idx}].IsCorrect`,
          a.isCorrect ? "true" : "false"
        );
        if (a.answerId) formData.append(`Answers[${idx}].AnswerId`, a.answerId);
        if (a.imageFile) {
          formData.append(`Answers[${idx}].ImageFile`, a.imageFile);
        }
      });

      if (editing.quizId) {
        await quizAdminApi.update(editing.quizId, formData);
        toast.success("Đã cập nhật câu hỏi");
      } else {
        await quizAdminApi.create(formData);
        toast.success("Đã thêm câu hỏi");
      }
      setEditing(null);
      reload();
    } catch (e) {
  console.log("QUIZ ERROR");
  console.log(e.response?.data);

  toast.error(
    e.response?.data?.message ||
    e.userMessage ||
    "Thao tác thất bại"
  );
}
     finally { setSaving(false); }
  };

  const confirmDelete = async () => {
    setSaving(true);
    try {
      await quizAdminApi.delete(deleting.quizId);
      toast.success("Đã xóa câu hỏi");
      setDeleting(null);
      reload();
    } catch (e) {
      toast.error(e.userMessage || "Xóa thất bại");
    } finally { setSaving(false); }
  };

  const typeBadge = (t) => {
    const map = { text: "bg-slate-100 text-slate-700", image: "bg-violet-100 text-violet-700", audio: "bg-amber-100 text-amber-700" };
    return <span className={`px-2 py-0.5 rounded text-xs font-bold ${map[t] || map.text}`}>{t}</span>;
  };

  return (
    <div>
      <AdminPageHeader
        title="Quản lý câu hỏi Quiz"
        subtitle="Quản lý câu hỏi và đáp án. Mỗi câu hỏi phải có đúng 1 đáp án đúng."
        action={<PrimaryButton onClick={openCreate} disabled={!unitFilter}>Thêm câu hỏi</PrimaryButton>}
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
          <div className="text-sm text-slate-500 font-semibold">{items.length} câu hỏi</div>
        </div>
      </div>

      <div className="bg-white border border-slate-200 rounded-xl overflow-x-auto">
        <table className="w-full min-w-[800px]">
          <thead className="bg-slate-50 text-xs uppercase text-slate-500 font-bold tracking-wider">
            <tr>
              <th className="text-left px-4 py-3">#</th>
              <th className="text-left px-4 py-3">Nội dung câu hỏi</th>
              <th className="text-left px-4 py-3">Loại</th>
              <th className="text-center px-4 py-3">Số đáp án</th>
              <th className="text-right px-4 py-3">Thao tác</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 text-sm">
            {!unitFilter ? <EmptyTable message="Chọn Unit để xem câu hỏi" />
              : loading ? <LoadingRow cols={5} />
              : items.length === 0 ? <EmptyTable message="Chưa có câu hỏi nào" />
              : items.map((q, i) => (
                <tr key={q.quizId} className="hover:bg-slate-50">
                  <td className="px-4 py-3 text-slate-500 font-bold">#{i + 1}</td>
                  <td className="px-4 py-3 font-semibold text-slate-900">{q.questionText}</td>
                  <td className="px-4 py-3">{typeBadge(q.questionType)}</td>
                  <td className="px-4 py-3 text-center font-mono">{q.answers?.length || 0}</td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end gap-2">
                      <SecondaryButton icon={Pencil} onClick={() => openEdit(q)}>Sửa</SecondaryButton>
                      <DangerButton icon={Trash2} onClick={() => setDeleting(q)}>Xóa</DangerButton>
                    </div>
                  </td>
                </tr>
              ))}
          </tbody>
        </table>
      </div>

      <Modal
        open={!!editing}
        title={editing?.quizId ? "Sửa câu hỏi" : "Thêm câu hỏi mới"}
        size="xl"
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
            <div className="grid grid-cols-3 gap-3">
              <div className="col-span-2">
                <AdminInput label="Nội dung câu hỏi *" placeholder='VD: "Apple" nghĩa là gì?'
                  value={editing.questionText}
                  onChange={(e) => setEditing({ ...editing, questionText: e.target.value })} />
              </div>
              <AdminSelect label="Loại câu hỏi"
                value={editing.questionType}
                onChange={(e) => setEditing({ ...editing, questionType: e.target.value })}>
                <option value="text">Text (chữ)</option>
                <option value="image">Image (chọn ảnh)</option>
                <option value="audio">Audio (nghe)</option>
              </AdminSelect>
            </div>

            {editing.questionType === "image" && (
  <ImageUpload
    value={editing.imageFile}
    currentUrl={editing.imageUrl}
    onChange={(file) => setEditing({ ...editing, imageFile: file })}
  />
)}

            {editing.questionType === "audio" && (
              <AdminInput label="Văn bản TTS (để đọc) *" placeholder="Cat"
                value={editing.ttsText || ""}
                onChange={(e) => setEditing({ ...editing, ttsText: e.target.value })} />
            )}

            {/* Answers */}
            <div>
              <div className="flex items-center justify-between mb-2">
                <label className="text-sm font-semibold text-slate-700">Đáp án (chọn 1 đáp án đúng)</label>
                <button type="button" onClick={addAnswer} className="text-sky-600 text-sm font-semibold hover:text-sky-700 inline-flex items-center gap-1">
                  <Plus className="w-4 h-4" /> Thêm đáp án
                </button>
              </div>
              <div className="space-y-2">
                {editing.answers.map((a, idx) => (
                  <div key={idx} className={`rounded-lg border-2 p-3 ${a.isCorrect ? "border-emerald-300 bg-emerald-50" : "border-slate-200 bg-white"}`}>
                    <div className="flex items-center gap-3">
                      <button type="button" onClick={() => setAnswerCorrect(idx)} className="shrink-0" title="Đặt làm đáp án đúng">
                        {a.isCorrect ? <CheckCircle2 className="w-6 h-6 text-emerald-500" /> : <Circle className="w-6 h-6 text-slate-300 hover:text-slate-500" />}
                      </button>
                      <span className="w-6 h-6 rounded bg-slate-100 text-slate-600 text-xs font-bold flex items-center justify-center">
                                                {String.fromCharCode(65 + idx)}
                      </span>
                        <select
  value={a.answerType}
  onChange={(e) => {
    const next = [...editing.answers];
    next[idx] = {
      ...next[idx],
      answerType: e.target.value
    };
    setEditing({ ...editing, answers: next });
  }}
  className="rounded-lg border border-slate-200 px-2 py-1 text-sm"
>
  <option value="text">Text</option>
  <option value="image">Image</option>
</select>

                      {a.answerType === "text" ? (
  <input
    className="flex-1 rounded-lg border border-slate-200 px-3 py-1.5 text-sm"
    value={a.answerText}
    placeholder={`Đáp án ${String.fromCharCode(65 + idx)}`}
    onChange={(e) => {
      const next = [...editing.answers];
      next[idx] = {
        ...next[idx],
        answerText: e.target.value
      };
      setEditing({ ...editing, answers: next });
    }}
  />
) : (
  <div className="flex-1 text-sm text-slate-500">
    Đáp án bằng hình ảnh
  </div>
)}
                        
                      <button type="button" onClick={() => removeAnswer(idx)} className="shrink-0 p-1.5 rounded text-rose-500 hover:bg-rose-50" title="Xóa">
                        <X className="w-4 h-4" />
                      </button>
                    </div>
                    {a.answerType === "image" && (
   <ImageUpload
      value={a.imageFile}
      currentUrl={a.imageUrl}
      onChange={(file) => {
         const next = [...editing.answers];
         next[idx] = {
            ...next[idx],
            imageFile: file
         };
         setEditing({ ...editing, answers: next });
      }}
   />
)}
                  </div>
                ))}
              </div>
              <p className="text-xs text-slate-500 mt-2 font-semibold">
                Tick dấu ✓ xanh để đánh dấu đáp án đúng. Mỗi câu hỏi phải có đúng 1 đáp án đúng.
              </p>
            </div>
          </div>
        )}
      </Modal>

      <ConfirmDialog
        open={!!deleting}
        title="Xóa câu hỏi"
        message={`Xóa câu hỏi "${deleting?.questionText}"? Các đáp án sẽ bị xóa cùng.`}
        confirmText="Xóa"
        danger
        loading={saving}
        onConfirm={confirmDelete}
        onClose={() => setDeleting(null)}
      />
    </div>
  );
}