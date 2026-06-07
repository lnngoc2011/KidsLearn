import { useCallback, useEffect, useRef, useState } from "react";

export function useTTS() {
  const [supported, setSupported] = useState(false);
  const [voice, setVoice] = useState(null);
  const speakingRef = useRef(false);

  useEffect(() => {
    if (typeof window === "undefined" || !window.speechSynthesis) {
      setSupported(false);
      return;
    }
    setSupported(true);

    const pickVoice = () => {
      const voices = window.speechSynthesis.getVoices();
      const pref =
        voices.find((v) => /en-US/i.test(v.lang) && /female|samantha|google/i.test(v.name)) ||
        voices.find((v) => /en-US/i.test(v.lang)) ||
        voices.find((v) => /en/i.test(v.lang)) ||
        voices[0];
      if (pref) setVoice(pref);
    };
    pickVoice();
    window.speechSynthesis.onvoiceschanged = pickVoice;
    return () => {
      try { window.speechSynthesis.cancel(); } catch {}
    };
  }, []);

  const speak = useCallback(
    (text, { rate = 0.9, pitch = 1.1 } = {}) => {
      if (!supported || !text) return;
      try {
        window.speechSynthesis.cancel();
        const u = new SpeechSynthesisUtterance(text);
        if (voice) u.voice = voice;
        u.lang = voice?.lang || "en-US";
        u.rate = rate;
        u.pitch = pitch;
        speakingRef.current = true;
        u.onend = () => (speakingRef.current = false);
        window.speechSynthesis.speak(u);
      } catch {}
    },
    [supported, voice]
  );

  return { supported, speak };
}
