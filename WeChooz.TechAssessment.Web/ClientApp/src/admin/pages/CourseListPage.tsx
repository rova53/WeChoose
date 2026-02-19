import React from 'react';
import { Paper, Title, Text, Stack } from '@mantine/core';
import { CourseDataGrid } from '../components/courses/CourseDataGrid';
import { useGetAllCourses } from '../../hooks/courses/useGetAllCourses';

export const CourseListPage: React.FC = () => {
    const { courses = [], refetch, loading } = useGetAllCourses();

    return (
        <Stack gap="lg">
            <div>
                <Title order={2} fw={700} c="dark.7">
                    Catalogue des Cours
                </Title>
                <Text size="sm" c="dimmed" mt={4}>
                    Gérez le contenu pédagogique, définissez les populations cibles et les durées de formation.
                </Text>
            </div>

            <Paper p="md" radius="md" shadow="sm" withBorder>
                <CourseDataGrid
                    courses={courses}
                    loading={loading}
                    onSuccess={refetch}
                />
            </Paper>
        </Stack>
    );
};
