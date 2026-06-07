import { Outlet } from "react-router-dom";
import Navbar from "./Navbar";

export default function Layout() {
  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      <main className="flex-1 w-full max-w-[1440px] mx-auto px-container-margin-mobile md:px-container-margin-desktop py-section-gap">
        <Outlet />
      </main>
      <footer className="w-full py-12 mt-section-gap bg-surface-container-high border-t-2 border-outline-variant flex flex-col items-center justify-center gap-3 px-container-margin-mobile md:px-container-margin-desktop">
        <div className="font-display font-bold text-headline-md text-secondary">KidsLearn</div>
        <p className="font-body text-label-lg text-on-surface-variant">
          © {new Date().getFullYear()} KidsLearn — Học tiếng Anh thật vui!
        </p>
      </footer>
    </div>
  );
}
