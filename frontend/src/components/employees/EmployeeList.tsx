import { useEffect, useState } from 'react';
import { api } from '../../lib/api';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, Stack, TextField, Typography } from '@mui/material';

type Employee = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  departmentId: number;
  departmentName: string;
  position?: string;
  salary: number;
  isActive: boolean;
};

export default function EmployeeList() {
  const [rows, setRows] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<any>({ firstName: '', lastName: '', email: '', departmentId: 1, salary: 0 });
  const [editingId, setEditingId] = useState<number | null>(null);

  const load = async () => {
    setLoading(true);
    const data = await api.getEmployees();
    setRows(data);
    setLoading(false);
  };

  useEffect(() => { load(); }, []);

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'firstName', headerName: 'First name', flex: 1 },
    { field: 'lastName', headerName: 'Last name', flex: 1 },
    { field: 'email', headerName: 'Email', flex: 1.5 },
    { field: 'departmentName', headerName: 'Department', flex: 1 },
    { field: 'position', headerName: 'Position', flex: 1 },
    { field: 'salary', headerName: 'Salary', width: 120 },
    {
      field: 'actions', headerName: 'Actions', width: 220, sortable: false, renderCell: (params) => (
        <Stack direction="row" spacing={1}>
          <Button size="small" variant="outlined" onClick={() => onEdit(params.row)}>Edit</Button>
          <Button size="small" color="error" variant="outlined" onClick={() => onDelete(params.row.id)}>Delete</Button>
        </Stack>
      )
    }
  ];

  const onEdit = (row: Employee) => {
    setEditingId(row.id);
    setForm({ ...row });
    setOpen(true);
  };

  const onCreate = () => {
    setEditingId(null);
    setForm({ firstName: '', lastName: '', email: '', departmentId: 1, salary: 0 });
    setOpen(true);
  };

  const onSave = async () => {
    if (editingId) {
      await api.updateEmployee(editingId, form);
    } else {
      await api.createEmployee(form);
    }
    setOpen(false);
    await load();
  };

  const onDelete = async (id: number) => {
    await api.deleteEmployee(id);
    await load();
  };

  return (
    <Box sx={{ p: 3 }}>
      <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 2 }}>
        <Typography variant="h5">Employees</Typography>
        <Button variant="contained" onClick={onCreate}>Add Employee</Button>
      </Stack>
      <div style={{ height: 520, width: '100%' }}>
        <DataGrid rows={rows} columns={columns} loading={loading} pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />
      </div>

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>{editingId ? 'Edit Employee' : 'Create Employee'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField label="First Name" value={form.firstName} onChange={(e) => setForm({ ...form, firstName: e.target.value })} />
            <TextField label="Last Name" value={form.lastName} onChange={(e) => setForm({ ...form, lastName: e.target.value })} />
            <TextField label="Email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
            <TextField label="Department Id" type="number" value={form.departmentId} onChange={(e) => setForm({ ...form, departmentId: Number(e.target.value) })} />
            <TextField label="Position" value={form.position ?? ''} onChange={(e) => setForm({ ...form, position: e.target.value })} />
            <TextField label="Salary" type="number" value={form.salary} onChange={(e) => setForm({ ...form, salary: Number(e.target.value) })} />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={onSave}>Save</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}


