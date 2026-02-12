import { apiClient } from '../apiClient';
import { UserDTO } from './UserDTO';

export const UserService = {
    getAll: () => apiClient.get<UserDTO[]>('users'),
    getById: (id: string) => apiClient.get<UserDTO>(`user/${id}`),
    create: (User: Partial<UserDTO>) => apiClient.post<UserDTO>('user', User),
    update: (id: string, User: Partial<UserDTO>) => apiClient.put<UserDTO>(`user/${id}`, User),
    delete: (id: string) => apiClient.delete<void>(`user/${id}`),
};