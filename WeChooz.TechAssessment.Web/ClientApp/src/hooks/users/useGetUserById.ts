import { useApiQuery } from '../useApiQuery';
import { UserService } from '../../services/users/UserService';
import { UserDTO } from '../../services/users/UserDTO';

export const useGetUserById = (id: string) => {
    const { data: user, loading, error, refetch } = useApiQuery<UserDTO>(
        () => UserService.getById(id),
        [id]
    );

    return {
        user,
        loading,
        error,
        refetch
    };
};
