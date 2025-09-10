import { useEffect, useState } from 'react';
import { api } from '../../lib/api';
import { Box, Button, Card, CardContent, Stack, TextField, Typography } from '@mui/material';

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

export default function Attendance() {
  const [employeeId, setEmployeeId] = useState<number>(1);
  const [items, setItems] = useState<AttendanceItem[]>([]);
  const [notes, setNotes] = useState<string>('');

  const load = async () => {
    const data = await api.getAttendance(employeeId);
    setItems(data);
  };

  useEffect(() => { load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [employeeId]);

  const onCheckIn = async () => {
    await api.checkIn(employeeId, notes);
    setNotes('');
    await load();
  };

  const onCheckOut = async () => {
    await api.checkOut(employeeId, notes);
    setNotes('');
    await load();
  };

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h5" sx={{ mb: 2 }}>Attendance</Typography>
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems="center" sx={{ mb: 2 }}>
        <TextField label="Employee ID" type="number" value={employeeId} onChange={(e) => setEmployeeId(Number(e.target.value))} />
        <TextField label="Notes" value={notes} onChange={(e) => setNotes(e.target.value)} sx={{ minWidth: 240 }} />
        <Button variant="contained" onClick={onCheckIn}>Check In</Button>
        <Button variant="outlined" onClick={onCheckOut}>Check Out</Button>
      </Stack>

      <Stack spacing={1}>
        {items.map((a) => (
          <Card key={a.id}>
            <CardContent>
              <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" spacing={2}>
                <Typography variant="subtitle1">{a.date} - {a.employeeName}</Typography>
                <Typography variant="body2">In: {new Date(a.checkInTime).toLocaleTimeString()} | Out: {a.checkOutTime ? new Date(a.checkOutTime).toLocaleTimeString() : '-'} | Hours: {a.totalHours ?? '-'}</Typography>
              </Stack>
              {a.notes && <Typography variant="body2" color="text.secondary">Notes: {a.notes}</Typography>}
            </CardContent>
          </Card>
        ))}
      </Stack>
    </Box>
  );
}


