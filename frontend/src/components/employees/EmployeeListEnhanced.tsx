import { useEffect, useState } from "react";
import { api, type Employee } from "../../lib/api";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
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
  Tabs,
  Tab,
  Alert,
  Snackbar,
  Chip,
  Paper,
  Grid,
  Card,
  CardContent,
} from "@mui/material";
import { convertUtcToLocalDate, formatDateForInput } from "../../utils/timezone";
import {
  Add as AddIcon,
  Upload as UploadIcon,
  Download as DownloadIcon,
} from "@mui/icons-material";

type EmployeeForm = {
  firstName: string;
  lastName: string;
  email: string;
  departmentId: number;
  salary: number;
  position: string;
  dateOfBirth: string;
  dateOfJoining: string;
};

type TabPanelProps = {
  children?: React.ReactNode;
  index: number;
  value: number;
};

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

export default function EmployeeListEnhanced() {
  const [rows, setRows] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const [tabValue, setTabValue] = useState(0);
  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };
  const [form, setForm] = useState<EmployeeForm>({
    firstName: "",
    lastName: "",
    email: "",
    departmentId: 1,
    salary: 0,
    position: "",
    dateOfBirth: "",
    dateOfJoining: new Date().toISOString().split("T")[0],
  });
  const [editingId, setEditingId] = useState<number | null>(null);
  const [bulkData, setBulkData] = useState<string>("");
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success" as "success" | "error",
  });

  const load = async () => {
    setLoading(true);
    try {
      const data = await api.getEmployees();
      setRows(data);
    } catch {
      setSnackbar({
        open: true,
        message: "Error loading employees",
        severity: "error",
      });
    }
    setLoading(false);
  };

  useEffect(() => {
    load();
  }, []);

  const columns: GridColDef[] = [
    { field: "id", headerName: "ID", width: 70 },
    { field: "firstName", headerName: "First name", flex: 1 },
    { field: "lastName", headerName: "Last name", flex: 1 },
    { field: "email", headerName: "Email", flex: 1.5 },
    { field: "departmentName", headerName: "Department", flex: 1 },
    { field: "position", headerName: "Position", flex: 1 },
    {
      field: "salary",
      headerName: "Salary",
      width: 120,
      renderCell: (params) => `$${params.value.toLocaleString()}`,
    },
    {
      field: "isActive",
      headerName: "Status",
      width: 100,
      renderCell: (params) => (
        <Chip
          label={params.value ? "Active" : "Inactive"}
          color={params.value ? "success" : "default"}
          size="small"
        />
      ),
    },
    {
      field: "actions",
      headerName: "Actions",
      width: 220,
      sortable: false,
      renderCell: (params) => (
        <Stack direction="row" spacing={1}>
          <Button
            size="small"
            variant="outlined"
            onClick={() => onEdit(params.row)}
          >
            Edit
          </Button>
          <Button
            size="small"
            color="error"
            variant="outlined"
            onClick={() => onDelete(params.row.id)}
          >
            Delete
          </Button>
        </Stack>
      ),
    },
  ];

  const onEdit = (row: Employee) => {
    setEditingId(row.id);
    setForm({
      firstName: row.firstName,
      lastName: row.lastName,
      email: row.email,
      departmentId: row.departmentId,
      salary: row.salary,
      position: row.position || "",
      dateOfBirth: "",
      dateOfJoining: formatDateForInput(row.dateOfJoining) || "",
    });
    setOpen(true);
  };

  const onCreate = () => {
    setEditingId(null);
    setForm({
      firstName: "",
      lastName: "",
      email: "",
      departmentId: 1,
      salary: 0,
      position: "",
      dateOfBirth: "",
      dateOfJoining: new Date().toISOString().split("T")[0],
    });
    setOpen(true);
  };

  const onSave = async () => {
    try {
      const employeeData = {
        ...form,
        name: `${form.firstName} ${form.lastName}`,
        department: "IT", // Placeholder
        departmentName: "Information Technology", // Placeholder
        isActive: true, // Default value
      };
      if (editingId) {
        await api.updateEmployee(editingId, employeeData);
        setSnackbar({
          open: true,
          message: "Employee updated successfully",
          severity: "success",
        });
      } else {
        await api.createEmployee(employeeData);
        setSnackbar({
          open: true,
          message: "Employee created successfully",
          severity: "success",
        });
      }
      setOpen(false);
      await load();
    } catch {
      setSnackbar({
        open: true,
        message: "Error saving employee",
        severity: "error",
      });
    }
  };

  const onDelete = async (id: number) => {
    try {
      await api.deleteEmployee(id);
      setSnackbar({
        open: true,
        message: "Employee deleted successfully",
        severity: "success",
      });
      await load();
    } catch {
      setSnackbar({
        open: true,
        message: "Error deleting employee",
        severity: "error",
      });
    }
  };

  const onBulkCreate = async () => {
    try {
      const lines = bulkData.split("\n").filter((line) => line.trim());
      const employees = lines.map((line) => {
        const [firstName, lastName, email, departmentId, position, salary] =
          line.split(",");
        return {
          firstName: firstName?.trim(),
          lastName: lastName?.trim(),
          name: `${firstName?.trim()} ${lastName?.trim()}`,
          email: email?.trim(),
          department: "IT", // Placeholder
          departmentId: parseInt(departmentId?.trim()) || 1,
          departmentName: "Information Technology", // Placeholder
          position: position?.trim() || "",
          salary: parseFloat(salary?.trim()) || 0,
          isActive: true, // Default value
          dateOfBirth: "1990-01-01",
          dateOfJoining: new Date().toISOString().split("T")[0],
        };
      });

      // Create employees one by one (in real app, you'd have a bulk endpoint)
      for (const employee of employees) {
        await api.createEmployee(employee);
      }

      setSnackbar({
        open: true,
        message: `${employees.length} employees created successfully`,
        severity: "success",
      });
      setBulkData("");
      await load();
    } catch {
      setSnackbar({
        open: true,
        message: "Error creating employees",
        severity: "error",
      });
    }
  };

  const onExport = async () => {
    try {
      await api.downloadReport("employees", "employees.csv");
      setSnackbar({
        open: true,
        message: "Employees exported successfully",
        severity: "success",
      });
    } catch {
      setSnackbar({
        open: true,
        message: "Error exporting employees",
        severity: "error",
      });
    }
  };

  const stats = {
    total: rows.length,
    active: rows.filter((emp) => emp.isActive).length,
    avgSalary:
      rows.length > 0
        ? rows.reduce((sum, emp) => sum + emp.salary, 0) / rows.length
        : 0,
    departments: new Set(rows.map((emp) => emp.departmentName)).size,
  };

  return (
    <Box
      className="main-content font-montserrat"
      sx={{
        bgcolor: "background.paper",
        borderRadius: 3,
        boxShadow: "0 4px 24px rgba(0,0,0,0.08)",
        p: { xs: 1, sm: 2, md: 3 },
      }}
    >
      <Stack
        direction="row"
        justifyContent="space-between"
        alignItems="center"
        sx={{ mb: 3 }}
      >
        <Typography
          variant="h4"
          sx={{ fontWeight: 700, color: "primary.main" }}
        >
          👥 Employee Management
        </Typography>
        <Stack direction="row" spacing={2}>
          <Button
            variant="outlined"
            startIcon={<DownloadIcon />}
            onClick={onExport}
          >
            Export
          </Button>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={onCreate}
          >
            Add Employee
          </Button>
        </Stack>
      </Stack>
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} sm={6} md={3} component="div">
          <Card className="border-rounded shadow-lg">
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Total Employees
              </Typography>
              <Typography variant="h4">{stats.total}</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card className="border-rounded shadow-lg">
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Active Employees
              </Typography>
              <Typography variant="h4">{stats.active}</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card className="border-rounded shadow-lg">
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Avg Salary
              </Typography>
              <Typography variant="h4">
                ${stats.avgSalary.toLocaleString()}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card className="border-rounded shadow-lg">
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Departments
              </Typography>
              <Typography variant="h4">{stats.departments}</Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
      <Paper className="border-rounded shadow-lg" sx={{ width: "100%" }}>
        <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
          <Tabs
            value={tabValue}
            onChange={handleTabChange}
          >
            <Tab label="Employee List" />
            <Tab label="Bulk Create" />
            <Tab label="Analytics" />
          </Tabs>
        </Box>
        <TabPanel value={tabValue} index={0}>
          <div style={{ height: 520, width: "100%" }}>
            <DataGrid
              rows={rows}
              columns={columns}
              loading={loading}
              pageSizeOptions={[10, 25, 50]}
              initialState={{
                pagination: { paginationModel: { pageSize: 10 } },
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
          </div>
        </TabPanel>
        <TabPanel value={tabValue} index={1}>
          <Box>
            <Typography
              variant="h6"
              gutterBottom
              sx={{ fontWeight: 600, color: "secondary.main" }}
            >
              Bulk Create Employees
            </Typography>
            <Typography variant="body2" color="textSecondary" sx={{ mb: 2 }}>
              Enter employee data in CSV format: FirstName, LastName, Email,
              DepartmentId, Position, Salary
            </Typography>
            <TextField
              multiline
              rows={10}
              fullWidth
              value={bulkData}
              onChange={(e) => setBulkData(e.target.value)}
              placeholder="John,Doe,john.doe@company.com,1,Developer,50000\nJane,Smith,jane.smith@company.com,2,Designer,45000"
              sx={{ mb: 2 }}
            />
            <Button
              variant="contained"
              onClick={onBulkCreate}
              startIcon={<UploadIcon />}
            >
              Create Employees
            </Button>
          </Box>
        </TabPanel>
        <TabPanel value={tabValue} index={2}>
          <Box>
            <Typography
              variant="h6"
              gutterBottom
              sx={{ fontWeight: 600, color: "primary.main" }}
            >
              Employee Analytics
            </Typography>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <Card className="border-rounded shadow-lg">
                  <CardContent>
                    <Typography variant="h6" gutterBottom>
                      Department Distribution
                    </Typography>
                    {Object.entries(
                      rows.reduce((acc, emp) => {
                        acc[emp.departmentName] =
                          (acc[emp.departmentName] || 0) + 1;
                        return acc;
                      }, {} as Record<string, number>)
                    ).map(([dept, count]) => (
                      <Box
                        key={dept}
                        display="flex"
                        justifyContent="space-between"
                        sx={{ mb: 1 }}
                      >
                        <Typography>{dept}</Typography>
                        <Typography>{count}</Typography>
                      </Box>
                    ))}
                  </CardContent>
                </Card>
              </Grid>
              <Grid item xs={12} md={6}>
                <Card className="border-rounded shadow-lg">
                  <CardContent>
                    <Typography variant="h6" gutterBottom>
                      Salary Ranges
                    </Typography>
                    <Typography>
                      Min: $
                      {Math.min(
                        ...rows.map((emp) => emp.salary)
                      ).toLocaleString()}
                    </Typography>
                    <Typography>
                      Max: $
                      {Math.max(
                        ...rows.map((emp) => emp.salary)
                      ).toLocaleString()}
                    </Typography>
                    <Typography>
                      Average: ${stats.avgSalary.toLocaleString()}
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
            </Grid>
          </Box>
        </TabPanel>
      </Paper>
      <Dialog
        open={open}
        onClose={() => setOpen(false)}
        fullWidth
        maxWidth="sm"
      >
        <DialogTitle>
          {editingId ? "Edit Employee" : "Create Employee"}
        </DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="First Name"
              value={form.firstName}
              onChange={(e) => setForm({ ...form, firstName: e.target.value })}
              required
            />
            <TextField
              label="Last Name"
              value={form.lastName}
              onChange={(e) => setForm({ ...form, lastName: e.target.value })}
              required
            />
            <TextField
              label="Email"
              type="email"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
              required
            />
            <TextField
              label="Department ID"
              type="number"
              value={form.departmentId}
              onChange={(e) =>
                setForm({ ...form, departmentId: Number(e.target.value) })
              }
              required
            />
            <TextField
              label="Position"
              value={form.position}
              onChange={(e) => setForm({ ...form, position: e.target.value })}
            />
            <TextField
              label="Salary"
              type="number"
              value={form.salary}
              onChange={(e) =>
                setForm({ ...form, salary: Number(e.target.value) })
              }
              required
            />
            <TextField
              label="Date of Birth"
              type="date"
              value={form.dateOfBirth}
              onChange={(e) =>
                setForm({ ...form, dateOfBirth: e.target.value })
              }
              InputLabelProps={{ shrink: true }}
            />
            <TextField
              label="Date of Joining"
              type="date"
              value={form.dateOfJoining}
              onChange={(e) =>
                setForm({ ...form, dateOfJoining: e.target.value })
              }
              InputLabelProps={{ shrink: true }}
              required
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={onSave}>
            Save
          </Button>
        </DialogActions>
      </Dialog>
      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        sx={{
          '& .MuiSnackbar-root': {
            position: 'fixed',
            bottom: 16,
            right: 16,
          }
        }}
      >
        <Alert
          onClose={() => setSnackbar({ ...snackbar, open: false })}
          severity={snackbar.severity}
          sx={{ 
            borderRadius: 2,
            minWidth: 300,
            boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
          }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
