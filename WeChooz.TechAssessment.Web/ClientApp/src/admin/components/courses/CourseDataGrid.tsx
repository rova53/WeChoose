import React from 'react';
import { TextInput, Button, Group, ActionIcon, Tooltip, Text, Stack } from '@mantine/core';
import { DataTable } from 'mantine-datatable';
import { IconEdit, IconTrash, IconPlus, IconSearch } from '@tabler/icons-react';
import { CourseDTO } from '../../../services/courses/CourseDTO';
import { CourseModal } from './CourseModal';
import { useDeleteCourse } from '../../../hooks/courses/useDeleteCourse';
import { TargetAudience, targetAudienceLabels } from '../../../services/courses/TargetAudience';

interface Props {
    fetchCourses: (params: { page: number; pageSize: number; search: string }) => Promise<{ items: CourseDTO[]; total: number }>;
    onSuccess: () => void;
    refreshKey: number;
}

export const CourseDataGrid: React.FC<Props> = ({ fetchCourses, onSuccess, refreshKey }) => {
    const [page, setPage] = React.useState(1);
    const [pageSize, setPageSize] = React.useState(10);
    const [search, setSearch] = React.useState('');
    const [rows, setRows] = React.useState<CourseDTO[]>([]);
    const [rowCount, setRowCount] = React.useState(0);
    const [loading, setLoading] = React.useState(false);
    const [isDeleting, setDeleting] = React.useState(false);
    const [forceLoadKey, setForceLoadKey] = React.useState(0);

    const { mutate: deleteCourse } = useDeleteCourse();

    const [selectedCourse, setSelectedCourse] = React.useState<CourseDTO | null>(null);
    const [isModalOpen, setIsModalOpen] = React.useState(false);

    React.useEffect(() => {
        if (refreshKey > 0) {
            setPage(1);
            setForceLoadKey(k => k + 1);
        }
    }, [refreshKey]);

    const loadData = React.useCallback(() => {
        setLoading(true);
        fetchCourses({ page, pageSize, search })
            .then(data => {
                setRows([...data.items].sort((a, b) => b.id.localeCompare(a.id)));
                setRowCount(data.total);
                setLoading(false);
            })
            .catch(err => {
                console.error('Erreur chargement grid cours:', err);
                setLoading(false);
            });
    }, [page, pageSize, search, fetchCourses, forceLoadKey]);

    React.useEffect(() => {
        loadData();
    }, [loadData]);

    const handleOpenCreate = () => {
        setSelectedCourse(null);
        setIsModalOpen(true);
    };

    const handleOpenEdit = (course: CourseDTO) => {
        setSelectedCourse(course);
        setIsModalOpen(true);
    };

    return (
        <Stack gap="sm">
            <Group>
                <TextInput
                    leftSection={<IconSearch size={16} />}
                    placeholder="Titre, population cible..."
                    value={search}
                    onChange={e => setSearch(e.target.value)}
                    style={{ flex: 1 }}
                />
                <Button leftSection={<IconPlus size={16} />} onClick={handleOpenCreate}>
                    Ajouter
                </Button>
            </Group>

            <DataTable
                withTableBorder
                borderRadius="sm"
                highlightOnHover
                records={rows}
                fetching={loading}
                totalRecords={rowCount}
                recordsPerPage={pageSize}
                page={page}
                onPageChange={setPage}
                recordsPerPageOptions={[5, 10, 25, 50]}
                onRecordsPerPageChange={p => { setPageSize(p); setPage(1); }}
                noRecordsText="Aucun cours trouvé"
                columns={[
                    {
                        accessor: 'name',
                        title: 'Titre du Cours',
                        render: (course) => (
                            <Text fw={600} c="blue">{course.name}</Text>
                        ),
                    },
                    {
                        accessor: 'targetAudience',
                        title: 'Population cible',
                        render: (course) =>
                            targetAudienceLabels[course.targetAudience as TargetAudience] ?? course.targetAudience,
                    },
                    {
                        accessor: 'maxCapacity',
                        title: 'Capacité Max',
                        textAlign: 'center',
                        render: (course) => <Text fw={700}>{course.maxCapacity}</Text>,
                    },
                    {
                        accessor: 'actions',
                        title: 'Actions',
                        textAlign: 'right',
                        render: (course) => (
                            <Group gap={4} justify="flex-end" wrap="nowrap">
                                <Tooltip label="Modifier">
                                    <ActionIcon variant="subtle" color="blue" onClick={() => handleOpenEdit(course)}>
                                        <IconEdit size={16} />
                                    </ActionIcon>
                                </Tooltip>
                                <Tooltip label="Supprimer">
                                    <ActionIcon
                                        variant="subtle"
                                        color="red"
                                        disabled={isDeleting}
                                        onClick={() => {
                                            if (window.confirm(`Supprimer le cours "${course.name}" ?`)) {
                                                setDeleting(true);
                                                deleteCourse(course.id, {
                                                    onSuccess: () => { onSuccess(); setDeleting(false); },
                                                    onError: () => setDeleting(false),
                                                });
                                            }
                                        }}
                                    >
                                        <IconTrash size={16} />
                                    </ActionIcon>
                                </Tooltip>
                            </Group>
                        ),
                    },
                ]}
            />

            {isModalOpen && (
                <CourseModal
                    course={selectedCourse}
                    onClose={() => { setIsModalOpen(false); setSelectedCourse(null); }}
                    onSuccess={() => { setIsModalOpen(false); onSuccess(); }}
                />
            )}
        </Stack>
    );
};
