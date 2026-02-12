import { Navigate } from "react-router-dom";
import { useCurrentUser } from "../hooks/auth/useCurrentUser";
import { PolicyRoles } from "../services/auth/PolicyRoles";

interface AuthGuardProps {
    children: React.ReactNode;
    requiredRoles?: PolicyRoles[];
}

export const AuthGuard = ({ children, requiredRoles }: AuthGuardProps) => {
    const { isLoggedIn, user, loading } = useCurrentUser();

    if (loading) return <div>Chargement...</div>;
    if (!isLoggedIn) return <Navigate to="/login" replace />;

    // Bitwise check for flags
    if (
        requiredRoles &&
        user?.roles !== undefined &&
        !requiredRoles.some(role => (user.roles & role) === role)
    ) {
        return <Navigate to="/unauthorized" replace />;
    }

    return <>{children}</>;
};