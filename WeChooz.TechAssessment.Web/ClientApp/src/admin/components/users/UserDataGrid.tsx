import * as React from 'react';
import { DataTable, type DataTableColumn } from 'mantine-datatable';
import { ActionIcon, Tooltip, Group, Text } from '@mantine/core';
import { IconTrash, IconEdit } from '@tabler/icons-react';
import { UserDTO } from '../../../services/users/UserDTO';
import { UserModal } from './UserModal';
import { useDeleteUser } from "../../../hooks/users/useDeleteUser.ts";
import { TableToolbar } from '../common/TableToolbar';

interface Props {
    users: UserDTO[];
    loading: boolean;
    onSuccess: () => void | Promise<void>
}

const PAGE_SIZES = [10, 25, 50];

export const UserDataGrid: React.FC<Props> = ({ users, loading, onSuccess }) => {
    const [page, setPage] = React.useState(1);
    const [pageSize, setPageSize] = React.useState(10);
    const [search, setSearch] = React.useState('');
    const [isDeleting, setDeleting] = React.useState(false);
    const { mutate: deleteUser } = useDeleteUser();

    const [selectedUser, setSelectedUser] = React.useState<UserDTO | null>(null);
    const [isModalOpen, setIsModalOpen] = React.useState(false);

    const filteredRecords = React.useMemo(() => {
        let filtered = [...users];
        if (search) {
            const searchLower = search.toLowerCase();
            filtered = filtered.filter(u =>
                u.lastName.toLowerCase().includes(searchLower) ||
                u.firstName.toLowerCase().includes(searchLower) ||
                u.email.toLowerCase().includes(searchLower) ||
                (u.companyName || '').toLowerCase().includes(searchLower)
            );
        }
        return filtered;
    }, [users, search]);

    const records = React.useMemo(() => {
        const start = (page - 1) * pageSize;
        const end = start + pageSize;
        return filteredRecords.slice(start, end);
    }, [filteredRecords, page, pageSize]);

    const handleSuccess = async () => {
        await onSuccess();
    };

    const handleOpenCreate = () => {
        setSelectedUser(null);
        setIsModalOpen(true);
    };

    const handleOpenEdit = (user: UserDTO) => {
        setSelectedUser(user);
        setIsModalOpen(true);
    };

    const columns: DataTableColumn<UserDTO>[] = [
        { accessor: 'lastName', title: 'Nom', width: '20%', sortable: true },
        { accessor: 'firstName', title: 'Prénom', width: '20%', sortable: true },
        { accessor: 'email', title: 'Email', width: '30%' },
        { 
            accessor: 'companyName', 
            title: 'Entreprise', 
            width: '20%',
            render: (record) => record.companyName ? <Text size="sm">{record.companyName}</Text> : <Text size="sm" c="dimmed">-</Text>
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
                                if (window.confirm(`Supprimer l'utilisateur ${record.firstName} ?`)) {
                                    setDeleting(true);
                                    deleteUser(record.id, {
                                        onSuccess: () => {
                                            handleSuccess();
                                            setDeleting(false);
                                        },
                                        onError: (error: unknown) => {
                                            console.error(error);
                                            setDeleting(false);
                                        }
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
                searchPlaceholder="Filtrer par nom, email..."
                addButtonLabel="Ajouter un utilisateur"
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
                noRecordsText="Aucun utilisateur trouvé"
                minHeight={200}
            />

            {isModalOpen && (
                <UserModal
                    user={selectedUser}
                    onClose={() => {
                        setIsModalOpen(false);
                        setSelectedUser(null);
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
