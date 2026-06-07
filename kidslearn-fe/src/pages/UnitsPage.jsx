
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { gradeApi } from "../api/content";
import LoadingScreen from "../components/LoadingScreen";
import PageHeader from "../components/PageHeader";
import EmptyState from "../components/EmptyState";

export default function UnitsPage() {
  const { gradeId } = useParams();

  const [units, setUnits] = useState([]);
  const [grade, setGrade] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try {
        const [g, u] = await Promise.all([
          gradeApi.getById(gradeId),
          gradeApi.getUnits(gradeId),
        ]);

        setGrade(g);
        setUnits(u || []);
      } finally {
        setLoading(false);
      }
    })();
  }, [gradeId]);

  if (loading) return <LoadingScreen />;

  return (
    <div>
      <PageHeader
        title={grade ? grade.gradeName : "Danh sách Unit"}
        subtitle="Chọn một chủ đề để học từ vựng và làm Quiz."
        back="/grades"
      />

      {units.length === 0 ? (
        <EmptyState
          title="Chưa có Unit nào"
          hint="Khối lớp này chưa có nội dung. Hãy thử khối khác!"
        />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-8">
          {units.map((u) => (
            <div
              key={u.unitId}
            className="
            bg-white
            rounded-[32px]
            overflow-hidden
            border-2 border-blue-100
            shadow-md
            hover:shadow-xl
            hover:border-blue-300
            transition-all
            duration-300
            "
            >
              {/* IMAGE */}
              <div className="h-44 bg-gradient-to-b from-blue-50 to-white flex items-center justify-center border-b">
                {u.imageUrl ? (
                   <img
              src={`http://localhost:5050${u.imageUrl}`}
              alt={u.title}
              className="max-h-36 object-contain"
            />
                ) : (
                  <div className="w-full h-full flex items-center justify-center text-7xl">
                    📚
                  </div>
                )}
              </div>

              {/* CONTENT */}
              <div className="p-4">
                <span className="inline-flex items-center px-3 py-1 rounded-full bg-blue-100 text-blue-700 text-xs font-bold mb-3">
                  Unit {u.orderIndex}
                </span>

                <h3 className="text-xl font-bold text-gray-900 mb-2 line-clamp-2">
                  {u.title}
                </h3>

                {u.description && (
                  <p className="text-gray-500 text-sm mb-4 line-clamp-2">
                    {u.description}
                  </p>
                )}

                <div className="flex gap-4 text-sm text-gray-500 mb-5">
                  <span>📚 {u.vocabCount} từ</span>
                  <span>📝 {u.quizCount} câu</span>
                </div>

                <div className="grid grid-cols-2 gap-3">
                  <Link
                    to={`/units/${u.unitId}/learn`}
                    className="bg-blue-600 hover:bg-blue-700 text-white text-center py-3 rounded-xl font-semibold transition"
                  >
                    📖 Học từ
                  </Link>

                  <Link
                    to={`/units/${u.unitId}/quiz`}
                    className="bg-yellow-400 hover:bg-yellow-500 text-gray-900 text-center py-3 rounded-xl font-semibold transition"
                  >
                    📝 Quiz
                  </Link>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
