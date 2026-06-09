import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { GraduationCap, BookOpen, BookMarked, PencilLine, Users, TrendingUp, ChevronRight } from "lucide-react";
import { gradeApi } from "../../api/content";
import { unitAdminApi, userAdminApi } from "../../api/admin";
import { AdminPageHeader } from "../../components/AdminUI";

export default function AdminDashboard() {
  const [stats, setStats] = useState({ grades: 0, units: 0, vocabs: 0, quizzes: 0, users: 0, students: 0 });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try {
        const [gradesRes, unitsRes, usersRes] = await Promise.all([
          gradeApi.getAll(),
          unitAdminApi.list(),
          userAdminApi.list(),
        ]);

        // 1. Kiểm tra an toàn: API trả về mảng trực tiếp hay bọc trong .data
        const grades = Array.isArray(gradesRes) ? gradesRes : (gradesRes?.data || []);
        const units = Array.isArray(unitsRes) ? unitsRes : (unitsRes?.data || []);
        const users = Array.isArray(usersRes) ? usersRes : (usersRes?.data || []);

        // Debug xem cấu trúc thực tế trả về là gì
        console.log("Dữ liệu Units nhận được:", units);

        // 2. Tính toán an toàn, bao quát các trường hợp đặt tên biến từ Backend
        const vocabs = units.reduce((s, u) => s + (u.vocabCount || u.VocabCount || u.vocabs?.length || u.Vocabs?.length || 0), 0);
        const quizzes = units.reduce((s, u) => s + (u.quizCount || u.QuizCount || u.quizzes?.length || u.Quizzes?.length || 0), 0);
        const students = users.filter((u) => u.role === "Student" || u.Role === "Student").length;

        // 3. Cập nhật State để hiển thị lên màn hình
        setStats({
          grades: grades.length,
          units: units.length,
          vocabs,
          quizzes,
          users: users.length,
          students,
        });
      } catch (error) {
        console.error("Lỗi lấy dữ liệu dashboard:", error);
      } finally {
        console.log("Load thành công");
        setLoading(false);
      }
    })();
  }, []);

  const cards = [
    { label: "Khối lớp", value: stats.grades, icon: GraduationCap, color: "from-sky-400 to-sky-600",   to: "/admin/grades" },
    { label: "Chủ đề",   value: stats.units,  icon: BookOpen,      color: "from-violet-400 to-violet-600", to: "/admin/units" },
    { label: "Từ vựng",  value: stats.vocabs, icon: BookMarked,    color: "from-emerald-400 to-emerald-600", to: "/admin/vocabularies" },
    { label: "Câu hỏi",  value: stats.quizzes,icon: PencilLine,    color: "from-amber-400 to-amber-600", to: "/admin/quizzes" },
    { label: "Người dùng", value: stats.users, icon: Users, color: "from-rose-400 to-rose-600", to: "/admin/users" },
    { label: "Học sinh", value: stats.students, icon: TrendingUp, color: "from-cyan-400 to-cyan-600", to: "/admin/users" },
  ];

  return (
    <div>
      <AdminPageHeader title="Tổng quan hệ thống" />

      {loading ? (
        <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
          {Array.from({ length: 6 }).map((_, i) => (
            <div key={i} className="h-28 rounded-xl bg-slate-100 animate-pulse" />
          ))}
        </div>
      ) : (
        <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
          {cards.map((c) => (
            <Link
              key={c.label}
              to={c.to}
              className="group bg-white border border-slate-200 rounded-xl p-5 hover:shadow-md hover:-translate-y-0.5 transition"
            >
              <div className="flex items-start justify-between">
                <div className={`w-12 h-12 rounded-xl bg-gradient-to-br ${c.color} flex items-center justify-center text-white shadow`}>
                  <c.icon className="w-6 h-6" />
                </div>
                <ChevronRight className="w-4 h-4 text-slate-300 group-hover:text-slate-500 transition" />
              </div>
              <div className="font-display text-3xl font-bold text-slate-900 mt-3">{c.value}</div>
              <div className="text-sm text-slate-500 font-semibold mt-0.5">{c.label}</div>
            </Link>
          ))}
        </div>
      )}

    </div>
  );
}