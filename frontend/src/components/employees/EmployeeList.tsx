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
  FormControl,
  InputLabel,
  Select,
  InputAdornment,
  Fade,
  Zoom,
} from '@mui/material';
import { convertUtcToLocalDate, formatDateForInput } from '../../utils/timezone';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  MoreVert as MoreVertIcon,
  Search as SearchIcon,
  PictureAsPdf as PdfIcon,
  TableChart as ExcelIcon,
  FileDownload as CsvIcon,
  Refresh as RefreshIcon,
} from '@mui/icons-material';

type Employee = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  dateOfBirth: string;
  dateOfJoining: string;
  position?: string;
  salary: number;
  departmentId: number;
  departmentName: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
};

type Department = {
  id: number;
  name: string;
};

export default function EmployeeList() {
  const [rows, setRows] = useState<Employee[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<any>({ 
    firstName: '', 
    lastName: '', 
    email: '', 
    phoneNumber: '',
    address: '',
    dateOfBirth: new Date().toISOString().split('T')[0],
    dateOfJoining: new Date().toISOString().split('T')[0],
    position: '',
    salary: 0,
    departmentId: 1
  });
  const [editingId, setEditingId] = useState<number | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedDepartment, setSelectedDepartment] = useState<number | ''>('');
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  const load = async () => {
    setLoading(true);
    try {
    const data = await api.getEmployees();
    setRows(data);
    } catch (error) {
      showSnackbar('Error loading employees', 'error');
    } finally {
    setLoading(false);
    }
  };

  const loadDepartments = async () => {
    try {
      const data = await api.getDepartments();
      setDepartments(data);
    } catch (error) {
      showSnackbar('Error loading departments', 'error');
    }
  };

  const showSnackbar = (message: string, severity: 'success' | 'error' = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleDownload = async (format: 'pdf' | 'excel' | 'csv') => {
    try {
      if (format === 'csv') {
        await api.downloadEmployeeReportCsv();
      } else if (format === 'pdf') {
        await api.downloadEmployeeReportPdf();
      } else if (format === 'excel') {
        await api.downloadEmployeeReportExcel();
      }
      showSnackbar(`Employee report downloaded successfully`);
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
    loadDepartments();
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
      field: 'firstName', 
      headerName: 'First Name', 
      flex: 1,
      minWidth: 120,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => (
        <Box sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          width: '100%'
        }}>
          <Typography variant="body2" fontWeight="medium">
            {params.value}
          </Typography>
        </Box>
      )
    },
    { 
      field: 'lastName', 
      headerName: 'Last Name', 
      flex: 1,
      minWidth: 120,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => (
        <Box sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          width: '100%'
        }}>
          <Typography variant="body2" fontWeight="medium">
            {params.value}
          </Typography>
        </Box>
      )
    },
    { 
      field: 'email', 
      headerName: 'Email', 
      flex: 1.5,
      minWidth: 200,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => (
        <Box sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          width: '100%'
        }}>
          <Typography variant="body2" color="text.secondary">
            {params.value}
          </Typography>
        </Box>
      )
    },
    { 
      field: 'departmentName', 
      headerName: 'Department', 
      flex: 1,
      minWidth: 120,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => (
        <Box sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          width: '100%'
        }}>
          <Chip 
            label={params.value} 
            color="primary" 
            variant="outlined"
            size="small"
          />
        </Box>
      )
    },
    { 
      field: 'position', 
      headerName: 'Position', 
      flex: 1,
      minWidth: 120,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => (
        <Box sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          width: '100%'
        }}>
          <Typography variant="body2" color="text.secondary">
            {params.value || 'Not specified'}
          </Typography>
        </Box>
      )
    },
    { 
      field: 'salary', 
      headerName: 'Salary', 
      width: 120,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => (
        <Box sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          width: '100%'
        }}>
          <Typography variant="body2" fontWeight="medium">
            ${params.value.toLocaleString()}
          </Typography>
        </Box>
      )
    },
    { 
      field: 'dateOfJoining', 
      headerName: 'Date of Joining', 
      width: 150,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => (
        <Box sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          width: '100%'
        }}>
          <Typography variant="body2">
            {convertUtcToLocalDate(params.value)}
          </Typography>
        </Box>
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
        <Box sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          width: '100%'
        }}>
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
        </Box>
      )
    }
  ];

  const onEdit = (row: Employee) => {
    setEditingId(row.id);
    setForm({ 
      ...row, 
      dateOfBirth: formatDateForInput(row.dateOfBirth) || new Date().toISOString().split('T')[0],
      dateOfJoining: formatDateForInput(row.dateOfJoining) || new Date().toISOString().split('T')[0]
    });
    setOpen(true);
  };

  const onCreate = () => {
    setEditingId(null);
    setForm({ 
      firstName: '', 
      lastName: '', 
      email: '', 
      phoneNumber: '',
      address: '',
      dateOfBirth: new Date().toISOString().split('T')[0],
      dateOfJoining: new Date().toISOString().split('T')[0],
      position: '',
      salary: 0,
      departmentId: departments[0]?.id || 1
    });
    setOpen(true);
  };

  const onSave = async () => {
    try {
    if (editingId) {
      await api.updateEmployee(editingId, form);
        showSnackbar('Employee updated successfully');
    } else {
      await api.createEmployee(form);
        showSnackbar('Employee created successfully');
    }
    setOpen(false);
    await load();
    } catch (error) {
      showSnackbar('Error saving employee', 'error');
    }
  };

  const onDelete = async (id: number) => {
    try {
    await api.deleteEmployee(id);
      showSnackbar('Employee deleted successfully');
    await load();
    } catch (error) {
      showSnackbar('Error deleting employee', 'error');
    }
  };

  const filteredRows = rows.filter(row => {
    const matchesSearch = !searchTerm || 
      row.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      row.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      row.email.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesDepartment = !selectedDepartment || row.departmentId === selectedDepartment;
    
    return matchesSearch && matchesDepartment;
  });

  return (
    <Box 
      sx={{ 
        width: "100%",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "flex-start",
        p: { xs: 0, sm: 1, md: 2, lg: 3, xl: 4 },
        mx: "auto",
      }}
    >
      {/* Header Section */}
      <Fade in timeout={600}>
        <Card 
          sx={{ 
            mb: { xs: 3, sm: 4, md: 5, lg: 6 },
            width: "100%",
            maxWidth: { xs: "100%", sm: "100%", md: "1200px", lg: "1400px", xl: "1600px" },
            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            color: 'white',
            boxShadow: '0 8px 32px rgba(102, 126, 234, 0.3)',
            borderRadius: { xs: 2, sm: 3, md: 4 },
            mx: "auto",
          }}
        >
          <CardContent sx={{ p: { xs: 3, sm: 4, md: 5 } }}>
            <Stack 
              direction={{ xs: 'column', md: 'row' }} 
              justifyContent="space-between" 
              alignItems={{ xs: 'stretch', md: 'center' }}
              spacing={{ xs: 2, sm: 3 }}
            >
              <Box sx={{ textAlign: { xs: "center", md: "left" } }}>
                <Typography 
                  variant="h4" 
                  fontWeight="700" 
                  sx={{ 
                    mb: 1,
                    fontFamily: 'Montserrat, Roboto, Arial',
                    textShadow: '0 2px 4px rgba(0,0,0,0.1)',
                    fontSize: { xs: "1.5rem", sm: "1.8rem", md: "2.125rem", lg: "2.5rem" },
                  }}
                >
                  👥 Employee Management
                </Typography>
                <Typography 
                  variant="body1" 
                  sx={{ 
                    opacity: 0.9,
                    fontSize: { xs: "0.9rem", sm: "1rem", md: "1.1rem" },
                  }}
                >
                  Manage your workforce efficiently with comprehensive employee data
                </Typography>
              </Box>
              <Stack 
                direction={{ xs: 'column', sm: 'row' }} 
                spacing={{ xs: 1, sm: 2 }}
                sx={{ width: { xs: "100%", md: "auto" } }}
              >
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
                    width: { xs: "100%", sm: "auto" },
                    py: { xs: 1.5, sm: 1 },
                  }}
                >
                  Add Employee
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
                    alignSelf: { xs: "center", sm: "stretch" },
                  }}
                >
                  <MoreVertIcon />
                </IconButton>
              </Stack>
            </Stack>
          </CardContent>
        </Card>
      </Fade>

      {/* Filters Section */}
      <Zoom in timeout={800}>
        <Card 
          sx={{ 
            mb: { xs: 3, sm: 4, md: 5, lg: 6 },
            width: "100%",
            maxWidth: { xs: "100%", sm: "100%", md: "1200px", lg: "1400px", xl: "1600px" },
            boxShadow: '0 4px 20px rgba(0,0,0,0.08)',
            borderRadius: { xs: 2, sm: 3 },
            mx: "auto",
          }}
        >
          <CardContent sx={{ p: { xs: 2, sm: 3 } }}>
            <Stack 
              direction={{ xs: 'column', sm: 'row' }} 
              spacing={{ xs: 2, sm: 3 }} 
              alignItems="center"
            >
              <TextField
                placeholder="Search employees..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <SearchIcon color="action" />
                    </InputAdornment>
                  ),
                }}
                sx={{ 
                  flexGrow: 1, 
                  minWidth: { xs: "100%", sm: 200 },
                  width: { xs: "100%", sm: "auto" },
                }}
                size="small"
              />
              <FormControl 
                size="small" 
                sx={{ 
                  minWidth: { xs: "100%", sm: 150 },
                  width: { xs: "100%", sm: "auto" },
                }}
              >
                <InputLabel>Department</InputLabel>
                <Select
                  value={selectedDepartment}
                  onChange={(e) => setSelectedDepartment(e.target.value as number | '')}
                  label="Department"
                >
                  <MenuItem value="">All Departments</MenuItem>
                  {departments.map((dept) => (
                    <MenuItem key={dept.id} value={dept.id}>
                      {dept.name}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
              <IconButton 
                onClick={load}
                sx={{ 
                  bgcolor: 'primary.50',
                  '&:hover': { bgcolor: 'primary.100' },
                  alignSelf: { xs: "center", sm: "stretch" },
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
            borderRadius: { xs: 2, sm: 3 },
            overflow: 'hidden',
            width: "100%",
            maxWidth: { xs: "100%", sm: "100%", md: "1200px", lg: "1400px", xl: "1600px" },
            mx: "auto",
          }}
        >
          <Box sx={{ 
            height: { xs: 500, sm: 600, md: 700 },
            width: '100%',
            overflow: 'auto',
          }}>
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
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  padding: '8px 16px',
                },
                '& .MuiDataGrid-cellContent': {
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  width: '100%',
                  height: '100%',
                },
                '& .MuiDataGrid-cellCheckbox': {
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
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

      {/* Employee Form Dialog */}
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
          {editingId ? 'Edit Employee' : 'Add New Employee'}
        </DialogTitle>
        <DialogContent sx={{ p: 3 }}>
          <Stack spacing={3} sx={{ mt: 1 }}>
            <Stack direction="row" spacing={2}>
              <TextField 
                label="First Name" 
                value={form.firstName} 
                onChange={(e) => setForm({ ...form, firstName: e.target.value })}
                fullWidth
                required
              />
              <TextField 
                label="Last Name" 
                value={form.lastName} 
                onChange={(e) => setForm({ ...form, lastName: e.target.value })}
                fullWidth
                required
              />
            </Stack>
            <TextField 
              label="Email" 
              type="email"
              value={form.email} 
              onChange={(e) => setForm({ ...form, email: e.target.value })}
              fullWidth
              required
            />
            <Stack direction="row" spacing={2}>
              <TextField 
                label="Phone Number" 
                value={form.phoneNumber || ''} 
                onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })}
                fullWidth
              />
              <TextField 
                label="Address" 
                value={form.address || ''} 
                onChange={(e) => setForm({ ...form, address: e.target.value })}
                fullWidth
              />
            </Stack>
            <FormControl fullWidth required>
              <InputLabel>Department</InputLabel>
              <Select
                value={form.departmentId}
                onChange={(e) => setForm({ ...form, departmentId: Number(e.target.value) })}
                label="Department"
              >
                {departments.map((dept) => (
                  <MenuItem key={dept.id} value={dept.id}>
                    {dept.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
            <TextField 
              label="Position" 
              value={form.position || ''} 
              onChange={(e) => setForm({ ...form, position: e.target.value })}
              fullWidth
            />
            <Stack direction="row" spacing={2}>
              <TextField 
                label="Salary" 
                type="number"
                value={form.salary} 
                onChange={(e) => setForm({ ...form, salary: Number(e.target.value) })}
                fullWidth
                required
                InputProps={{
                  startAdornment: <InputAdornment position="start">$</InputAdornment>,
                }}
              />
              <TextField 
                label="Date of Birth" 
                type="date"
                value={form.dateOfBirth} 
                onChange={(e) => setForm({ ...form, dateOfBirth: e.target.value })}
                fullWidth
                InputLabelProps={{ shrink: true }}
              />
            </Stack>
            <TextField 
              label="Date of Joining" 
              type="date"
              value={form.dateOfJoining} 
              onChange={(e) => setForm({ ...form, dateOfJoining: e.target.value })}
              fullWidth
              InputLabelProps={{ shrink: true }}
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


