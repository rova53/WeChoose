import * as React from 'react';
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import { TextField, Box, Button, IconButton, Tooltip } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import AddIcon from '@mui/icons-material/Add';
import { CourseDTO } from '../../../services/courses/CourseDTO';
import { CourseModal } from './CourseModal';
import { useDeleteCourse } from '../../../hooks/courses/useDeleteCourse';
import {TargetAudience, targetAudienceLabels} from "../../../services/courses/TargetAudience.ts";

interface Props {
    fetchCourses: (params: { page: number; pageSize: number; search: string }) => Promise<{ items: CourseDTO[]; total: number }>;
    onSuccess: () => void;
}

export const CourseDataGrid: React.FC<Props> = ({ fetchCourses, onSuccess }) => {
    const [paginationModel, setPaginationModel] = React.useState({ page: 0, pageSize: 10 });
    const [search, setSearch] = React.useState('');
    const [rows, setRows] = React.useState<CourseDTO[]>([]);
    const [rowCount, setRowCount] = React.useState(0);
    const [loading, setLoading] = React.useState(false);
    const [isDeleting, setDeleting] = React.useState(false);

    const { mutate: deleteCourse } = useDeleteCourse();
    
    const [selectedCourse, setSelectedCourse] = React.useState<CourseDTO | null>(null);
    const [isModalOpen, setIsModalOpen] = React.useState(false);

    const loadData = React.useCallback(() => {
        setLoading(true);
        fetchCourses({
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
                console.error("Erreur chargement grid cours:", err);
                setLoading(false);
            });
    }, [paginationModel, search, fetchCourses]);

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

    const columns: GridColDef[] = [
        {
            field: 'name',
            headerName: 'Titre du Cours',
            flex: 1.5,
            minWidth: 200,
            renderCell: (params) => (
                <Box sx={{ fontWeight: 600, color: 'primary.main' }}>
                    {params.value}
                </Box>
            )
        },
        {
            field: 'targetAudience',
            headerName: 'Population cible',
            flex: 1,
            minWidth: 150,
            renderCell: (params: GridRenderCellParams) => {
                const audienceValue = params.value as TargetAudience;

                return (
                    <span>
                {targetAudienceLabels[audienceValue]}
            </span>
                );
            }
        },
        {
            field: 'maxCapacity',
            headerName: 'Capacité Max',
            flex: 0.8,
            minWidth: 120,
            align: 'center',
            headerAlign: 'center',
            renderCell: (params) => (
                <Box sx={{ bgcolor: 'action.hover', px: 1, borderRadius: 1, fontWeight: 'bold' }}>
                    {params.value}
                </Box>
            )
        },
        {
            field: 'actions',
            headerName: 'Actions',
            sortable: false,
            width: 120,
            headerAlign: 'right',
            align: 'right',
            renderCell: (params: GridRenderCellParams<CourseDTO>) => (
                <Box sx={{ display: 'flex', gap: 1, justifyContent: 'flex-end', width: '100%' }}>
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
                                    if (window.confirm(`Supprimer le cours "${params.row.name}" ?`)) {
                                        setDeleting(true);
                                        deleteCourse(params.row.id, {
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
                    label="Rechercher un cours..."
                    variant="outlined"
                    size="small"
                    value={search}
                    onChange={e => setSearch(e.target.value)}
                    fullWidth
                    placeholder="Titre, population cible..."
                />
                <Button
                    variant="contained"
                    onClick={handleOpenCreate}
                    startIcon={<AddIcon />}
                    sx={{ whiteSpace: 'nowrap' }}
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
                localeText={{ noRowsLabel: 'Aucun cours trouvé' }}
                sx={{
                    '& .MuiDataGrid-cell:focus': { outline: 'none' },
                    borderRadius: 2,
                    overflow: 'hidden',
                    boxShadow: 1,
                    border: '1px solid',
                    borderColor: 'divider'
                }}
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
                        onSuccess();
                    }}
                />
            )}
        </Box>
    );
};