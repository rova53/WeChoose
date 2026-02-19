import React from 'react';
import { TextInput, Button, Group, ActionIcon, Tooltip, Badge, Text, Stack } from '@mantine/core';
import { DataTable } from 'mantine-datatable';
import { IconEdit, IconTrash, IconPlus, IconSearch } from '@tabler/icons-react';
import { SessionDTO } from '../../../services/sessions/SessionDTO';
import { DeliveryMode, deliveryModeLabels } from '../../../services/sessions/DeliveryMode';
import { SessionModal } from './SessionModal';
import { useDeleteSession } from '../../../hooks/sessions/useDeleteSession';

interface Props {
    fetchSessions: (params: { page: number; pageSize: number; search: string }) => Promise<{ items: SessionDTO[]; total: number }>;
    onSuccess: () => void;
    refreshKey: number;
}

export const SessionDataGrid: React.FC<Props> = ({ fetchSessions, onSuccess, refreshKey }) => {
    const [page, setPage] = React.useState(1);
    const [pageSize, setPageSize] = React.useState(10);
    const [search, setSearch] = React.useState('');
    const [rows, setRows] = React.useState<SessionDTO[]>([]);
    const [rowCount, setRowCount] = React.useState(0);
    const [loading, setLoading] = React.useState(false);
    const [isDeleting, setDeleting] = React.useState(false);
    const [forceLoadKey, setForceLoadKey] = React.useState(0);

    const { mutate: deleteSession } = useDeleteSession();

    const [selectedSession, setSelectedSession] = React.useState<SessionDTO | null>(null);
    const [isModalOpen, setIsModalOpen] = React.useState(false);

    React.useEffect(() => {
        if (refreshKey > 0) {
            setPage(1);
            setForceLoadKey(k => k + 1);
        }
    }, [refreshKey]);

    const loadData = React.useCallback(() => {
        setLoading(true);
        fetchSessions({ page, pageSize, search })
            .then(data => {
                setRows([...data.items].sort((a, b) => b.id.localeCompare(a.id)));
                setRowCount(data.total);
                setLoading(false);
            })
            .catch(err => {
                console.error('Erreur chargement grid sessions:', err);
                setLoading(false);
            });
    }, [page, pageSize, search, fetchSessions, forceLoadKey]);

    React.useEffect(() => {
        loadData();
    }, [loadData]);

    const handleOpenCreate = () => {
        setSelectedSession(null);
        setIsModalOpen(true);
    };

    const handleOpenEdit = (session: SessionDTO) => {
        setSelectedSession(session);
        setIsModalOpen(true);
    };

    const formatDate = (dateString: string) =>
        new Date(dateString).toLocaleDateString('fr-FR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
        });

    const getDeliveryModeColor = (mode: string | number): string => {
        const modeValue = typeof mode === 'string' ? Number(mode) : mode;
        switch (modeValue) {
            case DeliveryMode.InPerson: return 'green';
            case DeliveryMode.Remote: return 'yellow';
            default: return 'gray';
        }
    };

    return (
        <Stack gap="sm">
            <Group>
                <TextInput
                    leftSection={<IconSearch size={16} />}
                    placeholder="Filtrer par cours ou mode..."
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
                noRecordsText="Aucune session trouvée"
                columns={[
                    {
                        accessor: 'courseName',
                        title: 'Cours',
                    },
                    {
                        accessor: 'startDate',
                        title: 'Date de début',
                        render: (session) => formatDate(session.startDate),
                    },
                    {
                        accessor: 'deliveryMode',
                        title: 'Mode',
                        render: (session) => {
                            const modeValue = typeof session.deliveryMode === 'string'
                                ? Number(session.deliveryMode)
                                : session.deliveryMode;
                            return (
                                <Badge variant="outline" color={getDeliveryModeColor(session.deliveryMode)} fw={500}>
                                    {deliveryModeLabels[modeValue as DeliveryMode] ?? session.deliveryMode}
                                </Badge>
                            );
                        },
                    },
                    {
                        accessor: 'userCount',
                        title: 'Inscrits',
                        textAlign: 'center',
                        render: (session) => (
                            <Text fw={700} c="dimmed" ta="center">
                                {session.course.maxCapacity - session.UserCount} inscrit(s)
                            </Text>
                        ),
                    },
                    {
                        accessor: 'actions',
                        title: 'Actions',
                        render: (session) => (
                            <Group gap={4} wrap="nowrap">
                                <Tooltip label="Modifier">
                                    <ActionIcon variant="subtle" color="blue" onClick={() => handleOpenEdit(session)}>
                                        <IconEdit size={16} />
                                    </ActionIcon>
                                </Tooltip>
                                <Tooltip label="Supprimer">
                                    <ActionIcon
                                        variant="subtle"
                                        color="red"
                                        disabled={isDeleting}
                                        onClick={() => {
                                            if (window.confirm(`Supprimer la session du ${formatDate(session.startDate)} ?`)) {
                                                setDeleting(true);
                                                deleteSession(session.id, {
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
                <SessionModal
                    session={selectedSession}
                    onClose={() => { setIsModalOpen(false); setSelectedSession(null); }}
                    onSuccess={() => { console.log('onSuccess'); setIsModalOpen(false); onSuccess(); }}
                />
            )}
        </Stack>
    );
};
