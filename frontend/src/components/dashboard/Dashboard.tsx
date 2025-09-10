import { useEffect, useState } from 'react';
import { 
  Box, 
  Card, 
  CardContent, 
  Typography, 
  Grid, 
  Paper,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Chip,
  Avatar
} from '@mui/material';
import {
  People as PeopleIcon,
  AccessTime as AccessTimeIcon,
  Assessment as AssessmentIcon,
  TrendingUp as TrendingUpIcon,
  CheckCircle as CheckCircleIcon,
  Schedule as ScheduleIcon
} from '@mui/icons-material';
import { api } from '../../lib/api';
import { useAuthStore } from '../../store/auth';

type DashboardStats = {
  totalEmployees: number;
  activeEmployees: number;
  todayCheckIns: number;
  pendingReports: number;
};

type RecentActivity = {
  id: number;
  type: 'checkin' | 'checkout' | 'employee_added' | 'employee_updated';
  message: string;
  timestamp: string;
  employeeName?: string;
};

export default function Dashboard() {
  const [stats, setStats] = useState<DashboardStats>({
    totalEmployees: 0,
    activeEmployees: 0,
    todayCheckIns: 0,
    pendingReports: 0
  });
  const [recentActivity, setRecentActivity] = useState<RecentActivity[]>([]);
  const [loading, setLoading] = useState(true);
  const { username } = useAuthStore();

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      // Load employees for stats
      const employees = await api.getEmployees();
      const activeEmployees = employees.filter((emp: any) => emp.isActive);
      
      // Mock data for demonstration - in real app, these would come from API
      setStats({
        totalEmployees: employees.length,
        activeEmployees: activeEmployees.length,
        todayCheckIns: Math.floor(Math.random() * 20) + 5, // Mock data
        pendingReports: Math.floor(Math.random() * 5) + 1
      });

      // Mock recent activity
      setRecentActivity([
        {
          id: 1,
          type: 'checkin',
          message: 'John Doe checked in',
          timestamp: new Date().toISOString(),
          employeeName: 'John Doe'
        },
        {
          id: 2,
          type: 'employee_added',
          message: 'New employee added: Jane Smith',
          timestamp: new Date(Date.now() - 3600000).toISOString(),
          employeeName: 'Jane Smith'
        },
        {
          id: 3,
          type: 'checkout',
          message: 'Mike Johnson checked out',
          timestamp: new Date(Date.now() - 7200000).toISOString(),
          employeeName: 'Mike Johnson'
        }
      ]);
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const getActivityIcon = (type: string) => {
    switch (type) {
      case 'checkin':
        return <CheckCircleIcon color="success" />;
      case 'checkout':
        return <ScheduleIcon color="info" />;
      case 'employee_added':
        return <PeopleIcon color="primary" />;
      case 'employee_updated':
        return <TrendingUpIcon color="warning" />;
      default:
        return <AssessmentIcon />;
    }
  };

  const getActivityColor = (type: string) => {
    switch (type) {
      case 'checkin':
        return 'success';
      case 'checkout':
        return 'info';
      case 'employee_added':
        return 'primary';
      case 'employee_updated':
        return 'warning';
      default:
        return 'default';
    }
  };

  const formatTime = (timestamp: string) => {
    return new Date(timestamp).toLocaleTimeString();
  };

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Welcome back, {username}!
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 4 }}>
        Here's what's happening in your Employee Management System today.
      </Typography>

      {/* Stats Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Avatar sx={{ bgcolor: 'primary.main', mr: 2 }}>
                  <PeopleIcon />
                </Avatar>
                <Box>
                  <Typography variant="h4">{stats.totalEmployees}</Typography>
                  <Typography color="text.secondary">Total Employees</Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Avatar sx={{ bgcolor: 'success.main', mr: 2 }}>
                  <CheckCircleIcon />
                </Avatar>
                <Box>
                  <Typography variant="h4">{stats.activeEmployees}</Typography>
                  <Typography color="text.secondary">Active Employees</Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Avatar sx={{ bgcolor: 'info.main', mr: 2 }}>
                  <AccessTimeIcon />
                </Avatar>
                <Box>
                  <Typography variant="h4">{stats.todayCheckIns}</Typography>
                  <Typography color="text.secondary">Today's Check-ins</Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Avatar sx={{ bgcolor: 'warning.main', mr: 2 }}>
                  <AssessmentIcon />
                </Avatar>
                <Box>
                  <Typography variant="h4">{stats.pendingReports}</Typography>
                  <Typography color="text.secondary">Pending Reports</Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Recent Activity */}
      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Recent Activity
            </Typography>
            <List>
              {recentActivity.map((activity) => (
                <ListItem key={activity.id} divider>
                  <ListItemIcon>
                    {getActivityIcon(activity.type)}
                  </ListItemIcon>
                  <ListItemText
                    primary={activity.message}
                    secondary={formatTime(activity.timestamp)}
                  />
                  <Chip 
                    label={activity.type.replace('_', ' ')} 
                    color={getActivityColor(activity.type) as any}
                    size="small"
                  />
                </ListItem>
              ))}
            </List>
          </Paper>
        </Grid>

        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Quick Actions
            </Typography>
            <List>
              <ListItem button component="a" href="/employees">
                <ListItemIcon>
                  <PeopleIcon />
                </ListItemIcon>
                <ListItemText primary="Manage Employees" />
              </ListItem>
              <ListItem button component="a" href="/attendance">
                <ListItemIcon>
                  <AccessTimeIcon />
                </ListItemIcon>
                <ListItemText primary="View Attendance" />
              </ListItem>
              <ListItem button component="a" href="/reports">
                <ListItemIcon>
                  <AssessmentIcon />
                </ListItemIcon>
                <ListItemText primary="Generate Reports" />
              </ListItem>
            </List>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}
