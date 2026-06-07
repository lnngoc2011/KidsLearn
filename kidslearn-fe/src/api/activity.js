import { api } from "./axios";

export const activityApi = {
  // Cập nhật vị trí học mới
  updateMyActivity: (unitId, lastVocabIndex) =>
    api.put("/activity/me", { unitId, lastVocabIndex }).then((r) => r.data),

  // Lấy vị trí học cũ khi mở Unit
  getByUnit: (unitId) =>
    api.get(`/activity/me/unit/${unitId}`).then((r) => r.data),

  // Danh sách Unit đang học dở (cho trang tiến độ)
  getInProgress: () =>
    api.get("/activity/me/in-progress").then((r) => r.data),

  // Unit truy cập gần nhất (cho nút "Tiếp tục học")
  getLatest: () =>
    api.get("/activity/me/latest").then((r) => r.data),
};