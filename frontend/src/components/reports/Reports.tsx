import { Box, Button, Stack, Typography } from '@mui/material';
import { api } from '../../lib/api';

export default function Reports() {
  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h5" sx={{ mb: 2 }}>Reports</Typography>
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
        <Button variant="contained" onClick={() => api.downloadReport('employees', 'employees.csv')}>Employee Directory</Button>
        <Button variant="contained" onClick={() => api.downloadReport('departments', 'departments.csv')}>Departments</Button>
        <Button variant="contained" onClick={() => api.downloadReport('attendance', 'attendance.csv')}>Attendance</Button>
        <Button variant="contained" onClick={() => api.downloadReport('salaries', 'salaries.csv')}>Salaries</Button>
      </Stack>
    </Box>
  );
}


