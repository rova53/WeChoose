import * as React from 'react';
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import { TextField, Box, Button, Chip, IconButton, Tooltip } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import AddIcon from '@mui/icons-material/Add';
import { SessionDTO } from '../../../services/sessions/SessionDTO';
import { DeliveryMode, deliveryModeLabels } from '../../../services/sessions/DeliveryMode';
import { SessionModal } from './SessionModal'; // Ton modal corrigé précédemment
import { useDeleteSession } from '../../../hooks/sessions/useDeleteSession';

interface Props {
    fetchSessions: (params: { page: number; pageSize: number; search: string }) => Promise<{ items: SessionDTO[]; total: number }>;
    onSuccess: () => void;
}

export const SessionDataGrid: React.FC<Props> = ({ fetchSessions, onSuccess }) => {
    const [paginationModel, setPaginationModel] = React.useState({ page: 0, pageSize: 10 });
    const [search, setSearch] = React.useState('');
    const [rows, setRows] = React.useState<SessionDTO[]>([]);
    const [rowCount, setRowCount] = React.useState(0);
    const [loading, setLoading] = React.useState(false);
    const [isDeleting, setDeleting] = React.useState(false);

    const { mutate: deleteSession } = useDeleteSession();

    // États pour le modal
    const [selectedSession, setSelectedSession] = React.useState<SessionDTO | null>(null);
    const [isModalOpen, setIsModalOpen] = React.useState(false);

    const loadData = React.useCallback(() => {
        setLoading(true);
        fetchSessions({
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
                console.error("Erreur chargement grid sessions:", err);
                setLoading(false);
            });
    }, [paginationModel, search, fetchSessions]);

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

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('fr-FR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    };

    const getDeliveryModeLabel = (mode: string | number) => {
        const modeValue = typeof mode === 'string' ? Number(mode) : mode;
        return deliveryModeLabels[modeValue as DeliveryMode] || mode;
    };

    const getDeliveryModeColor = (mode: string | number): "primary" | "success" | "warning" | "default" => {
        const modeValue = typeof mode === 'string' ? Number(mode) : mode;
        switch (modeValue) {
            case DeliveryMode.InPerson: return 'success';
            case DeliveryMode.Remote: return 'warning';
            default: return 'default';
        }
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
            headerName: 'Mode',
            flex: 1,
            minWidth: 130,
            renderCell: (params: GridRenderCellParams) => (
                <Chip
                    label={getDeliveryModeLabel(params.value)}
                    color={getDeliveryModeColor(params.value)}
                    size="small"
                    variant="outlined"
                    sx={{ fontWeight: 500 }}
                />
            )
        },
        {
            field: 'UserCount',
            headerName: 'Inscrits',
            flex: 0.6,
            minWidth: 100,
            align: 'center',
            headerAlign: 'center',
            renderCell: (params) => (
                <Box sx={{ fontWeight: 'bold', color: 'text.secondary' }}>
                    {params.row.course.maxCapacity - params.row.userCount} inscrit(s)
                </Box>
            )
        },
        {
            field: 'actions',
            headerName: 'Actions',
            sortable: false,
            width: 120,
            renderCell: (params: GridRenderCellParams<SessionDTO>) => (
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
                                    if (window.confirm(`Supprimer la session du ${formatDate(params.row.startDate)} ?`)) {
                                        setDeleting(true);
                                        deleteSession(params.row.id, {
                                            onSuccess: () => {
                                                onSuccess();
                                                setDeleting(false);
                                            },
                                            onError: () => setDeleting(false)
                                        });
                                    }
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
                    label="Rechercher une session..."
                    variant="outlined"
                    size="small"
                    value={search}
                    onChange={e => setSearch(e.target.value)}
                    fullWidth
                    placeholder="Filtrer par cours ou mode..."
                />
                <Button
                    variant="contained"
                    onClick={handleOpenCreate}
                    startIcon={<AddIcon />}
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
                localeText={{ noRowsLabel: 'Aucune session trouvée' }}
                sx={{
                    '& .MuiDataGrid-cell:focus': { outline: 'none' },
                    borderRadius: 2,
                    overflow: 'hidden',
                    boxShadow: 1
                }}
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
                        onSuccess();
                    }}
                />
            )}
        </Box>
    );
};