import * as React from 'react';
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import { TextField, Box, Button } from '@mui/material';
import { CourseDTO } from '../../../services/courses/CourseDTO';

interface Props {
    fetchCourses: (params: { page: number; pageSize: number; search: string }) => Promise<{ items: CourseDTO[]; total: number }>;
    onDelete: (id: string) => void;
    isDeleting?: boolean;
}

export const CourseDataGrid: React.FC<Props> = ({ fetchCourses, onDelete, isDeleting }) => {
    const [paginationModel, setPaginationModel] = React.useState({ page: 0, pageSize: 10 });
    const [search, setSearch] = React.useState('');
    const [rows, setRows] = React.useState<CourseDTO[]>([]);
    const [rowCount, setRowCount] = React.useState(0);
    const [loading, setLoading] = React.useState(false);

    React.useEffect(() => {
        setLoading(true);
        fetchCourses({ page: paginationModel.page + 1, pageSize: paginationModel.pageSize, search }).then(data => {
            setRows(data.items);
            setRowCount(data.total);
            setLoading(false);
        });
    }, [paginationModel, search, fetchCourses]);

    const columns: GridColDef[] = [
        { field: 'name', headerName: 'Titre', flex: 1 },
        { field: 'targetAudience', headerName: 'Population cible', flex: 1 },
        { field: 'maxCapacity', headerName: 'Population min', flex: 1 },
        {
            field: 'actions',
            headerName: 'Actions',
            sortable: false,
            filterable: false,
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
                label="Recherche"
                variant="outlined"
                size="small"
                value={search}
                onChange={e => setSearch(e.target.value)}
                style={{ marginBottom: 16 }}
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
            />
        </Box>
    );
};