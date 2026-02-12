import { useApiQuery } from '../useApiQuery';
import { AuthService, CurrentUser } from '../../services/auth/CurrentUser';

export const useCurrentUser = () => {
    const { data: user, loading, error, refetch } = useApiQuery<CurrentUser>(
        () => AuthService.getCurrentUser(),
        []
    );
    const login = () => {
            window.location.href = '/login';
        };
    const logout = async () => {
        await AuthService.logout();
        window.location.reload();
    };
    return {
        user,
        loading,
        error,
        refetch,
        isLoggedIn: !!user?.isAuthenticated,
        login,
        logout
    };
};