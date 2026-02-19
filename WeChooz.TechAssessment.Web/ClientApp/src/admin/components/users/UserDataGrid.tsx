import React from 'react';
import { TextInput, Button, Group, ActionIcon, Tooltip, Stack } from '@mantine/core';
import { DataTable } from 'mantine-datatable';
import { IconEdit, IconTrash, IconPlus, IconSearch } from '@tabler/icons-react';
import { UserDTO } from '../../../services/users/UserDTO';
import { UserModal } from './UserModal';
import { useDeleteUser } from '../../../hooks/users/useDeleteUser';

interface Props {
    fetchUsers: (params: { page: number; pageSize: number; search: string }) => Promise<{ items: UserDTO[]; total: number }>;
    onSuccess: () => void;
    refreshKey: number;
}

export const UserDataGrid: React.FC<Props> = ({ fetchUsers, onSuccess, refreshKey }) => {
    const [page, setPage] = React.useState(1);
    const [pageSize, setPageSize] = React.useState(10);
    const [search, setSearch] = React.useState('');
    const [rows, setRows] = React.useState<UserDTO[]>([]);
    const [rowCount, setRowCount] = React.useState(0);
    const [loading, setLoading] = React.useState(false);
    const [isDeleting, setDeleting] = React.useState(false);
    const [forceLoadKey, setForceLoadKey] = React.useState(0);

    const { mutate: deleteUser } = useDeleteUser();

    const [selectedUser, setSelectedUser] = React.useState<UserDTO | null>(null);
    const [isModalOpen, setIsModalOpen] = React.useState(false);

    React.useEffect(() => {
        if (refreshKey > 0) {
            setPage(1);
            setForceLoadKey(k => k + 1);
        }
    }, [refreshKey]);

    const loadData = React.useCallback(() => {
        setLoading(true);
        fetchUsers({ page, pageSize, search })
            .then(data => {
                setRows([...data.items].sort((a, b) => b.id.localeCompare(a.id)));
                setRowCount(data.total);
                setLoading(false);
            })
            .catch(err => {
                console.error('Erreur chargement grid:', err);
                setLoading(false);
            });
    }, [page, pageSize, search, fetchUsers, forceLoadKey]);

    React.useEffect(() => {
        loadData();
    }, [loadData]);

    const handleOpenCreate = () => {
        setSelectedUser(null);
        setIsModalOpen(true);
    };

    const handleOpenEdit = (user: UserDTO) => {
        setSelectedUser(user);
        setIsModalOpen(true);
    };

    return (
        <Stack gap="sm">
            <Group>
                <TextInput
                    leftSection={<IconSearch size={16} />}
                    placeholder="Filtrer par nom, prénom, email..."
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
                noRecordsText="Aucun utilisateur trouvé"
                columns={[
                    { accessor: 'lastName', title: 'Nom' },
                    { accessor: 'firstName', title: 'Prénom' },
                    { accessor: 'email', title: 'Email' },
                    {
                        accessor: 'actions',
                        title: 'Actions',
                        render: (user) => (
                            <Group gap={4} wrap="nowrap">
                                <Tooltip label="Modifier">
                                    <ActionIcon variant="subtle" color="blue" onClick={() => handleOpenEdit(user)}>
                                        <IconEdit size={16} />
                                    </ActionIcon>
                                </Tooltip>
                                <Tooltip label="Supprimer">
                                    <ActionIcon
                                        variant="subtle"
                                        color="red"
                                        disabled={isDeleting}
                                        onClick={() => {
                                            if (window.confirm(`Supprimer l'utilisateur ${user.firstName} ?`)) {
                                                setDeleting(true);
                                                deleteUser(user.id, {
                                                    onSuccess: () => { onSuccess(); setDeleting(false); },
                                                    onError: (error) => { console.error(error); setDeleting(false); },
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
                <UserModal
                    user={selectedUser}
                    onClose={() => { setIsModalOpen(false); setSelectedUser(null); }}
                    onSuccess={() => { setIsModalOpen(false); onSuccess(); }}
                />
            )}
        </Stack>
    );
};
