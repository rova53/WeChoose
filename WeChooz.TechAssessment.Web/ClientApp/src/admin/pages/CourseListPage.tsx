import React, { useState, useCallback } from 'react';
import { Container, Paper, Title, Text, Tabs, Button, Group } from '@mantine/core';
import { CourseDataGrid } from '../components/courses/CourseDataGrid';
import { useGetAllCourses } from '../../hooks/courses/useGetAllCourses';
import { useNavigate } from 'react-router-dom';
import { useCurrentUser } from '../../hooks/auth/useCurrentUser';

export const CourseListPage: React.FC = () => {
    const navigate = useNavigate();
    const { logout } = useCurrentUser();
    const [activeTab, setActiveTab] = useState<string>('courses');
    const [refreshKey, setRefreshKey] = useState(0);

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

        return {
            items: filtered.slice(start, end),
            total: filtered.length,
        };
    }, [courses]);

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
                <Title order={2} mb={4}>Catalogue des Cours</Title>
                <Text size="sm" c="dimmed" mb="md">
                    Gérez le contenu pédagogique, définissez les populations cibles et les durées de formation.
                </Text>

                <CourseDataGrid
                    fetchCourses={fetchCourses}
                    refreshKey={refreshKey}
                    onSuccess={async () => {
                        console
                        await refetch();
                        setRefreshKey(k => k + 1);
                    }}
                />
            </Paper>
        </Container>
    );
};
