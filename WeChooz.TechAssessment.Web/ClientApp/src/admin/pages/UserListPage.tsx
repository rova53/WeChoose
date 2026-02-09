import React, { useState, useMemo } from 'react';
import { Container, Paper, Typography, Button, Box, Tabs, Tab } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { UserDataGrid } from '../components/users/UserDataGrid';
import { useDeleteUser } from '../../hooks/users/useDeleteUser';
import { useGetAllUsers } from '../../hooks/users/useGetAllUsers';
import { CreateUserModal } from '../components/users/CreateUserModal';
import { useNavigate } from 'react-router-dom';

export const UserListPage: React.FC = () => {
    const { mutate: deleteUser, loading: deleting } = useDeleteUser();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const navigate = useNavigate();
    const [activeTab, setActiveTab] = useState(2);
    const [refreshKey, setRefreshKey] = useState(0);

    const { users, loading, refetch } = useGetAllUsers();

    const fetchUsers = useMemo(() => {
        return async (params: { page: number; pageSize: number; search: string }) => {
            let filtered = users;
            if (params.search) {
                const searchLower = params.search.toLowerCase();
                filtered = users.filter(u =>
                    u.lastName.toLowerCase().includes(searchLower) ||
                    u.firstName.toLowerCase().includes(searchLower) ||
                    u.email.toLowerCase().includes(searchLower) ||
                    (u.companyName || '').toLowerCase().includes(searchLower)
                );
            }
            // Only send first letter of firstName
            const mapped = filtered.map(u => ({ ...u, firstName: u.firstName ? u.firstName[0] : '' }));
            const start = (params.page - 1) * params.pageSize;
            const end = start + params.pageSize;
            const items = mapped.slice(start, end);
            return {
                items,
                total: mapped.length
            };
        };
    }, [users, refreshKey]);

    const handleTabChange = (_: React.SyntheticEvent, value: number) => {
        setActiveTab(value);
        if (value === 0) navigate('/admin/courses');
        if (value === 1) navigate('/admin/sessions');
        if (value === 2) navigate('/admin/users');
    };

    const handleDelete = async (id: string) => {
        if (window.confirm('Êtes-vous sûr de vouloir supprimer cet utilisateur ?')) {
            const result = await deleteUser(id);
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
                        Liste des Utilisateurs
                    </Typography>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={() => setIsModalOpen(true)}
                    >
                        Créer Nouvel Utilisateur
                    </Button>
                </Box>

                <UserDataGrid
                    key={refreshKey}
                    fetchUsers={fetchUsers}
                    onDelete={handleDelete}
                    isDeleting={deleting}
                />
            </Paper>

            {isModalOpen && (
                <CreateUserModal
                    onClose={() => setIsModalOpen(false)}
                    onSuccess={handleCreateSuccess}
                />
            )}
        </Container>
    );
};
