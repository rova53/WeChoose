import React from 'react';
import { Paper, Title, Text, Stack } from '@mantine/core';
import { UserDataGrid } from '../components/users/UserDataGrid';
import { useGetAllUsers } from '../../hooks/users/useGetAllUsers';

export const UserListPage: React.FC = () => {
    const { users = [], refetch, loading } = useGetAllUsers();

    return (
        <Stack gap="lg">
            <div>
                <Title order={2} fw={700} c="dark.7">
                    Gestion des Utilisateurs
                </Title>
                <Text size="sm" c="dimmed" mt={4}>
                    Créez, modifiez ou supprimez les comptes utilisateurs et leurs inscriptions.
                </Text>
            </div>

            <Paper p="md" radius="md" shadow="sm" withBorder>
                <UserDataGrid
                    users={users}
                    loading={loading}
                    onSuccess={refetch}
                />
            </Paper>
        </Stack>
    );
};
