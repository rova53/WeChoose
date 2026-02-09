import { useApiMutation } from '../useApiMutation';
import { SessionService } from '../../services/sessions/SessionService';

export const useDeleteSession = () => {
    return useApiMutation<void, string>(
        (id) => SessionService.delete(id)
    );
};
