import React, { useState, useCallback } from 'react';
import { Container, Paper, Title, Text, Tabs, Button, Group } from '@mantine/core';
import { UserDataGrid } from '../components/users/UserDataGrid';
import { useGetAllUsers } from '../../hooks/users/useGetAllUsers';
import { useNavigate } from 'react-router-dom';
import { useCurrentUser } from '../../hooks/auth/useCurrentUser';

export const UserListPage: React.FC = () => {
    const navigate = useNavigate();
    const { logout } = useCurrentUser();
    const [activeTab, setActiveTab] = useState<string>('users');
    const [refreshKey, setRefreshKey] = useState(0);

    const { users = [], refetch } = useGetAllUsers();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const fetchUsers = useCallback(async (params: { page: number; pageSize: number; search: string }) => {
        let filtered = [...users];

        if (params.search) {
            const searchLower = params.search.toLowerCase();
            filtered = users.filter(u =>
                u.lastName.toLowerCase().includes(searchLower) ||
                u.firstName.toLowerCase().includes(searchLower) ||
                u.email.toLowerCase().includes(searchLower) ||
                (u.companyName || '').toLowerCase().includes(searchLower)
            );
        }

        const start = (params.page - 1) * params.pageSize;
        const end = start + params.pageSize;

        return {
            items: filtered.slice(start, end),
            total: filtered.length,
        };
    }, [users]);

    const handleTabChange = (value: string | null) => {
        if (!value) return;
        setActiveTab(value);
        const routes: Record<string, string> = {
            courses: '/admin/courses',
            sessions: '/admin/sessions',
            users: '/admin/users',
        };
        navigate(routes[value]);
    };

    return (
        <Container size="lg" py="xl">

            <Paper p="md" radius="md" shadow="sm">
                <Title order={2} mb={4}>Gestion des Utilisateurs</Title>
                <Text size="sm" c="dimmed" mb="md">
                    Créez, modifiez ou supprimez les comptes utilisateurs et leurs inscriptions.
                </Text>

                <UserDataGrid
                    fetchUsers={fetchUsers}
                    refreshKey={refreshKey}
                    onSuccess={async () => { await refetch(); setRefreshKey(k => k + 1); }}
                />
            </Paper>
        </Container>
    );
};
