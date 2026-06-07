import { api } from "./axios";

export const progressApi = {
  getMyHistory: () => api.get("/progress/me").then((r) => r.data),
  getMyBest: (unitId) => api.get("/progress/me/best", { params: { unitId } }).then((r) => r.data),
};

export const badgeApi = {
  getAllWithStatus: () => api.get("/badges").then((r) => r.data),
  getMyEarned: () => api.get("/badges/me").then((r) => r.data),
  getMyLocked: () => api.get("/badges/me/locked").then((r) => r.data),
};
