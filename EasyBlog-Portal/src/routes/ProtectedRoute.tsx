import { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { hasAdminRole, isUserLogin } from "../hooks/useLogin";
interface ProtectedRouteProps {
  children: ReactNode;
  isAdmin: boolean;
}
const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  children,
  isAdmin,
}) => {
  const auth: boolean = isUserLogin();
  const isAdminUser: boolean = hasAdminRole();
  if (!auth) return <Navigate to="/login" />;
  if (isAdmin && !isAdminUser) {
    return <Navigate to="/403" />;
  }
  return children;
};

export default ProtectedRoute;
