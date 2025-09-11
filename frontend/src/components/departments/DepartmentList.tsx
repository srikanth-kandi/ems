import { useEffect, useState } from 'react';
import { api } from '../../lib/api';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { 
  Box, 
  Button, 
  Dialog, 
  DialogActions, 
  DialogContent, 
  DialogTitle, 
  Stack, 
  TextField, 
  Typography,
  Card,
  CardContent,
  Chip,
  IconButton,
  Tooltip,
  Paper,
  Alert,
  Snackbar,
  Menu,
  MenuItem,
  ListItemIcon,
  ListItemText,
  Divider,
  Fade,
  Zoom,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Download as DownloadIcon,
  MoreVert as MoreVertIcon,
  Search as SearchIcon,
  Refresh as RefreshIcon,
  Business as BusinessIcon,
  People as PeopleIcon,
  PictureAsPdf as PdfIcon,
  TableChart as ExcelIcon,
  FileDownload as CsvIcon,
} from '@mui/icons-material';

type Department = {
  id: number;
  name: string;
  description?: string;
  managerName?: string;
  employeeCount?: number;
};

export default function DepartmentList() {
  const [rows, setRows] = useState<Department[]>([]);
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<any>({ 
    name: '', 
    description: '', 
    managerName: ''
  });
  const [editingId, setEditingId] = useState<number | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  const load = async () => {
    setLoading(true);
    try {
      const data = await api.getDepartments();
      setRows(data);
    } catch (error) {
      showSnackbar('Error loading departments', 'error');
    } finally {
      setLoading(false);
    }
  };

  const showSnackbar = (message: string, severity: 'success' | 'error' = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleDownload = async (format: 'pdf' | 'excel' | 'csv') => {
    try {
      const filename = `departments.${format === 'csv' ? 'csv' : format === 'pdf' ? 'pdf' : 'xlsx'}`;
      const endpoint = format === 'csv' ? 'departments' : `departments/${format}`;
      await api.downloadReport(endpoint, filename);
      showSnackbar(`${filename} downloaded successfully`);
    } catch (error) {
      showSnackbar('Error downloading report', 'error');
    }
  };

  const handleMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  useEffect(() => { 
    load(); 
  }, []);

  const columns: GridColDef[] = [
    { 
      field: 'id', 
      headerName: 'ID', 
      width: 70,
      headerAlign: 'center',
      align: 'center',
    },
    { 
      field: 'name', 
      headerName: 'Department Name', 
      flex: 1,
      minWidth: 200,
      renderCell: (params) => (
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <BusinessIcon color="primary" fontSize="small" />
          <Typography variant="body2" fontWeight="medium">
            {params.value}
          </Typography>
        </Box>
      )
    },
    { 
      field: 'managerName', 
      headerName: 'Manager', 
      flex: 1,
      minWidth: 150,
      renderCell: (params) => (
        <Typography variant="body2" color="text.secondary">
          {params.value || 'Not assigned'}
        </Typography>
      )
    },
    { 
      field: 'employeeCount', 
      headerName: 'Employees', 
      width: 120,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => (
        <Chip 
          label={params.value || 0} 
          color="primary" 
          variant="outlined"
          size="small"
          icon={<PeopleIcon />}
        />
      )
    },
    { 
      field: 'description', 
      headerName: 'Description', 
      flex: 1.5,
      minWidth: 200,
      renderCell: (params) => (
        <Typography 
          variant="body2" 
          color="text.secondary"
          sx={{ 
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
            maxWidth: 200
          }}
        >
          {params.value || 'No description'}
        </Typography>
      )
    },
    {
      field: 'actions', 
      headerName: 'Actions', 
      width: 120, 
      sortable: false,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => (
        <Stack direction="row" spacing={0.5}>
          <Tooltip title="Edit">
            <IconButton 
              size="small" 
              color="primary"
              onClick={() => onEdit(params.row)}
              sx={{ 
                bgcolor: 'primary.50',
                '&:hover': { bgcolor: 'primary.100' }
              }}
            >
              <EditIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Delete">
            <IconButton 
              size="small" 
              color="error"
              onClick={() => onDelete(params.row.id)}
              sx={{ 
                bgcolor: 'error.50',
                '&:hover': { bgcolor: 'error.100' }
              }}
            >
              <DeleteIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Stack>
      )
    }
  ];

  const onEdit = (row: Department) => {
    setEditingId(row.id);
    setForm({ 
      name: row.name,
      description: row.description || '',
      managerName: row.managerName || ''
    });
    setOpen(true);
  };

  const onCreate = () => {
    setEditingId(null);
    setForm({ 
      name: '', 
      description: '', 
      managerName: ''
    });
    setOpen(true);
  };

  const onSave = async () => {
    try {
      if (editingId) {
        await api.updateDepartment(editingId, form);
        showSnackbar('Department updated successfully');
      } else {
        await api.createDepartment(form);
        showSnackbar('Department created successfully');
      }
      setOpen(false);
      await load();
    } catch (error) {
      showSnackbar('Error saving department', 'error');
    }
  };

  const onDelete = async (id: number) => {
    try {
      await api.deleteDepartment(id);
      showSnackbar('Department deleted successfully');
      await load();
    } catch (error) {
      showSnackbar('Error deleting department', 'error');
    }
  };

  const filteredRows = rows.filter(row => 
    !searchTerm || 
    row.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (row.managerName && row.managerName.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (row.description && row.description.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  return (
    <Box sx={{ p: { xs: 2, md: 3 } }}>
      {/* Header Section */}
      <Fade in timeout={600}>
        <Card 
          sx={{ 
            mb: 3, 
            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            color: 'white',
            boxShadow: '0 8px 32px rgba(102, 126, 234, 0.3)',
          }}
        >
          <CardContent sx={{ p: 3 }}>
            <Stack 
              direction={{ xs: 'column', md: 'row' }} 
              justifyContent="space-between" 
              alignItems={{ xs: 'stretch', md: 'center' }}
              spacing={2}
            >
              <Box>
                <Typography 
                  variant="h4" 
                  fontWeight="700" 
                  sx={{ 
                    mb: 1,
                    fontFamily: 'Montserrat, Roboto, Arial',
                    textShadow: '0 2px 4px rgba(0,0,0,0.1)',
                  }}
                >
                  🏢 Department Management
                </Typography>
                <Typography variant="body1" sx={{ opacity: 0.9 }}>
                  Organize your workforce with structured department management
                </Typography>
              </Box>
              <Stack direction="row" spacing={2}>
                <Button
                  variant="contained"
                  startIcon={<AddIcon />}
                  onClick={onCreate}
                  sx={{
                    bgcolor: 'rgba(255,255,255,0.2)',
                    color: 'white',
                    backdropFilter: 'blur(10px)',
                    border: '1px solid rgba(255,255,255,0.3)',
                    '&:hover': {
                      bgcolor: 'rgba(255,255,255,0.3)',
                      transform: 'translateY(-2px)',
                      boxShadow: '0 4px 12px rgba(0,0,0,0.2)',
                    },
                    transition: 'all 0.3s ease',
                  }}
                >
                  Add Department
                </Button>
                <IconButton
                  onClick={handleMenuClick}
                  sx={{
                    bgcolor: 'rgba(255,255,255,0.2)',
                    color: 'white',
                    backdropFilter: 'blur(10px)',
                    border: '1px solid rgba(255,255,255,0.3)',
                    '&:hover': {
                      bgcolor: 'rgba(255,255,255,0.3)',
                      transform: 'translateY(-2px)',
                    },
                    transition: 'all 0.3s ease',
                  }}
                >
                  <MoreVertIcon />
                </IconButton>
              </Stack>
            </Stack>
          </CardContent>
        </Card>
      </Fade>

      {/* Search Section */}
      <Zoom in timeout={800}>
        <Card sx={{ mb: 3, boxShadow: '0 4px 20px rgba(0,0,0,0.08)' }}>
          <CardContent sx={{ p: 2 }}>
            <Stack 
              direction={{ xs: 'column', sm: 'row' }} 
              spacing={2} 
              alignItems="center"
            >
              <TextField
                placeholder="Search departments..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                InputProps={{
                  startAdornment: (
                    <SearchIcon color="action" />
                  ),
                }}
                sx={{ flexGrow: 1, minWidth: 200 }}
                size="small"
              />
              <IconButton 
                onClick={load}
                sx={{ 
                  bgcolor: 'primary.50',
                  '&:hover': { bgcolor: 'primary.100' }
                }}
              >
                <RefreshIcon />
              </IconButton>
            </Stack>
          </CardContent>
        </Card>
      </Zoom>

      {/* Data Grid */}
      <Zoom in timeout={1000}>
        <Paper 
          sx={{ 
            boxShadow: '0 4px 20px rgba(0,0,0,0.08)',
            borderRadius: 2,
            overflow: 'hidden',
          }}
        >
          <Box sx={{ height: 600, width: '100%' }}>
            <DataGrid 
              rows={filteredRows} 
              columns={columns} 
              loading={loading}
              pageSizeOptions={[10, 25, 50]}
              initialState={{ 
                pagination: { 
                  paginationModel: { pageSize: 10 } 
                } 
              }}
              sx={{
                '& .MuiDataGrid-root': {
                  border: 'none',
                },
                '& .MuiDataGrid-columnHeaders': {
                  background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
                  color: '#2c3e50',
                  fontWeight: 600,
                },
                '& .MuiDataGrid-row': {
                  '&:nth-of-type(even)': {
                    bgcolor: 'rgba(0,0,0,0.02)',
                  },
                  '&:hover': {
                    bgcolor: 'rgba(102, 126, 234, 0.08)',
                  },
                },
                '& .MuiDataGrid-cell': {
                  borderBottom: '1px solid rgba(224, 224, 224, 0.5)',
                },
              }}
            />
          </Box>
        </Paper>
      </Zoom>

      {/* Download Menu */}
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
        PaperProps={{
          sx: {
            mt: 1,
            minWidth: 200,
            boxShadow: '0 8px 32px rgba(0,0,0,0.12)',
            borderRadius: 2,
          }
        }}
      >
        <MenuItem onClick={() => { handleDownload('pdf'); handleMenuClose(); }}>
          <ListItemIcon><PdfIcon color="error" /></ListItemIcon>
          <ListItemText>Download PDF</ListItemText>
        </MenuItem>
        <MenuItem onClick={() => { handleDownload('excel'); handleMenuClose(); }}>
          <ListItemIcon><ExcelIcon color="success" /></ListItemIcon>
          <ListItemText>Download Excel</ListItemText>
        </MenuItem>
        <MenuItem onClick={() => { handleDownload('csv'); handleMenuClose(); }}>
          <ListItemIcon><CsvIcon color="primary" /></ListItemIcon>
          <ListItemText>Download CSV</ListItemText>
        </MenuItem>
      </Menu>

      {/* Department Form Dialog */}
      <Dialog 
        open={open} 
        onClose={() => setOpen(false)} 
        fullWidth 
        maxWidth="sm"
        PaperProps={{
          sx: {
            borderRadius: 3,
            boxShadow: '0 8px 32px rgba(0,0,0,0.12)',
          }
        }}
      >
        <DialogTitle sx={{ 
          background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
          color: 'white',
          fontWeight: 600,
        }}>
          {editingId ? 'Edit Department' : 'Add New Department'}
        </DialogTitle>
        <DialogContent sx={{ p: 3 }}>
          <Stack spacing={3} sx={{ mt: 1 }}>
            <TextField 
              label="Department Name" 
              value={form.name} 
              onChange={(e) => setForm({ ...form, name: e.target.value })}
              fullWidth
              required
            />
            <TextField 
              label="Manager Name" 
              value={form.managerName} 
              onChange={(e) => setForm({ ...form, managerName: e.target.value })}
              fullWidth
            />
            <TextField 
              label="Description" 
              value={form.description} 
              onChange={(e) => setForm({ ...form, description: e.target.value })}
              fullWidth
              multiline
              rows={3}
            />
          </Stack>
        </DialogContent>
        <DialogActions sx={{ p: 3, pt: 0 }}>
          <Button 
            onClick={() => setOpen(false)}
            sx={{ 
              color: 'text.secondary',
              '&:hover': { bgcolor: 'grey.100' }
            }}
          >
            Cancel
          </Button>
          <Button 
            variant="contained" 
            onClick={onSave}
            sx={{
              background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
              '&:hover': {
                background: 'linear-gradient(135deg, #5a6fd8 0%, #6a4190 100%)',
                transform: 'translateY(-1px)',
                boxShadow: '0 4px 12px rgba(102, 126, 234, 0.4)',
              },
              transition: 'all 0.3s ease',
            }}
          >
            {editingId ? 'Update' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert
          onClose={() => setSnackbar({ ...snackbar, open: false })}
          severity={snackbar.severity}
          sx={{ borderRadius: 2 }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
