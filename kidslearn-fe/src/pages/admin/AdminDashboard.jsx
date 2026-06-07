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
        const [grades, units, users] = await Promise.all([
          gradeApi.getAll(),
          unitAdminApi.list(),
          userAdminApi.list(),
        ]);
        
        const vocabs = (units || []).reduce((s, u) => s + (u.vocabCount || 0), 0);
        const quizzes = (units || []).reduce((s, u) => s + (u.quizCount || 0), 0);
        const students = (users || []).filter((u) => u.role === "Student").length;
        setStats({
          grades: grades?.length || 0,
          units: units?.length || 0,
          vocabs, quizzes,
          users: users?.length || 0,
          students,
        });
      } catch {} finally { 
            console.log("load thanh cong");
            setLoading(false); }
      
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
