import { useApiQuery } from '../useApiQuery';
import { UserService } from '../../services/users/UserService';
import { UserDTO } from '../../services/users/UserDTO';

export const useGetAllUsers = () => {
    const { data: users, loading, error, refetch } = useApiQuery<UserDTO[]>(
        () => UserService.getAll(),
        []
    );

    return {
        users: users || [],
        loading,
        error,
        refetch
    };
};
