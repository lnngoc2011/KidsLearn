export const levelGradient = (level) => {
  if (level >= 8) return "from-tertiary-fixed to-secondary-fixed";
  if (level >= 5) return "from-secondary-fixed to-tertiary-fixed-dim";
  if (level >= 3) return "from-primary-fixed to-secondary-fixed";
  return "from-primary-fixed-dim to-primary-fixed";
};

export const formatDate = (iso) => {
  if (!iso) return "—";
  const d = new Date(iso);
  return d.toLocaleDateString("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
};

export const formatDateTime = (iso) => {
  if (!iso) return "—";
  const d = new Date(iso);
  return d.toLocaleString("vi-VN", { hour: "2-digit", minute: "2-digit", day: "2-digit", month: "2-digit" });
};

export const scoreColor = (score) => {
  if (score >= 90) return "text-primary";
  if (score >= 70) return "text-primary-container";
  if (score >= 50) return "text-secondary";
  return "text-tertiary";
};

export const stars = (count) => "⭐".repeat(count) + "☆".repeat(Math.max(0, 3 - count));
