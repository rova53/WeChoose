import React, { useState, useCallback } from 'react';
import { Container, Paper, Title, Text, Tabs, Button, Group } from '@mantine/core';
import { SessionDataGrid } from '../components/sessions/SessionDataGrid';
import { useGetAllSessions } from '../../hooks/sessions/useGetAllSessions';
import { useNavigate } from 'react-router-dom';
import { useCurrentUser } from '../../hooks/auth/useCurrentUser';

export const SessionListPage: React.FC = () => {
    const navigate = useNavigate();
    const { logout } = useCurrentUser();
    const [activeTab, setActiveTab] = useState<string>('sessions');
    const [refreshKey, setRefreshKey] = useState(0);

    const { sessions = [], refetch } = useGetAllSessions();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const fetchSessions = useCallback(async (params: { page: number; pageSize: number; search: string }) => {
        let filtered = [...sessions];

        if (params.search) {
            const searchLower = params.search.toLowerCase();
            filtered = sessions.filter(s =>
                s.courseName.toLowerCase().includes(searchLower) ||
                s.deliveryMode.toString().toLowerCase().includes(searchLower)
            );
        }

        const start = (params.page - 1) * params.pageSize;
        const end = start + params.pageSize;

        return {
            items: filtered.slice(start, end),
            total: filtered.length,
        };
    }, [sessions]);

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
                <Title order={2} mb={4}>Gestion des Sessions</Title>
                <Text size="sm" c="dimmed" mb="md">
                    Planifiez de nouvelles sessions de formation, gérez les dates et les modes de diffusion.
                </Text>

                <SessionDataGrid
                    fetchSessions={fetchSessions}
                    refreshKey={refreshKey}
                    onSuccess={async () => { console.log('onSuccess2'); await refetch(); setRefreshKey(k => k + 1); }}
                />
            </Paper>
        </Container>
    );
};
