import React, { useState, useMemo } from 'react';
import { Container, Paper, Typography, Button, Box, Tabs, Tab } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { useGetAllCourses } from '../../hooks/courses/useGetAllCourses';
import { useDeleteCourse } from '../../hooks/courses/useDeleteCourse';
import { CreateCourseModal } from '../components/courses/CreateCourseModal';
import { CourseDataGrid } from '../components/courses/CourseDataGrid';
import { useNavigate } from 'react-router-dom';

export const CourseListPage: React.FC = () => {
    const { courses = [], loading, error, refetch } = useGetAllCourses();
    const { mutate: deleteCourse, loading: deleting } = useDeleteCourse();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const navigate = useNavigate();
    const [activeTab, setActiveTab] = useState(0);
    const [refreshKey, setRefreshKey] = useState(0);

    const fetchCourses = useMemo(() => {
        return async (params: { page: number; pageSize: number; search: string }) => {
            let filtered = courses;
            if (params.search) {
                const searchLower = params.search.toLowerCase();
                filtered = courses.filter(c =>
                    c.name.toLowerCase().includes(searchLower)
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
    }, [courses, refreshKey]);

    const handleDelete = async (id: string) => {
        if (window.confirm('Êtes-vous sûr de vouloir supprimer ce cours ?')) {
            await deleteCourse(id);
            setRefreshKey(prev => prev + 1);
            refetch();
        }
    };

    const handleCreateSuccess = () => {
        setIsModalOpen(false);
        setRefreshKey(prev => prev + 1);
        refetch();
    };

    const handleTabChange = (_: React.SyntheticEvent, value: number) => {
        setActiveTab(value);
        if (value === 0) navigate('/admin/courses');
        if (value === 1) navigate('/admin/sessions');
        if (value === 2) navigate('/admin/users');
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
                        Catalogue des Cours
                    </Typography>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={() => setIsModalOpen(true)}
                    >
                        Nouveau Cours
                    </Button>
                </Box>

                <CourseDataGrid
                    key={refreshKey}
                    fetchCourses={fetchCourses}
                    onDelete={handleDelete}
                    isDeleting={deleting}
                />
            </Paper>

            {isModalOpen && (
                <CreateCourseModal
                    onClose={() => setIsModalOpen(false)}
                    onSuccess={handleCreateSuccess}
                />
            )}
        </Container>
    );
};