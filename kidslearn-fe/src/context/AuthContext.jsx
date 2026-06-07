import { createContext, useContext, useState, useEffect, useCallback } from "react";
import { authApi } from "../api/auth";
import { userApi } from "../api/user";

const AuthContext = createContext(null);

const TOKEN_KEY = "kidslearn_token";
const USER_KEY = "kidslearn_user";

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  });
  const [profile, setProfile] = useState(null);
  const [loadingProfile, setLoadingProfile] = useState(false);

  const persist = (data) => {
    localStorage.setItem(TOKEN_KEY, data.token);
    localStorage.setItem(
      USER_KEY,
      JSON.stringify({ userId: data.userId, username: data.username, role: data.role })
    );
    setUser({ userId: data.userId, username: data.username, role: data.role });
  };

  const login = async ({ username, password }) => {
    const data = await authApi.login({ username, password });
    persist(data);
    return data;
  };

  const register = async (payload) => {
    const data = await authApi.register(payload);
    persist(data);
    return data;
  };

  const logout = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setUser(null);
    setProfile(null);
  }, []);

  const refreshProfile = useCallback(async () => {
    if (!user) return null;
    setLoadingProfile(true);
    try {
      const data = await userApi.getMyProfile();
      setProfile(data);
      return data;
    } catch (e) {
      return null;
    } finally {
      setLoadingProfile(false);
    }
  }, [user]);

  useEffect(() => {
    if (user && !profile) refreshProfile();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user]);

  return (
    <AuthContext.Provider
      value={{ user, profile, loadingProfile, login, register, logout, refreshProfile, setProfile }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
}
