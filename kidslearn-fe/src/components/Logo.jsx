import Icon from "./Icon";

export default function Logo({ size = 32, withText = true, textClass = "" }) {
  return (
    <div className="flex items-center gap-2">
      <Icon name="school" size={size} filled className="text-secondary-container" />
      {withText && (
        <span className={`font-display font-extrabold text-primary leading-none ${textClass || "text-headline-md"}`}>
          KidsLearn
        </span>
      )}
    </div>
  );
}
