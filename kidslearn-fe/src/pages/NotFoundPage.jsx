import { Link } from "react-router-dom";
import Icon from "../components/Icon";
import Logo from "../components/Logo";

export default function NotFoundPage() {
  return (
    <div className="min-h-[80vh] flex flex-col items-center justify-center text-center px-4 bg-background">
      <div className="text-9xl mb-4 animate-floatGentle">🔍</div>
      <Logo size={48} />
      <h1 className="font-display text-display-lg text-on-surface mt-6">Ơ kìa, trang này đâu rồi?</h1>
      <p className="font-body text-body-lg text-on-surface-variant max-w-md mt-2">
        Bạn đến địa chỉ không tồn tại. Hãy quay về trang chính nhé!
      </p>
      <Link to="/" className="btn-primary mt-8">
        <Icon name="home" size={22} /> Về trang chính
      </Link>
    </div>
  );
}
