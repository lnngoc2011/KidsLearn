import { api } from "./axios";

export const gradeApi = {
  getAll: () => api.get("/grades").then((r) => r.data),
  getById: (id) => api.get(`/grades/${id}`).then((r) => r.data),
  getUnits: (id) => api.get(`/grades/${id}/units`).then((r) => r.data),
};

export const unitApi = {
  getById: (id) => api.get(`/units/${id}`).then((r) => r.data),

  search: (keyword) =>
    api.get("/units/search", {
      params: { keyword },
    }).then((r) => r.data),
};

export const vocabApi = {
  getByUnit: (unitId) => api.get("/vocabularies", { params: { unitId } }).then((r) => r.data),
};

export const quizApi = {
  getByUnit: (unitId) => api.get("/quizzes", { params: { unitId } }).then((r) => r.data),
  submit: (payload) => api.post("/quizzes/submit", payload).then((r) => r.data),
  review: (progressId) => api.get(`/quizzes/review/${progressId}`).then((r) => r.data),
};
