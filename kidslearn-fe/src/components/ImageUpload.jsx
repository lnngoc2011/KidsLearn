import { useRef, useState, useEffect } from "react";
import { Upload, X } from "lucide-react";
import toast from "react-hot-toast";

export default function ImageUpload({ value, onChange, currentUrl }) {
  const inputRef = useRef(null);
  const [preview, setPreview] = useState(currentUrl || "");
  const [dragOver, setDragOver] = useState(false);

  // Tạo preview khi có file mới
  useEffect(() => {
    if (value instanceof File) {
      const url = URL.createObjectURL(value);
      setPreview(url);
      return () => URL.revokeObjectURL(url);
    }
  }, [value]);

  // Khi mở modal edit, hiển thị ảnh hiện tại từ DB
  useEffect(() => {
    if (currentUrl && !value) setPreview(currentUrl);
  }, [currentUrl, value]);

  const handleFile = (file) => {
    if (!file) return;
    const allowed = ["image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif"];
    if (!allowed.includes(file.type)) return toast.error("Chỉ chấp nhận JPG, PNG, WEBP, GIF");
    if (file.size > 5 * 1024 * 1024) return toast.error("File quá lớn (tối đa 5MB)");
    onChange(file);
  };

  const remove = () => {
    setPreview("");
    onChange(null);
  };

  if (preview) {
    return (
      <div className="relative inline-block">
        <img src={preview} alt="preview" className="w-40 h-40 object-cover rounded-lg border-2 border-slate-200" />
        <button
          type="button"
          onClick={remove}
          className="absolute -top-2 -right-2 bg-red-500 hover:bg-red-600 text-white rounded-full p-1.5 shadow-md"
        >
          <X size={14} />
        </button>
      </div>
    );
  }

  return (
    <div
      onClick={() => inputRef.current?.click()}
      onDragOver={(e) => { e.preventDefault(); setDragOver(true); }}
      onDragLeave={() => setDragOver(false)}
      onDrop={(e) => { e.preventDefault(); setDragOver(false); handleFile(e.dataTransfer.files?.[0]); }}
      className={`border-2 border-dashed rounded-lg p-6 text-center cursor-pointer transition-colors
        ${dragOver ? "border-sky-500 bg-sky-50" : "border-slate-300 hover:border-sky-400 hover:bg-slate-50"}`}
    >
      <input
        ref={inputRef}
        type="file"
        accept="image/jpeg,image/jpg,image/png,image/webp,image/gif"
        className="hidden"
        onChange={(e) => handleFile(e.target.files?.[0])}
      />
      <div className="flex flex-col items-center gap-2 text-slate-500">
        <Upload size={32} className="text-slate-400" />
        <p className="text-sm font-semibold">Kéo thả ảnh vào đây hoặc click để chọn</p>
        <p className="text-xs text-slate-400">JPG, PNG, WEBP, GIF — tối đa 5MB</p>
      </div>
    </div>
  );
}