const LEVEL_THRESHOLDS = [
  { level: 1, min: 0,    max: 99,   name: "Beginner"     },
  { level: 2, min: 100,  max: 499,  name: "Explorer"     },
  { level: 3, min: 500,  max: 1999, name: "Smart Kid"    },
  { level: 4, min: 2000, max: null, name: "Vocab Master" },
];

export const getLevelInfo = (totalXP = 0) => {
  const current = LEVEL_THRESHOLDS.find(
    L => totalXP >= L.min && (L.max === null || totalXP <= L.max)
  ) ?? LEVEL_THRESHOLDS[0];

  const next = LEVEL_THRESHOLDS.find(L => L.level === current.level + 1);

  if (!next) {
    return {
      level:       current.level,
      levelName:   current.name,
      xpInLevel:   totalXP - current.min,
      xpRange:     0,
      nextLevelXP: totalXP,
      remainingXP: 0,
      progress:    100,
      isMaxLevel:  true,
    };
  }

  const xpInLevel   = totalXP - current.min;
  const xpRange     = next.min - current.min;
  const remainingXP = next.min - totalXP;
  const progress    = (xpInLevel / xpRange) * 100;

  return {
    level:       current.level,
    levelName:   current.name,
    xpInLevel,
    xpRange,
    nextLevelXP: next.min,
    remainingXP,
    progress:    Math.min(100, Math.max(0, progress)),
    isMaxLevel:  false,
  };
};


export const levelGradient = (level) => {
  if (level >= 4) return "from-tertiary-fixed to-secondary-fixed";
  if (level >= 3) return "from-secondary-fixed to-tertiary-fixed-dim";
  if (level >= 2) return "from-primary-fixed to-secondary-fixed";
  return "from-primary-fixed-dim to-primary-fixed";
};

// Date formatters 
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

//  Score color & stars
export const scoreColor = (score) => {
  if (score >= 90) return "text-primary";
  if (score >= 70) return "text-primary-container";
  if (score >= 50) return "text-secondary";
  return "text-tertiary";
};

export const stars = (count) => "⭐".repeat(count) + "☆".repeat(Math.max(0, 3 - count));