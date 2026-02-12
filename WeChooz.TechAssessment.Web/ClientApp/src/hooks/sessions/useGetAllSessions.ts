import { useApiQuery } from '../useApiQuery';
import { SessionService } from '../../services/sessions/SessionService';
import { SessionDTO } from '../../services/sessions/SessionDTO';

export const useGetAllSessions = () => {
    const { data: sessions, loading, error, refetch } = useApiQuery<SessionDTO[]>(
        () => SessionService.getAll(),
        []
    );

    return {
        sessions: sessions || [],
        loading,
        error,
        refetch
    };
};
