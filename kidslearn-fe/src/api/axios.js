import axios from "axios";

const baseURL = "http://localhost:5050/api";

export const api = axios.create({
  baseURL,
  headers: { "Content-Type": "application/json" },
});

// Attach JWT to every request
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("kidslearn_token");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  // Nếu là FormData thì bỏ Content-Type
  if (config.data instanceof FormData) {
    delete config.headers["Content-Type"];
  }

  return config;
});

// Unwrap ApiResponse { success, data, message }
api.interceptors.response.use(
  (res) => {
    if (res.data && typeof res.data === "object" && "data" in res.data) {
      return { ...res, data: res.data.data, _message: res.data.message };
    }
    return res;
  },
  (err) => {
    if (err.response?.status === 401) {
      localStorage.removeItem("kidslearn_token");
      localStorage.removeItem("kidslearn_user");
      if (!window.location.pathname.startsWith("/login")) {
        window.location.href = "/login";
      }
    }
    // Normalize error message
    const apiMsg = err.response?.data?.message;
    err.userMessage = apiMsg || err.message || "Đã xảy ra lỗi";
    return Promise.reject(err);
  }
);
