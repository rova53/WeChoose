import { useCurrentUser } from "../hooks/auth/useCurrentUser";
import { PolicyRoles } from "../services/auth/PolicyRoles";
import { Navigate } from "react-router-dom";

export const RoleRedirect: React.FC = () => {
    const { user, loading } = useCurrentUser();

    if (loading) return null;

    // Bitwise check for Sales role
    if (user && (user.roles & PolicyRoles.Sales) === PolicyRoles.Sales) {
        return <Navigate to="/admin/users" replace />;
    }

    // Sinon, redirige vers /admin/courses
    return <Navigate to="/admin/courses" replace />;
};