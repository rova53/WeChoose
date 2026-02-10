import { apiClient } from '../apiClient';
import { SessionDTO } from './SessionDTO';

export const SessionService = {
    getAll: () => apiClient.get<SessionDTO[]>('sessions'),
    getById: (id: string) => apiClient.get<SessionDTO>(`sessions/${id}`),
    create: (session: Partial<SessionDTO>) => apiClient.post<SessionDTO>('sessions', session),
    update: (id: string, session: Partial<SessionDTO>) => apiClient.put<SessionDTO>(`sessions/${id}`, session),
    delete: (id: string) => apiClient.delete<void>(`sessions/${id}`),
    enroll: (sessionId: string) => apiClient.post<void>(`sessions/${sessionId}/enroll`, null)
};