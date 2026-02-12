import { useApiQuery } from '../useApiQuery';
import { SessionService } from '../../services/sessions/SessionService';
import { SessionDTO } from '../../services/sessions/SessionDTO';

export const useGetSessionById = (id: string) => {
    const { data: session, loading, error, refetch } = useApiQuery<SessionDTO>(
        () => SessionService.getById(id),
        [id]
    );

    return {
        session,
        loading,
        error,
        refetch
    };
};
