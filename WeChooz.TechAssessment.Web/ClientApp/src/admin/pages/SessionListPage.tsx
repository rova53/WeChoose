import React, { useMemo } from 'react';
import { Paper, Title, Text, Stack } from '@mantine/core';
import { SessionDataGrid } from '../components/sessions/SessionDataGrid';
import { useGetAllSessions } from '../../hooks/sessions/useGetAllSessions';

export const SessionListPage: React.FC = () => {
    const { sessions = [], refetch, loading } = useGetAllSessions();

    return (
        <Stack gap="lg">
            <div>
                <Title order={2} fw={700} c="dark.7">
                    Gestion des Sessions
                </Title>
                <Text size="sm" c="dimmed" mt={4}>
                    Planifiez de nouvelles sessions de formation, gérez les dates et les modes de diffusion.
                </Text>
            </div>

            <Paper p="md" radius="md" shadow="sm" withBorder>
                <SessionDataGrid
                    sessions={sessions}
                    loading={loading}
                    onSuccess={refetch}
                />
            </Paper>
        </Stack>
    );
};
