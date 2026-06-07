import Icon from "./Icon";

export default function LoadingScreen({ text = "Đang tải..." }) {
  return (
    <div className="flex flex-col items-center justify-center min-h-[60vh] gap-4">
      <div className="animate-floatGentle">
        <Icon name="school" size={56} filled className="text-primary" />
      </div>
      <div className="flex items-center gap-1.5">
        <span className="w-3 h-3 rounded-full bg-primary animate-bounce" />
        <span className="w-3 h-3 rounded-full bg-secondary-container animate-bounce" style={{ animationDelay: "150ms" }} />
        <span className="w-3 h-3 rounded-full bg-tertiary-container animate-bounce" style={{ animationDelay: "300ms" }} />
      </div>
      <p className="font-body font-bold text-on-surface-variant">{text}</p>
    </div>
  );
}
