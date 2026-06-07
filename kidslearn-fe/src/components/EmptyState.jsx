import Icon from "./Icon";

export default function EmptyState({ icon = "auto_awesome", title, hint, action }) {
  return (
    <div className="card flex flex-col items-center text-center py-10">
      <div className="w-16 h-16 rounded-full bg-secondary-fixed flex items-center justify-center mb-3">
        <Icon name={icon} size={32} className="text-on-secondary-container" filled />
      </div>
      <h3 className="font-display text-headline-md text-on-surface">{title}</h3>
      {hint && <p className="text-on-surface-variant mt-1 max-w-md">{hint}</p>}
      {action && <div className="mt-4">{action}</div>}
    </div>
  );
}
