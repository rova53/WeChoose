import * as React from 'react';
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import { TextField, Box, Button, IconButton, Tooltip } from '@mui/material';
import { UserDTO } from '../../../services/users/UserDTO';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import { UserModal } from './UserModal';
import {useUpdateUser} from "../../../hooks/users/useUpdateUser.ts";
import {useDeleteUser} from "../../../hooks/users/useDeleteUser.ts";

interface Props {
    fetchUsers: (params: { page: number; pageSize: number; search: string }) => Promise<{ items: UserDTO[]; total: number }>;
    onSuccess: () => void
}

export const UserDataGrid: React.FC<Props> = ({ fetchUsers, onSuccess }) => {
    const [paginationModel, setPaginationModel] = React.useState({ page: 0, pageSize: 10 });
    const [search, setSearch] = React.useState('');
    const [rows, setRows] = React.useState<UserDTO[]>([]);
    const [rowCount, setRowCount] = React.useState(0);
    const [loading, setLoading] = React.useState(false);
    const [isDeleting, setDeleting] = React.useState(false);
    const { mutate: deleteUser } = useDeleteUser();

    // États pour le modal
    const [selectedUser, setSelectedUser] = React.useState<UserDTO | null>(null);
    const [isModalOpen, setIsModalOpen] = React.useState(false);


    const loadData = React.useCallback(() => {
        setLoading(true);
        fetchUsers({
            page: paginationModel.page + 1,
            pageSize: paginationModel.pageSize,
            search
        })
            .then(data => {
                setRows(data.items);
                setRowCount(data.total);
                setLoading(false);
            })
            .catch((err) => {
                console.error("Erreur chargement grid:", err);
                setLoading(false);
            });
    }, [paginationModel, search, fetchUsers]);

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

    const columns: GridColDef[] = [
        { field: 'lastName', headerName: 'Nom', flex: 1, minWidth: 120 },
        { field: 'firstName', headerName: 'Prénom', flex: 1, minWidth: 120 },
        { field: 'email', headerName: 'Email', flex: 1.5, minWidth: 180 },
        {
            field: 'actions',
            headerName: 'Actions',
            sortable: false,
            width: 120,
            renderCell: (params: GridRenderCellParams<UserDTO>) => (
                <Box sx={{ display: 'flex', gap: 1, alignItems: 'center', height: '100%' }}>
                    <Tooltip title="Modifier">
                        <IconButton
                            color="primary"
                            size="small"
                            onClick={() => handleOpenEdit(params.row)}
                        >
                            <EditIcon fontSize="small" />
                        </IconButton>
                    </Tooltip>

                    <Tooltip title="Supprimer">
                        <span>
                            <IconButton
                                color="error"
                                size="small"
                                disabled={isDeleting}
                                onClick={() => {
                                    setDeleting(true);
                                    if (window.confirm(`Supprimer l'utilisateur ${params.row.firstName} ?`)) {
                                        console.log("params.row", params.row);
                                        deleteUser(params.row.id, {
                                            onSuccess: () => {
                                                onSuccess();  // ✅ Appelé seulement après succès
                                                setDeleting(false);
                                            },
                                            onError: (error) => {
                                                console.error(error);
                                                setDeleting(false);  // ✅ Important de gérer l'erreur aussi
                                            }
                                        });
                                    }else
                                        setDeleting(false);
                                }}
                            >
                                <DeleteIcon fontSize="small" />
                            </IconButton>
                        </span>
                    </Tooltip>
                </Box>
            ),
        },
    ];

    return (
        <Box>
            <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
                <TextField
                    label="Rechercher..."
                    variant="outlined"
                    size="small"
                    value={search}
                    onChange={e => setSearch(e.target.value)}
                    fullWidth
                    placeholder="Filtrer par nom, prénom, email..."
                />
                <Button
                    variant="contained"
                    onClick={handleOpenCreate}
                    startIcon={<EditIcon />}
                >
                    Ajouter
                </Button>
            </Box>

            <DataGrid
                rows={rows}
                columns={columns}
                pagination
                paginationMode="server"
                rowCount={rowCount}
                paginationModel={paginationModel}
                onPaginationModelChange={setPaginationModel}
                loading={loading}
                autoHeight
                pageSizeOptions={[5, 10, 25, 50]}
                disableRowSelectionOnClick
                localeText={{ noRowsLabel: 'Aucun utilisateur trouvé' }}
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
                        onSuccess(); 
                    }}
                />
            )}
        </Box>
    );
};