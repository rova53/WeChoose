import { apiClient } from '../apiClient';
import { UserDTO } from './UserDTO';

export const UserService = {
    getAll: () => apiClient.get<UserDTO[]>('Users'),
    getById: (id: string) => apiClient.get<UserDTO>(`Users/${id}`),
    create: (User: Partial<UserDTO>) => apiClient.post<UserDTO>('Users', User),
    update: (id: string, User: Partial<UserDTO>) => apiClient.put<UserDTO>(`Users/${id}`, User),
    delete: (id: string) => apiClient.delete<void>(`Users/${id}`),
};