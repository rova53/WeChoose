import { apiClient } from '../apiClient';
import { SessionDTO } from './SessionDTO';

export const SessionService = {
    getAll: () => apiClient.get<SessionDTO[]>('sessions'),
    getById: (id: string) => apiClient.get<SessionDTO>(`session/${id}`),
    create: (session: Partial<SessionDTO>) => apiClient.post<SessionDTO>('session', session),
    update: (id: string, session: Partial<SessionDTO>) => apiClient.put<SessionDTO>(`session/${id}`, session),
    delete: (id: string) => apiClient.delete<void>(`session/${id}`),
    enroll: (sessionId: string) => apiClient.post<void>(`session/${sessionId}/enroll`, null)
};