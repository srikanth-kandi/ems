import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { Box } from '@mui/material';

// Import components (to be created)
import Login from './components/auth/Login';
import EmployeeList from './components/employees/EmployeeList';
import Attendance from './components/attendance/Attendance';
import Reports from './components/reports/Reports';
import { useAuthStore } from './store/auth';
// import Dashboard from './components/dashboard/Dashboard';
// import EmployeeList from './components/employees/EmployeeList';

const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
});

function PrivateRoute({ children }: { children: JSX.Element }) {
  const token = useAuthStore((s) => s.token);
  if (!token) {
    return <Navigate to="/login" replace />;
  }
  return children;
}

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Router>
        <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
          <Routes>
            {/* Temporary placeholder - will be replaced with actual components */}
            <Route path="/" element={<PrivateRoute>
              <Box sx={{ p: 4, textAlign: 'center' }}>
                <h1>Employee Management System</h1>
                <p>Welcome to EMS - You are logged in.</p>
              </Box>
            </PrivateRoute>} />
            <Route path="/employees" element={<PrivateRoute><EmployeeList /></PrivateRoute>} />
            <Route path="/attendance" element={<PrivateRoute><Attendance /></PrivateRoute>} />
            <Route path="/reports" element={<PrivateRoute><Reports /></PrivateRoute>} />
            <Route path="/login" element={<Login />} />
            <Route path="/login" element={<Login />} />
          </Routes>
        </Box>
      </Router>
    </ThemeProvider>
  );
}

export default App;