import { Routes, Route, Navigate } from "react-router-dom";
import ProtectedRoute from "./components/ProtectedRoute";
import AdminRoute from "./components/AdminRoute";
import Layout from "./components/Layout";
import AdminLayout from "./components/AdminLayout";

// Student pages
import LoginPage from "./pages/auth/LoginPage";
import RegisterPage from "./pages/auth/RegisterPage";
import HomePage from "./pages/HomePage";
import GradesPage from "./pages/GradesPage";
import UnitsPage from "./pages/UnitsPage";
import VocabularyPage from "./pages/VocabularyPage";
import QuizPage from "./pages/QuizPage";
import QuizResultPage from "./pages/QuizResultPage";
import ProgressPage from "./pages/ProgressPage";
import BadgesPage from "./pages/BadgesPage";
import ProfilePage from "./pages/ProfilePage";
import NotFoundPage from "./pages/NotFoundPage";

// Admin pages
import AdminDashboard from "./pages/admin/AdminDashboard";
import AdminGradesPage from "./pages/admin/AdminGradesPage";
import AdminUnitsPage from "./pages/admin/AdminUnitsPage";
import AdminVocabulariesPage from "./pages/admin/AdminVocabulariesPage";
import AdminQuizzesPage from "./pages/admin/AdminQuizzesPage";
import AdminUsersPage from "./pages/admin/AdminUsersPage";

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      {/* Student area */}
      <Route element={<ProtectedRoute><Layout /></ProtectedRoute>}>
        <Route path="/" element={<HomePage />} />
        <Route path="/grades" element={<GradesPage />} />
        <Route path="/grades/:gradeId" element={<UnitsPage />} />
        <Route path="/units/:unitId/learn" element={<VocabularyPage />} />
        <Route path="/units/:unitId/quiz" element={<QuizPage />} />
        <Route path="/units/:unitId/result" element={<QuizResultPage />} />
        <Route path="/progress" element={<ProgressPage />} />
        <Route path="/badges" element={<BadgesPage />} />
        <Route path="/profile" element={<ProfilePage />} />
      </Route>

      {/* Admin area */}
      <Route element={<AdminRoute><AdminLayout /></AdminRoute>}>
        <Route path="/admin" element={<AdminDashboard />} />
        <Route path="/admin/grades" element={<AdminGradesPage />} />
        <Route path="/admin/units" element={<AdminUnitsPage />} />
        <Route path="/admin/vocabularies" element={<AdminVocabulariesPage />} />
        <Route path="/admin/quizzes" element={<AdminQuizzesPage />} />
        <Route path="/admin/users" element={<AdminUsersPage />} />
      </Route>

      <Route path="/404" element={<NotFoundPage />} />
      <Route path="*" element={<Navigate to="/404" replace />} />
    </Routes>
  );
}
