import React, { useState, useCallback } from 'react';
import { Container, Paper, Typography, Button, Box, Tabs, Tab } from '@mui/material';
import LogoutIcon from '@mui/icons-material/Logout';
import { UserDataGrid } from '../components/users/UserDataGrid';
import { useDeleteUser } from '../../hooks/users/useDeleteUser';
import { useGetAllUsers } from '../../hooks/users/useGetAllUsers';
import { useNavigate } from 'react-router-dom';
import { useCurrentUser } from '../../hooks/auth/useCurrentUser';

export const UserListPage: React.FC = () => {
    const navigate = useNavigate();
    const { logout } = useCurrentUser();
    const { mutate: deleteUser, loading: deleting } = useDeleteUser();

    // On récupère les données et la fonction de rafraîchissement
    const { users = [], refetch } = useGetAllUsers();

    const [activeTab, setActiveTab] = useState(2);

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    /**
     * Cette fonction fait le pont entre les données brutes de useGetAllUsers
     * et les besoins de pagination/recherche du DataGrid.
     */
    const fetchUsers = useCallback(async (params: { page: number; pageSize: number; search: string }) => {
        // 1. On applique le filtre de recherche
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

        // 2. On applique la pagination locale
        const start = (params.page - 1) * params.pageSize;
        const end = start + params.pageSize;
        const items = filtered.slice(start, end);

        return {
            items,
            total: filtered.length
        };
    }, [users]); // Se déclenche quand 'users' change (après un refetch)

    const handleTabChange = (_: React.SyntheticEvent, value: number) => {
        setActiveTab(value);
        const routes = ['/admin/courses', '/admin/sessions', '/admin/users'];
        navigate(routes[value]);
    };

    const handleDelete = async (id: string) => {
        try {
            await deleteUser(id);
            await refetch(); // Force la mise à jour de la liste locale
        } catch (error) {
            console.error("Erreur lors de la suppression:", error);
        }
    };

    return (
        <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
            {/* Barre de navigation haute */}
            <Box sx={{
                borderBottom: 1,
                borderColor: 'divider',
                mb: 3,
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
            }}>
                <Tabs value={activeTab} onChange={handleTabChange}>
                    <Tab label="Cours" />
                    <Tab label="Sessions" />
                    <Tab label="Utilisateurs" />
                </Tabs>

                <Button
                    variant="text"
                    color="error"
                    startIcon={<LogoutIcon />}
                    onClick={handleLogout}
                    sx={{ fontWeight: 'bold' }}
                >
                    Déconnexion
                </Button>
            </Box>

            <Paper sx={{ p: 3, borderRadius: 2, boxShadow: 3 }}>
                <Box sx={{ mb: 3 }}>
                    <Typography variant="h4" component="h1" sx={{ fontWeight: 'bold', color: 'text.primary' }}>
                        Gestion des Utilisateurs
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        Créez, modifiez ou supprimez les comptes utilisateurs et leurs inscriptions.
                    </Typography>
                </Box>
                <UserDataGrid
                    fetchUsers={fetchUsers}
                    onSuccess={() => refetch()}
                />
            </Paper>
        </Container>
    );
};