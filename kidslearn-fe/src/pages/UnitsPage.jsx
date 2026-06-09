import React, { useEffect, useState } from "react";
import { Link, useParams, useNavigate } from "react-router-dom";
import { gradeApi } from "../api/content";
import LoadingScreen from "../components/LoadingScreen";
import PageHeader from "../components/PageHeader";
import EmptyState from "../components/EmptyState";

export default function UnitsPage() {
  const { gradeId } = useParams();
  const navigate = useNavigate();
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
        /* ĐỔI xl:grid-cols-3 THÀNH xl:grid-cols-4 VÀ GIẢM gap XUỐNG 6 ĐỂ VỪA VẶN MÀN HÌNH */
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-6">
          {units.map((u, idx) => {
            const reviewNo = (idx + 1) / 4;
            const startUnit = (reviewNo - 1) * 4 + 1;
            const endUnit = reviewNo * 4;

            return (
              <React.Fragment key={u.unitId}>
                <div
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

                {/* THẺ REVIEW (Cứ mỗi 4 bài) */}
                {(idx + 1) % 4 === 0 && (
                  <div
                    onClick={() =>
                      navigate(`/grades/${gradeId}/review/${reviewNo}`)
                    }
                    className="
                      cursor-pointer
                      xl:col-span-4
                      md:col-span-2
                      bg-gradient-to-r
                      from-green-400
                      to-emerald-500
                      rounded-[28px]
                      p-6
                      text-white
                      shadow-lg
                      hover:scale-[1.01]
                      transition
                    "
                  >
                    <h3 className="text-2xl font-bold mb-2">
                      ⭐ Review {reviewNo}
                    </h3>
                    <p>
                      Ôn tập Unit {startUnit} đến Unit {endUnit}
                    </p>
                    <p className="mt-2 opacity-90">10 câu hỏi ngẫu nhiên</p>
                  </div>
                )}
              </React.Fragment>
            );
          })}

          {/* FINAL REVIEW BLOCK */}
          {units.length > 0 && (
            <div
              onClick={() => navigate(`/grades/${gradeId}/final-review`)}
              className="
                cursor-pointer
                xl:col-span-4
                md:col-span-2
                bg-gradient-to-r
                from-purple-500
                to-pink-500
                rounded-[28px]
                p-6
                text-white
                shadow-lg
                hover:scale-[1.01]
                transition
              "
            >
              <h3 className="text-2xl font-bold mb-2">🏆 Final Review</h3>
              <p>Ôn tập toàn bộ {units.length} Unit</p>
              <p className="mt-2 opacity-90">20 câu hỏi ngẫu nhiên</p>
            </div>
          )}
        </div>
      )}
    </div>
  );
}