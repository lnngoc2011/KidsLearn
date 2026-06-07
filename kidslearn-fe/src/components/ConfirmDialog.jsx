import { AlertTriangle } from "lucide-react";
import Modal from "./Modal";

export default function ConfirmDialog({ open, title = "Xác nhận", message, onConfirm, onClose, confirmText = "Xác nhận", danger = false, loading = false }) {
  return (
    <Modal
      open={open}
      title={title}
      onClose={onClose}
      footer={
        <>
          <button onClick={onClose} className="px-4 py-2 rounded-lg bg-slate-100 hover:bg-slate-200 font-semibold text-slate-700">
            Hủy
          </button>
          <button
            disabled={loading}
            onClick={onConfirm}
            className={`px-4 py-2 rounded-lg font-semibold text-white disabled:opacity-50 ${
              danger ? "bg-rose-500 hover:bg-rose-600" : "bg-sky-500 hover:bg-sky-600"
            }`}
          >
            {loading ? "..." : confirmText}
          </button>
        </>
      }
    >
      <div className="flex gap-3">
        {danger && (
          <div className="shrink-0 w-10 h-10 rounded-full bg-rose-100 flex items-center justify-center">
            <AlertTriangle className="w-5 h-5 text-rose-600" />
          </div>
        )}
        <p className="text-slate-700">{message}</p>
      </div>
    </Modal>
  );
}
