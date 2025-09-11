import React from 'react';
import { 
  Box, 
  Button, 
  Stack, 
  TextField, 
  FormControl, 
  InputLabel, 
  Select, 
  MenuItem, 
  InputAdornment,
  IconButton,
  Menu,
  MenuItem as MenuItemComponent,
  ListItemIcon,
  ListItemText,
  Chip,
  Typography,
  Tooltip,
  Paper,
  Collapse,
} from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  MoreVert as MoreVertIcon,
  PictureAsPdf as PdfIcon,
  TableChart as ExcelIcon,
  FileDownload as CsvIcon,
} from '@mui/icons-material';

import { useEmployeeManagement } from '../../hooks/useEmployeeManagement';
import PageHeader from '../common/PageHeader';
import FilterSection from '../common/FilterSection';
import FormDialog from '../common/FormDialog';
import NotificationSnackbar from '../common/NotificationSnackbar';

export default function EmployeeListRefactored() {
  const {
    employees,
    departments,
    loading,
    open,
    form,
    editingId,
    searchTerm,
    selectedDepartment,
    notification,
    setOpen,
    setForm,
    setSearchTerm,
    setSelectedDepartment,
    handleCreate,
    handleEdit,
    handleSave,
    handleDelete,
    handleDownload,
    closeNotification,
  } = useEmployeeManagement();

  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

  const handleMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

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
    },
    { 
      field: 'lastName', 
      headerName: 'Last Name', 
      flex: 1,
      minWidth: 120,
    },
    { 
      field: 'email', 
      headerName: 'Email', 
      flex: 1.5,
      minWidth: 200,
    },
    { 
      field: 'departmentName', 
      headerName: 'Department', 
      flex: 1,
      minWidth: 120,
      renderCell: (params) => (
        <Chip 
          label={params.value} 
          color="primary" 
          variant="outlined"
          size="small"
        />
      )
    },
    { 
      field: 'position', 
      headerName: 'Position', 
      flex: 1,
      minWidth: 120,
    },
    { 
      field: 'salary', 
      headerName: 'Salary', 
      width: 120,
      headerAlign: 'right',
      align: 'right',
      renderCell: (params) => (
        <Typography variant="body2" fontWeight="medium">
          ${params.value.toLocaleString()}
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
              onClick={() => handleEdit(params.row)}
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
              onClick={() => handleDelete(params.row.id)}
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

  const headerActions = (
    <>
      <Button
        variant="contained"
        startIcon={<AddIcon />}
        onClick={handleCreate}
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
        }}
      >
        <MoreVertIcon />
      </IconButton>
    </>
  );

  return (
    <Box sx={{ p: { xs: 2, md: 3 } }}>
      <PageHeader
        title="Employee Management"
        subtitle="Manage your workforce efficiently with comprehensive employee data"
        actions={headerActions}
      />

      <FilterSection
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
        selectedDepartment={selectedDepartment}
        onDepartmentChange={setSelectedDepartment}
        departments={departments}
        onRefresh={() => window.location.reload()}
      />

      <Collapse in timeout={1000}>
        <Paper 
          sx={{ 
            boxShadow: '0 4px 20px rgba(0,0,0,0.08)',
            borderRadius: 2,
            overflow: 'hidden',
          }}
        >
          <Box sx={{ height: 600, width: '100%' }}>
            <DataGrid 
              rows={employees} 
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
      </Collapse>

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
        <MenuItemComponent onClick={() => { handleDownload('pdf'); handleMenuClose(); }}>
          <ListItemIcon><PdfIcon color="error" /></ListItemIcon>
          <ListItemText>Download PDF</ListItemText>
        </MenuItemComponent>
        <MenuItemComponent onClick={() => { handleDownload('excel'); handleMenuClose(); }}>
          <ListItemIcon><ExcelIcon color="success" /></ListItemIcon>
          <ListItemText>Download Excel</ListItemText>
        </MenuItemComponent>
        <MenuItemComponent onClick={() => { handleDownload('csv'); handleMenuClose(); }}>
          <ListItemIcon><CsvIcon color="primary" /></ListItemIcon>
          <ListItemText>Download CSV</ListItemText>
        </MenuItemComponent>
      </Menu>

      <FormDialog
        open={open}
        onClose={() => setOpen(false)}
        onSave={handleSave}
        title={editingId ? 'Edit Employee' : 'Add New Employee'}
        saveButtonText={editingId ? 'Update' : 'Create'}
      >
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
            label="Date of Joining" 
            type="date"
            value={form.dateOfJoining} 
            onChange={(e) => setForm({ ...form, dateOfJoining: e.target.value })}
            fullWidth
            InputLabelProps={{ shrink: true }}
          />
        </Stack>
      </FormDialog>

      <NotificationSnackbar
        open={notification.open}
        onClose={closeNotification}
        message={notification.message}
        severity={notification.severity}
      />
    </Box>
  );
}
