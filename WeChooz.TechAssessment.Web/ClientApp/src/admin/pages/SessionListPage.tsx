import React, { useState, useMemo } from 'react';
import { Container, Paper, Typography, Button, Box, Tabs, Tab } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { SessionDataGrid } from '../components/sessions/SessionDataGrid';
import { useDeleteSession } from '../../hooks/sessions/useDeleteSession';
import { useGetAllSessions } from '../../hooks/sessions/useGetAllSessions';
import { CreateSessionModal } from '../components/sessions/CreateSessionModal';
import { useNavigate } from 'react-router-dom';

export const SessionListPage: React.FC = () => {
    const { mutate: deleteSession, loading: deleting } = useDeleteSession();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const navigate = useNavigate();
    const [activeTab, setActiveTab] = useState(1);
    const [refreshKey, setRefreshKey] = useState(0);

    const { sessions, loading, refetch } = useGetAllSessions();

    const fetchSessions = useMemo(() => {
        return async (params: { page: number; pageSize: number; search: string }) => {
            let filtered = sessions;
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
        };
    }, [sessions, refreshKey]);

    const handleTabChange = (_: React.SyntheticEvent, value: number) => {
        setActiveTab(value);
        if (value === 0) navigate('/admin/courses');
        if (value === 1) navigate('/admin/sessions');
        if (value === 2) navigate('/admin/users');
    };

    const handleDelete = async (id: string) => {
        if (window.confirm('Êtes-vous sûr de vouloir supprimer cette session ?')) {
            const result = await deleteSession(id);
            if (result !== null) {
                setRefreshKey(prev => prev + 1);
                refetch();
            }
        }
    };

    const handleCreateSuccess = () => {
        setIsModalOpen(false);
        setRefreshKey(prev => prev + 1);
        refetch();
    };

    return (
        <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
            {/* Tabs Navigation */}
            <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}>
                <Tabs value={activeTab} onChange={handleTabChange}>
                    <Tab label="Cours" />
                    <Tab label="Sessions" />
                    <Tab label="Utilisateurs" />
                </Tabs>
            </Box>

            {/* Content */}
            <Paper sx={{ p: 3 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
                    <Typography variant="h4" component="h1">
                        Liste des Sessions
                    </Typography>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={() => setIsModalOpen(true)}
                    >
                        Créer Nouvelle Session
                    </Button>
                </Box>

                <SessionDataGrid
                    key={refreshKey}
                    fetchSessions={fetchSessions}
                    onDelete={handleDelete}
                    isDeleting={deleting}
                />
            </Paper>

            {isModalOpen && (
                <CreateSessionModal
                    onClose={() => setIsModalOpen(false)}
                    onSuccess={handleCreateSuccess}
                />
            )}
        </Container>
    );
};