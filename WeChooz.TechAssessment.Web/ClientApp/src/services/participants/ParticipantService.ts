import { apiClient } from '../apiClient';
import { ParticipantDTO } from './ParticipantDTO';

export const ParticipantService = {
    getAll: () => apiClient.get<ParticipantDTO[]>('participants'),
    getById: (id: string) => apiClient.get<ParticipantDTO>(`participants/${id}`),
    create: (participant: Partial<ParticipantDTO>) => apiClient.post<ParticipantDTO>('participants', participant),
    update: (id: string, participant: Partial<ParticipantDTO>) => apiClient.put<ParticipantDTO>(`participants/${id}`, participant),
    delete: (id: string) => apiClient.delete<void>(`participants/${id}`),
};