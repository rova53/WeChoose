import * as React from 'react';
import { DataTable, type DataTableColumn } from 'mantine-datatable';
import { ActionIcon, Tooltip, Group, Text, Badge } from '@mantine/core';
import { IconTrash, IconEdit } from '@tabler/icons-react';
import { CourseDTO } from '../../../services/courses/CourseDTO';
import { CourseModal } from './CourseModal';
import { useDeleteCourse } from '../../../hooks/courses/useDeleteCourse';
import { TargetAudience, targetAudienceLabels } from "../../../services/courses/TargetAudience.ts";
import { TableToolbar } from '../common/TableToolbar';

interface Props {
    courses: CourseDTO[];
    loading: boolean;
    onSuccess: () => void | Promise<void>;
}

const PAGE_SIZES = [10, 25, 50];

export const CourseDataGrid: React.FC<Props> = ({ courses, loading, onSuccess }) => {
    const [page, setPage] = React.useState(1);
    const [pageSize, setPageSize] = React.useState(10);
    const [search, setSearch] = React.useState('');
    const [isDeleting, setDeleting] = React.useState(false);

    const { mutate: deleteCourse } = useDeleteCourse();

    const [selectedCourse, setSelectedCourse] = React.useState<CourseDTO | null>(null);
    const [isModalOpen, setIsModalOpen] = React.useState(false);

    const filteredRecords = React.useMemo(() => {
        let filtered = [...courses];
        if (search) {
            const searchLower = search.toLowerCase();
            filtered = filtered.filter(c =>
                c.name.toLowerCase().includes(searchLower) ||
                (c.targetAudience && targetAudienceLabels[c.targetAudience as TargetAudience]?.toLowerCase().includes(searchLower))
            );
        }
        return filtered;
    }, [courses, search]);

    const records = React.useMemo(() => {
        const start = (page - 1) * pageSize;
        const end = start + pageSize;
        return filteredRecords.slice(start, end);
    }, [filteredRecords, page, pageSize]);

    const handleSuccess = async () => {
        await onSuccess();
    };

    const handleOpenCreate = () => {
        setSelectedCourse(null);
        setIsModalOpen(true);
    };

    const handleOpenEdit = (course: CourseDTO) => {
        setSelectedCourse(course);
        setIsModalOpen(true);
    };

    const columns: DataTableColumn<CourseDTO>[] = [
        {
            accessor: 'name',
            title: 'Titre du Cours',
            width: '35%',
            render: (record) => (
                <Text fw={600} size="sm" c="blue.7">
                    {record.name}
                </Text>
            ),
        },
        {
            accessor: 'targetAudience',
            title: 'Population cible',
            width: '25%',
            render: (record) => {
                const audienceValue = record.targetAudience as TargetAudience;
                return (
                    <Badge variant="light" color="gray">
                        {targetAudienceLabels[audienceValue]}
                    </Badge>
                );
            },
        },
        {
            accessor: 'maxCapacity',
            title: 'Capacité Max',
            textAlign: 'center',
            width: 150,
            render: (record) => (
                <Badge variant="outline" color="dark">
                    {record.maxCapacity} pers.
                </Badge>
            ),
        },
        {
            accessor: 'actions',
            title: 'Actions',
            textAlign: 'right',
            width: 100,
            render: (record) => (
                <Group gap="xs" justify="flex-end" wrap="nowrap">
                    <Tooltip label="Modifier">
                        <ActionIcon
                            variant="subtle"
                            color="blue"
                            onClick={() => handleOpenEdit(record)}
                        >
                            <IconEdit size={16} />
                        </ActionIcon>
                    </Tooltip>

                    <Tooltip label="Supprimer">
                        <ActionIcon
                            variant="subtle"
                            color="red"
                            disabled={isDeleting}
                            onClick={() => {
                                if (window.confirm(`Supprimer le cours "${record.name}" ?`)) {
                                    setDeleting(true);
                                    deleteCourse(record.id, {
                                        onSuccess: () => {
                                            handleSuccess();
                                            setDeleting(false);
                                        },
                                        onError: () => setDeleting(false)
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
    ];

    return (
        <div>
            <TableToolbar
                search={search}
                onSearchChange={(val) => { setSearch(val); setPage(1); }}
                onAdd={handleOpenCreate}
                searchPlaceholder="Rechercher un cours..."
                addButtonLabel="Nouveau cours"
            />

            <DataTable
                withTableBorder
                borderRadius="md"
                striped
                highlightOnHover
                records={records}
                columns={columns}
                fetching={loading}
                totalRecords={filteredRecords.length}
                recordsPerPage={pageSize}
                page={page}
                onPageChange={setPage}
                recordsPerPageOptions={PAGE_SIZES}
                onRecordsPerPageChange={(size) => {
                    setPageSize(size);
                    setPage(1);
                }}
                noRecordsText="Aucun cours trouvé"
                minHeight={200}
            />

            {isModalOpen && (
                <CourseModal
                    course={selectedCourse}
                    onClose={() => {
                        setIsModalOpen(false);
                        setSelectedCourse(null);
                    }}
                    onSuccess={() => {
                        setIsModalOpen(false);
                        handleSuccess();
                    }}
                />
            )}
        </div>
    );
};
