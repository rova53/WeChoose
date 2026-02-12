import { useApiMutation } from '../useApiMutation';
import { UserService } from '../../services/users/UserService';

export const useDeleteUser = () => {
    return useApiMutation<void, string>(
        (id) => UserService.delete(id)
    );
};
