import { api } from "./axios";

// Grades
export const gradeAdminApi = {
  create: (dto) => api.post("/grades", dto).then((r) => r.data),
  update: (id, dto) => api.put(`/grades/${id}`, dto).then((r) => r.data),
  delete: (id) => api.delete(`/grades/${id}`).then((r) => r.data),
};

// Units
export const unitAdminApi = {
  list: async (gradeId) => {
    const params = gradeId ? { gradeId } : {};
    const res = await api.get("/units", { params });
    return res.data;
  },

  create: async (data) => {
    const formData = new FormData();
    formData.append("GradeId", data.gradeId);
    formData.append("Title", data.title);
    formData.append("Description", data.description || "");
    formData.append("OrderIndex", data.orderIndex || 1);
    if (data.avatarFile) {
      formData.append("AvatarFile", data.avatarFile);
    }
    const res = await api.post("/units", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return res.data;
  },

  update: async (id, data) => {
    const formData = new FormData();
    formData.append("GradeId", data.gradeId);
    formData.append("Title", data.title);
    formData.append("Description", data.description || "");
    formData.append("OrderIndex", data.orderIndex || 1);
    if (data.avatarFile) {
      formData.append("AvatarFile", data.avatarFile);
    }
    const res = await api.put(`/units/${id}`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return res.data;
  },

  delete: async (id) => {
    const res = await api.delete(`/units/${id}`);
    return res.data;
  },
  
};

// Vocabularies
export const vocabAdminApi = {
  list: (unitId) => api.get("/vocabularies", { params: { unitId } }).then((r) => r.data),
  create: (dto) => api.post("/vocabularies", dto).then((r) => r.data),
  update: (id, dto) => api.put(`/vocabularies/${id}`, dto).then((r) => r.data),
  delete: (id) => api.delete(`/vocabularies/${id}`).then((r) => r.data),
};

// Quizzes
export const quizAdminApi = {
  list: (unitId) =>
    api.get("/quizzes", { params: { unitId } }).then((r) => r.data),

  create: (dto) =>
    api.post("/quizzes", dto, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    }).then((r) => r.data),

  update: (id, dto) =>
    api.put(`/quizzes/${id}`, dto, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    }).then((r) => r.data),

  delete: (id) =>
    api.delete(`/quizzes/${id}`).then((r) => r.data),
};

// Users
export const userAdminApi = {
  list: () => api.get("/users").then((r) => r.data),
  getById: (id) => api.get(`/users/${id}`).then((r) => r.data),
  changeRole: (id, role) => api.put(`/users/${id}/role`, { role }).then((r) => r.data),
  resetPassword: (id, newPassword = "Kid@123") =>
    api.put(`/users/${id}/reset-password`, { newPassword }).then((r) => r.data),
  delete: (id) => api.delete(`/users/${id}`).then((r) => r.data),
};
