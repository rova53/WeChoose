import React, { useState, useCallback } from 'react';
import { Container, Paper, Typography, Box, Tabs, Tab, Button } from '@mui/material';
import LogoutIcon from '@mui/icons-material/Logout';
import { CourseDataGrid } from '../components/courses/CourseDataGrid';
import { useGetAllCourses } from '../../hooks/courses/useGetAllCourses';
import { useNavigate } from 'react-router-dom';
import { useCurrentUser } from '../../hooks/auth/useCurrentUser';

export const CourseListPage: React.FC = () => {
    const navigate = useNavigate();
    const { logout } = useCurrentUser();
    const [activeTab, setActiveTab] = useState(0); // 0 pour l'index des Cours

    const { courses = [], refetch } = useGetAllCourses();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };
    
    const fetchCourses = useCallback(async (params: { page: number; pageSize: number; search: string }) => {
        let filtered = [...courses];

        if (params.search) {
            const searchLower = params.search.toLowerCase();
            filtered = courses.filter(c =>
                c.name.toLowerCase().includes(searchLower) ||
                (c.targetAudience && c.targetAudience.toLowerCase().includes(searchLower))
            );
        }

        const start = (params.page - 1) * params.pageSize;
        const end = start + params.pageSize;
        const items = filtered.slice(start, end);

        return {
            items,
            total: filtered.length
        };
    }, [courses]);

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
                        Catalogue des Cours
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        Gérez le contenu pédagogique, définissez les populations cibles et les durées de formation.
                    </Typography>
                </Box>

                <CourseDataGrid
                    fetchCourses={fetchCourses}
                    onSuccess={() => refetch()}
                />
            </Paper>
        </Container>
    );
};