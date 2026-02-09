import * as React from 'react';
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import { TextField, Box, Button } from '@mui/material';
import { UserDTO } from '../../../services/users/UserDTO';

interface Props {
    fetchUsers: (params: { page: number; pageSize: number; search: string }) => Promise<{ items: UserDTO[]; total: number }>;
    onDelete: (id: string) => void;
    isDeleting?: boolean;
}

export const UserDataGrid: React.FC<Props> = ({ fetchUsers, onDelete, isDeleting }) => {
    const [paginationModel, setPaginationModel] = React.useState({ page: 0, pageSize: 10 });
    const [search, setSearch] = React.useState('');
    const [rows, setRows] = React.useState<UserDTO[]>([]);
    const [rowCount, setRowCount] = React.useState(0);
    const [loading, setLoading] = React.useState(false);

    React.useEffect(() => {
        setLoading(true);
        fetchUsers({ page: paginationModel.page + 1, pageSize: paginationModel.pageSize, search })
            .then(data => {
                setRows(data.items);
                setRowCount(data.total);
                setLoading(false);
            })
            .catch(() => {
                setLoading(false);
            });
    }, [paginationModel, search, fetchUsers]);

    const columns: GridColDef[] = [
        {
            field: 'lastName',
            headerName: 'Nom',
            flex: 1,
            minWidth: 120
        },
        {
            field: 'firstName',
            headerName: 'Prénom',
            flex: 1,
            minWidth: 120
        },
        {
            field: 'email',
            headerName: 'Email',
            flex: 1.5,
            minWidth: 180
        },
        {
            field: 'companyName',
            headerName: 'Entreprise',
            flex: 1,
            minWidth: 120
        },
        {
            field: 'sessionId',
            headerName: 'Session ID',
            flex: 1,
            minWidth: 120
        },
        {
            field: 'actions',
            headerName: 'Actions',
            sortable: false,
            filterable: false,
            flex: 0.8,
            minWidth: 120,
            renderCell: (params: GridRenderCellParams) => (
                <Button
                    variant="contained"
                    color="primary"
                    size="small"
                    onClick={() => onDelete(params.row.id)}
                    disabled={isDeleting}
                >
                    {isDeleting ? 'Suppression...' : 'Éditer'}
                </Button>
            ),
        },
    ];

    return (
        <Box>
            <TextField
                label="Rechercher un utilisateur"
                variant="outlined"
                size="small"
                value={search}
                onChange={e => setSearch(e.target.value)}
                placeholder="Nom, email, entreprise..."
                style={{ marginBottom: 16 }}
                fullWidth
            />
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
            />
        </Box>
    );
};
