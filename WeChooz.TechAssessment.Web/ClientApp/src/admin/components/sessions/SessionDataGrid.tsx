import * as React from 'react';
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import { TextField, Box, Button, Chip } from '@mui/material';
import { SessionDTO } from '../../../services/sessions/SessionDTO';

interface Props {
    fetchSessions: (params: { page: number; pageSize: number; search: string }) => Promise<{ items: SessionDTO[]; total: number }>;
    onDelete: (id: string) => void;
    isDeleting?: boolean;
}

export const SessionDataGrid: React.FC<Props> = ({ fetchSessions, onDelete, isDeleting }) => {
    const [paginationModel, setPaginationModel] = React.useState({ page: 0, pageSize: 10 });
    const [search, setSearch] = React.useState('');
    const [rows, setRows] = React.useState<SessionDTO[]>([]);
    const [rowCount, setRowCount] = React.useState(0);
    const [loading, setLoading] = React.useState(false);

    React.useEffect(() => {
        setLoading(true);
        fetchSessions({ page: paginationModel.page + 1, pageSize: paginationModel.pageSize, search })
            .then(data => {
                setRows(data.items);
                setRowCount(data.total);
                setLoading(false);
            })
            .catch(() => {
                setLoading(false);
            });
    }, [paginationModel, search, fetchSessions]);

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('fr-FR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    };

    const getDeliveryModeLabel = (mode: string | number) => {
        const modes: Record<string | number, string> = {
            0: 'En présentiel',
            1: 'À distance',
            2: 'Hybride',
            'InPerson': 'En présentiel',
            'Remote': 'À distance',
            'Hybrid': 'Hybride',
        };
        return modes[mode] || mode;
    };

    const getDeliveryModeColor = (mode: string | number) => {
        const colors: Record<string | number, 'primary' | 'success' | 'warning'> = {
            0: 'primary',
            1: 'success',
            2: 'warning',
            'InPerson': 'primary',
            'Remote': 'success',
            'Hybrid': 'warning',
        };
        return colors[mode] || 'default';
    };

    const columns: GridColDef[] = [
        {
            field: 'courseName',
            headerName: 'Cours',
            flex: 1.5,
            minWidth: 200
        },
        {
            field: 'startDate',
            headerName: 'Date de début',
            flex: 1,
            minWidth: 120,
            renderCell: (params: GridRenderCellParams) => formatDate(params.value as string)
        },
        {
            field: 'deliveryMode',
            headerName: 'Mode de diffusion',
            flex: 1,
            minWidth: 150,
            renderCell: (params: GridRenderCellParams) => (
                <Chip
                    label={getDeliveryModeLabel(params.value)}
                    color={getDeliveryModeColor(params.value)}
                    size="small"
                />
            )
        },
        {
            field: 'UserCount',
            headerName: 'Users',
            flex: 0.8,
            minWidth: 120,
            align: 'center',
            headerAlign: 'center'
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
                label="Rechercher une session"
                variant="outlined"
                size="small"
                value={search}
                onChange={e => setSearch(e.target.value)}
                placeholder="Nom du cours, mode..."
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