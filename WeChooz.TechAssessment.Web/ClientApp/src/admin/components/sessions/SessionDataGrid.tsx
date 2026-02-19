import * as React from 'react';
import { DataTable, type DataTableColumn } from 'mantine-datatable';
import { ActionIcon, Tooltip, Group, Badge, Text, Progress } from '@mantine/core';
import { IconTrash, IconEdit } from '@tabler/icons-react';
import { SessionDTO } from '../../../services/sessions/SessionDTO';
import { DeliveryMode, deliveryModeLabels } from '../../../services/sessions/DeliveryMode';
import { SessionModal } from './SessionModal';
import { useDeleteSession } from '../../../hooks/sessions/useDeleteSession';
import { TableToolbar } from '../common/TableToolbar';

interface Props {
    sessions: SessionDTO[];
    loading: boolean;
    onSuccess: () => void | Promise<void>;
}

const PAGE_SIZES = [10, 25, 50];

export const SessionDataGrid: React.FC<Props> = ({ sessions, loading, onSuccess }) => {
    const [page, setPage] = React.useState(1);
    const [pageSize, setPageSize] = React.useState(10);
    const [search, setSearch] = React.useState('');
    const [isDeleting, setDeleting] = React.useState(false);

    const { mutate: deleteSession } = useDeleteSession();

    const [selectedSession, setSelectedSession] = React.useState<SessionDTO | null>(null);
    const [isModalOpen, setIsModalOpen] = React.useState(false);

    // Filtrage et pagination locaux basés sur les props
    const filteredRecords = React.useMemo(() => {
        let filtered = [...sessions];
        if (search) {
            const searchLower = search.toLowerCase();
            filtered = filtered.filter(s =>
                s.courseName.toLowerCase().includes(searchLower) ||
                deliveryModeLabels[s.deliveryMode as DeliveryMode]?.toLowerCase().includes(searchLower)
            );
        }
        return filtered;
    }, [sessions, search]);

    const records = React.useMemo(() => {
        const start = (page - 1) * pageSize;
        const end = start + pageSize;
        return filteredRecords.slice(start, end);
    }, [filteredRecords, page, pageSize]);

    const handleSuccess = async () => {
        await onSuccess();
    };

    const handleOpenCreate = () => {
        setSelectedSession(null);
        setIsModalOpen(true);
    };

    const handleOpenEdit = (session: SessionDTO) => {
        setSelectedSession(session);
        setIsModalOpen(true);
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('fr-FR', {
            day: 'numeric',
            month: 'long',
            year: 'numeric'
        });
    };

    const getDeliveryModeLabel = (mode: string | number) => {
        const modeValue = typeof mode === 'string' ? Number(mode) : mode;
        return deliveryModeLabels[modeValue as DeliveryMode] || String(mode);
    };

    const getDeliveryModeColor = (mode: string | number): string => {
        const modeValue = typeof mode === 'string' ? Number(mode) : mode;
        switch (modeValue) {
            case DeliveryMode.InPerson: return 'blue';
            case DeliveryMode.Remote: return 'orange';
            default: return 'gray';
        }
    };

    const columns: DataTableColumn<SessionDTO>[] = [
        {
            accessor: 'courseName',
            title: 'Cours',
            width: '30%',
            render: (record) => <Text fw={500} size="sm">{record.courseName}</Text>
        },
        {
            accessor: 'startDate',
            title: 'Date de début',
            width: '20%',
            render: (record) => <Text size="sm">{formatDate(record.startDate)}</Text>,
        },
        {
            accessor: 'deliveryMode',
            title: 'Mode',
            width: '15%',
            render: (record) => (
                <Badge
                    color={getDeliveryModeColor(record.deliveryMode)}
                    variant="light"
                >
                    {getDeliveryModeLabel(record.deliveryMode)}
                </Badge>
            ),
        },
        {
            accessor: 'UserCount',
            title: 'Remplissage',
            width: '20%',
            render: (record) => {
                const max = record.course?.maxCapacity || 0;
                const enrolled = max - record.UserCount;
                const percentage = max > 0 ? (enrolled / max) * 100 : 0;

                return (
                    <Tooltip label={`${enrolled} / ${max} inscrits`}>
                        <div>
                            <Group justify="space-between" mb={4}>
                                <Text size="xs" c="dimmed">{enrolled}/{max}</Text>
                                <Text size="xs" fw={700}>{Math.round(percentage)}%</Text>
                            </Group>
                            <Progress 
                                value={percentage} 
                                size="sm" 
                                color={percentage >= 100 ? 'red' : percentage > 80 ? 'orange' : 'blue'} 
                            />
                        </div>
                    </Tooltip>
                );
            },
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
                                if (window.confirm(`Supprimer la session du ${formatDate(record.startDate)} ?`)) {
                                    setDeleting(true);
                                    deleteSession(record.id, {
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
                searchPlaceholder="Filtrer par cours..."
                addButtonLabel="Nouvelle session"
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
                noRecordsText="Aucune session trouvée"
                minHeight={200}
            />

            {isModalOpen && (
                <SessionModal
                    session={selectedSession}
                    onClose={() => {
                        setIsModalOpen(false);
                        setSelectedSession(null);
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
