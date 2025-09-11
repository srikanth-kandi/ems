import { useCallback, useEffect, useState } from "react";
import { api } from "../../lib/api";
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
} from "@mui/material";

type AttendanceItem = {
  id: number;
  employeeId: number;
  employeeName: string;
  checkInTime: string;
  checkOutTime?: string;
  totalHours?: string;
  notes?: string;
  date: string;
};

type Employee = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
};

export default function Attendance() {
  const [employeeId, setEmployeeId] = useState<number>(1);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [items, setItems] = useState<AttendanceItem[]>([]);
  const [notes, setNotes] = useState<string>("");

  const loadEmployees = useCallback(async () => {
    const data = await api.getEmployees();
    setEmployees(data);
    if (data.length > 0 && !employeeId) {
      setEmployeeId(data[0].id);
    }
  }, [employeeId]);

  const load = async () => {
    if (employeeId) {
      const data = await api.getAttendance(employeeId);
      setItems(data);
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
    await api.checkIn(employeeId, notes);
    setNotes("");
    await load();
  };

  const onCheckOut = async () => {
    await api.checkOut(employeeId, notes);
    setNotes("");
    await load();
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
      <Typography
        variant="h5"
        sx={{
          mb: 2,
          fontWeight: 700,
          color: "primary.main",
          textAlign: { xs: "center", md: "left" },
        }}
      >
        Attendance
      </Typography>
      <Stack
        direction={{ xs: "column", sm: "row" }}
        spacing={2}
        alignItems="center"
        sx={{ mb: 2 }}
      >
        <FormControl sx={{ minWidth: 200 }}>
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
          sx={{ minWidth: 240 }}
        />
        <Button variant="contained" onClick={onCheckIn}>
          Check In
        </Button>
        <Button variant="outlined" onClick={onCheckOut}>
          Check Out
        </Button>
      </Stack>
      <Stack spacing={1}>
        {items.map((a) => (
          <Card key={a.id} className="border-rounded shadow-lg">
            <CardContent>
              <Stack
                direction={{ xs: "column", sm: "row" }}
                justifyContent="space-between"
                spacing={2}
              >
                <Typography variant="subtitle1">
                  {a.date} - {a.employeeName}
                </Typography>
                <Typography variant="body2">
                  In: {new Date(a.checkInTime).toLocaleTimeString()} | Out:{" "}
                  {a.checkOutTime
                    ? new Date(a.checkOutTime).toLocaleTimeString()
                    : "-"}{" "}
                  | Hours: {a.totalHours ?? "-"}
                </Typography>
              </Stack>
              {a.notes && (
                <Typography variant="body2" color="text.secondary">
                  Notes: {a.notes}
                </Typography>
              )}
            </CardContent>
          </Card>
        ))}
      </Stack>
    </Box>
  );
}
