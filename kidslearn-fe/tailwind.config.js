/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,jsx}"],
  theme: {
    extend: {
      // ── Material Design 3 colors (from Stitch designs) ──
      colors: {
        background: "#f7f9ff",
        surface: "#f7f9ff",
        "surface-bright": "#f7f9ff",
        "surface-dim": "#d7dae1",
        "surface-variant": "#dfe3ea",
        "surface-tint": "#00639d",
        "surface-container-lowest": "#ffffff",
        "surface-container-low": "#f0f4fb",
        "surface-container": "#ebeef5",
        "surface-container-high": "#e5e8ef",
        "surface-container-highest": "#dfe3ea",

        primary: "#00639d",
        "primary-container": "#3da9fc",
        "primary-fixed": "#cfe5ff",
        "primary-fixed-dim": "#98cbff",
        "inverse-primary": "#98cbff",

        secondary: "#745c00",
        "secondary-container": "#fcd03d",
        "secondary-fixed": "#ffe089",
        "secondary-fixed-dim": "#edc22e",

        tertiary: "#ae2f34",
        "tertiary-container": "#ff7977",
        "tertiary-fixed": "#ffdad8",
        "tertiary-fixed-dim": "#ffb3b0",

        error: "#ba1a1a",
        "error-container": "#ffdad6",

        outline: "#6f7883",
        "outline-variant": "#bfc7d3",

        "on-primary": "#ffffff",
        "on-primary-container": "#003c62",
        "on-primary-fixed": "#001d33",
        "on-primary-fixed-variant": "#004a77",
        "on-secondary": "#ffffff",
        "on-secondary-container": "#705900",
        "on-secondary-fixed": "#241a00",
        "on-secondary-fixed-variant": "#574500",
        "on-tertiary": "#ffffff",
        "on-tertiary-container": "#780114",
        "on-tertiary-fixed": "#410006",
        "on-tertiary-fixed-variant": "#8c1520",
        "on-error": "#ffffff",
        "on-error-container": "#93000a",
        "on-background": "#181c21",
        "on-surface": "#181c21",
        "on-surface-variant": "#3f4851",
        "inverse-surface": "#2c3136",
        "inverse-on-surface": "#eef1f8",
      },
      borderRadius: {
        DEFAULT: "1rem",
        lg: "2rem",
        xl: "3rem",
        full: "9999px",
      },
      spacing: {
        unit: "8px",
        gutter: "24px",
        "section-gap": "64px",
        "container-margin-mobile": "16px",
        "container-margin-desktop": "48px",
      },
      fontFamily: {
        display: ['"Plus Jakarta Sans"', "system-ui", "sans-serif"],
        headline: ['"Plus Jakarta Sans"', "system-ui", "sans-serif"],
        body: ['"Nunito Sans"', "system-ui", "sans-serif"],
        label: ['"Nunito Sans"', "system-ui", "sans-serif"],
      },
      fontSize: {
        "display-lg": ["48px", { lineHeight: "56px", letterSpacing: "-0.02em", fontWeight: "800" }],
        "headline-lg": ["32px", { lineHeight: "40px", fontWeight: "700" }],
        "headline-md": ["24px", { lineHeight: "32px", fontWeight: "700" }],
        "body-lg": ["20px", { lineHeight: "30px", fontWeight: "400" }],
        "body-md": ["16px", { lineHeight: "24px", fontWeight: "400" }],
        "label-lg": ["14px", { lineHeight: "20px", fontWeight: "500" }],
      },
      animation: {
        bounceSlow: "bounce 3s ease-in-out infinite",
        floatGentle: "floatGentle 4s ease-in-out infinite",
        ringPop: "ringPop 0.6s ease-out",
        shimmer: "shimmer 2s linear infinite",
      },
      keyframes: {
        floatGentle: {
          "0%, 100%": { transform: "translateY(0px)" },
          "50%": { transform: "translateY(-12px)" },
        },
        ringPop: {
          "0%": { transform: "scale(0.6)", opacity: 0 },
          "60%": { transform: "scale(1.1)", opacity: 1 },
          "100%": { transform: "scale(1)", opacity: 1 },
        },
        shimmer: {
          "0%": { backgroundPosition: "-200% 0" },
          "100%": { backgroundPosition: "200% 0" },
        },
      },
    },
  },
  plugins: [],
};
