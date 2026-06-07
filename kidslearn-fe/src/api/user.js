import { api } from "./axios";

export const userApi = {
  getMyProfile: () => api.get("/users/me/profile").then((r) => r.data),
  updateMyProfile: (payload) => api.put("/users/me", payload).then((r) => r.data),
  changePassword: (payload) => api.put("/users/me/password", payload).then((r) => r.data),
  
  //  Đổi avatar (multipart/form-data)
  updateAvatar: (avatarFile) => {
    const formData = new FormData();
    formData.append("file", avatarFile);
    return api.put("/users/me/avatar", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    }).then((r) => r.data);
  },
};