import { useNavigate } from "react-router-dom";
import Icon from "./Icon";

export default function PageHeader({ title, subtitle, back, right }) {
  const navigate = useNavigate();
  return (
    <div className="flex items-start gap-3 mb-6">
      {back !== false && (
        <button
          onClick={() => navigate(back || -1)}
          className="w-12 h-12 shrink-0 rounded-full bg-surface-container-lowest border-2 border-outline-variant
                     hover:bg-surface-container-high transition-colors flex items-center justify-center
                     border-b-4 border-b-outline"
          aria-label="Quay lại"
        >
          <Icon name="arrow_back" size={22} className="text-on-surface-variant" />
        </button>
      )}
      <div className="flex-1 min-w-0">
        <h1 className="font-display text-headline-lg text-on-surface leading-tight">{title}</h1>
        {subtitle && <p className="text-on-surface-variant mt-1 font-body">{subtitle}</p>}
      </div>
      {right}
    </div>
  );
}
