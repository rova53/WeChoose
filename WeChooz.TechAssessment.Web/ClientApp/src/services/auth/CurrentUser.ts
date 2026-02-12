import { apiClient } from '../apiClient';
import { PolicyRoles } from './PolicyRoles';

export interface CurrentUser {
    isAuthenticated: boolean;
    username: string | null;
    roles: PolicyRoles;
}

export const AuthService = {
    getCurrentUser: () => apiClient.get<CurrentUser>('account/me'),
    logout: () => apiClient.post<void>('account/logout', null),
};