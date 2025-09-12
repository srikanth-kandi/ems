import { useCallback, useEffect, useState } from "react";
import { api, type Attendance } from "../../lib/api";
import {
  Box,
  Button,
  Card,
  CardContent,
  Stack,
  TextField,
  Typography,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Snackbar,
  Alert,
} from "@mui/material";
import { convertUtcToLocalTime, convertUtcToLocalDate, formatDuration } from "../../utils/timezone";

type Employee = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
};

export default function Attendance() {
  const [employeeId, setEmployeeId] = useState<number>(1);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [items, setItems] = useState<Attendance[]>([]);
  const [notes, setNotes] = useState<string>("");
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  const showSnackbar = (message: string, severity: 'success' | 'error' = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const loadEmployees = useCallback(async () => {
    try {
      const data = await api.getEmployees();
      setEmployees(data);
      if (data.length > 0 && !employeeId) {
        setEmployeeId(data[0].id);
      }
    } catch (error) {
      showSnackbar('Error loading employees', 'error');
    }
  }, [employeeId]);

  const load = async () => {
    try {
      if (employeeId) {
        const data = await api.getEmployeeAttendance(employeeId);
        setItems(data);
      }
    } catch (error) {
      showSnackbar('Error loading attendance records', 'error');
    }
  };

  useEffect(() => {
    loadEmployees();
  }, [loadEmployees]);

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [employeeId]);

  const onCheckIn = async () => {
    try {
      await api.checkIn({ employeeId, notes });
      setNotes("");
      showSnackbar('Check-in successful!');
      await load();
    } catch (error) {
      showSnackbar('Error checking in', 'error');
    }
  };

  const onCheckOut = async () => {
    try {
      await api.checkOut({ employeeId, notes });
      setNotes("");
      showSnackbar('Check-out successful!');
      await load();
    } catch (error) {
      showSnackbar('Error checking out', 'error');
    }
  };

  return (
    <Box
      className="main-content font-montserrat"
      sx={{
        width: "100%",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "flex-start",
        bgcolor: "background.paper",
        borderRadius: { xs: 2, sm: 3, md: 4 },
        boxShadow: "0 8px 32px rgba(0,0,0,0.12)",
        p: { xs: 2, sm: 3, md: 4, lg: 5, xl: 6 },
        maxWidth: { xs: "100%", sm: "100%", md: "1200px", lg: "1400px", xl: "1600px" },
        mx: "auto",
      }}
    >
      <Typography
        variant="h4"
        sx={{
          mb: { xs: 3, sm: 4, md: 5, lg: 6 },
          fontWeight: 700,
          color: "primary.main",
          textAlign: "center",
          fontSize: { xs: "1.5rem", sm: "1.8rem", md: "2.125rem", lg: "2.5rem" },
          fontFamily: "Montserrat, Roboto, Arial",
        }}
      >
        ⏰ Attendance Management
      </Typography>
      
      <Stack
        direction={{ xs: "column", sm: "row" }}
        spacing={{ xs: 2, sm: 3 }}
        alignItems="center"
        sx={{ 
          mb: { xs: 3, sm: 4, md: 5, lg: 6 },
          width: "100%",
          maxWidth: { xs: "100%", sm: "800px", md: "1000px", lg: "1200px" },
          mx: "auto",
        }}
      >
        <FormControl 
          sx={{ 
            minWidth: { xs: "100%", sm: 200 },
            width: { xs: "100%", sm: "auto" },
          }}
        >
          <InputLabel>Employee</InputLabel>
          <Select
            value={employeeId}
            label="Employee"
            onChange={(e) => setEmployeeId(Number(e.target.value))}
          >
            {employees.map((emp) => (
              <MenuItem key={emp.id} value={emp.id}>
                {emp.firstName} {emp.lastName}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
        <TextField
          label="Notes"
          value={notes}
          onChange={(e) => setNotes(e.target.value)}
          sx={{ 
            minWidth: { xs: "100%", sm: 240 },
            width: { xs: "100%", sm: "auto" },
          }}
        />
        <Stack 
          direction={{ xs: "column", sm: "row" }}
          spacing={{ xs: 1, sm: 2 }}
          sx={{ width: { xs: "100%", sm: "auto" } }}
        >
          <Button 
            variant="contained" 
            onClick={onCheckIn}
            sx={{
              background: "linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)",
              "&:hover": {
                background: "linear-gradient(135deg, #3dd16b 0%, #32e0c7 100%)",
                transform: "translateY(-1px)",
                boxShadow: "0 4px 12px rgba(67, 233, 123, 0.4)",
              },
              transition: "all 0.3s ease",
              py: 1.5,
              px: 3,
              width: { xs: "100%", sm: "auto" },
            }}
          >
            Check In
          </Button>
          <Button 
            variant="outlined" 
            onClick={onCheckOut}
            sx={{
              borderColor: "primary.main",
              color: "primary.main",
              "&:hover": {
                bgcolor: "primary.50",
                transform: "translateY(-1px)",
              },
              transition: "all 0.3s ease",
              py: 1.5,
              px: 3,
              width: { xs: "100%", sm: "auto" },
            }}
          >
            Check Out
          </Button>
        </Stack>
      </Stack>
      
      <Stack 
        spacing={{ xs: 2, sm: 3 }}
        sx={{ 
          width: "100%",
          maxWidth: { xs: "100%", sm: "800px", md: "1000px", lg: "1200px" },
          mx: "auto",
        }}
      >
        {items.map((a) => (
          <Card 
            key={a.id} 
            sx={{
              borderRadius: { xs: 2, sm: 3 },
              boxShadow: "0 4px 20px rgba(0,0,0,0.08)",
              transition: "all 0.3s ease",
              "&:hover": {
                transform: "translateY(-2px)",
                boxShadow: "0 8px 32px rgba(0,0,0,0.12)",
              },
            }}
          >
            <CardContent sx={{ p: { xs: 2, sm: 3 } }}>
              <Stack
                direction={{ xs: "column", sm: "row" }}
                justifyContent="space-between"
                spacing={{ xs: 1, sm: 2 }}
                alignItems={{ xs: "flex-start", sm: "center" }}
              >
                <Typography 
                  variant="subtitle1"
                  sx={{ 
                    fontWeight: 600,
                    color: "primary.main",
                    fontSize: { xs: "0.9rem", sm: "1rem" },
                  }}
                >
                  {convertUtcToLocalDate(a.date)} - {a.employeeName}
                </Typography>
                <Typography 
                  variant="body2"
                  sx={{ 
                    color: "text.secondary",
                    fontSize: { xs: "0.8rem", sm: "0.875rem" },
                    textAlign: { xs: "left", sm: "right" },
                  }}
                >
                  In: {convertUtcToLocalTime(a.checkInTime)} | Out:{" "}
                  {a.checkOutTime
                    ? convertUtcToLocalTime(a.checkOutTime)
                    : "-"}{" "}
                  | Hours: {formatDuration(a.totalHours)}
                </Typography>
              </Stack>
              {a.notes && (
                <Typography 
                  variant="body2" 
                  sx={{ 
                    mt: 1,
                    p: 1.5,
                    bgcolor: (theme) => theme.palette.mode === 'dark' ? 'rgba(255, 255, 255, 0.05)' : 'rgba(0, 0, 0, 0.04)',
                    color: (theme) => theme.palette.mode === 'dark' ? 'grey.50' : 'grey.800',
                    borderRadius: 2,
                    fontSize: { xs: "0.8rem", sm: "0.875rem" },
                    border: (theme) => theme.palette.mode === 'dark' 
                      ? '1px solid rgba(255, 255, 255, 0.12)' 
                      : '1px solid rgba(0, 0, 0, 0.12)',
                    '& strong': {
                      color: (theme) => theme.palette.mode === 'dark' ? 'primary.light' : 'primary.main',
                      fontWeight: 600,
                    },
                  }}
                >
                  <strong>Notes:</strong> {a.notes}
                </Typography>
              )}
            </CardContent>
          </Card>
        ))}
      </Stack>

      {/* Snackbar */}
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
