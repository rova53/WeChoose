import React, { useState, useCallback } from 'react';
import { Container, Paper, Typography, Box, Tabs, Tab, Button } from '@mui/material';
import LogoutIcon from '@mui/icons-material/Logout';
import { SessionDataGrid } from '../components/sessions/SessionDataGrid';
import { useGetAllSessions } from '../../hooks/sessions/useGetAllSessions';
import { useNavigate } from 'react-router-dom';
import { useCurrentUser } from '../../hooks/auth/useCurrentUser';

export const SessionListPage: React.FC = () => {
    const navigate = useNavigate();
    const { logout } = useCurrentUser();
    const [activeTab, setActiveTab] = useState(1); // 1 car c'est l'index des Sessions

    // On récupère les sessions et la fonction refetch
    const { sessions = [], refetch } = useGetAllSessions();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    /**
     * Pont entre les données brutes et les besoins du DataGrid
     * (Pagination et recherche locales)
     */
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
        const items = filtered.slice(start, end);

        return {
            items,
            total: filtered.length
        };
    }, [sessions]);

    const handleTabChange = (_: React.SyntheticEvent, value: number) => {
        setActiveTab(value);
        const routes = ['/admin/courses', '/admin/sessions', '/admin/users'];
        navigate(routes[value]);
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

            {/* Contenu principal */}
            <Paper sx={{ p: 3, borderRadius: 2, boxShadow: 3 }}>
                <Box sx={{ mb: 3 }}>
                    <Typography variant="h4" component="h1" sx={{ fontWeight: 'bold', color: 'text.primary' }}>
                        Gestion des Sessions
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        Planifiez de nouvelles sessions de formation, gérez les dates et les modes de diffusion.
                    </Typography>
                </Box>

                <SessionDataGrid
                    fetchSessions={fetchSessions}
                    onSuccess={() => refetch()}
                />
            </Paper>
        </Container>
    );
};