import React, { useEffect, useState } from "react";
import { api } from "../../lib/api";
import {
  Box,
  Card,
  CardContent,
  Typography,
  List,
  ListItem,
  ListItemText,
  Chip,
  Button,
  Stack,
  Fade,
  LinearProgress,
  Collapse,
} from "@mui/material";
import { convertUtcToLocalDate } from "../../utils/timezone";
import {
  People as PeopleIcon,
  AccessTime as AccessTimeIcon,
  Assessment as AssessmentIcon,
  Business as BusinessIcon,
  TrendingUp as TrendingUpIcon,
  Add as AddIcon,
  Assessment as ReportIcon,
  Visibility as ViewIcon,
} from "@mui/icons-material";

type DashboardStats = {
  totalEmployees: number;
  activeEmployees: number;
  todayAttendance: number;
  totalDepartments: number;
};

type RecentActivity = {
  id: number;
  type: string;
  message: string;
  timestamp: string;
};

const StatCard = ({
  title,
  value,
  icon,
  gradient,
  loading = false,
}: {
  title: string;
  value: number;
  icon: React.JSX.Element;
  gradient: string;
  loading?: boolean;
}) => (
  <Card 
    sx={{ 
      height: '100%',
      borderRadius: 3,
      boxShadow: '0 4px 20px rgba(0,0,0,0.08)',
      transition: 'all 0.3s ease',
      '&:hover': {
        transform: 'translateY(-4px)',
        boxShadow: '0 8px 32px rgba(0,0,0,0.12)',
      },
    }}
  >
    <CardContent sx={{ p: 3 }}>
      <Box display="flex" alignItems="center" justifyContent="space-between">
        <Box sx={{ flexGrow: 1 }}>
          <Typography 
            color="textSecondary" 
            gutterBottom 
            variant="h6"
            sx={{ 
              fontWeight: 500,
              fontSize: '0.9rem',
              textTransform: 'uppercase',
              letterSpacing: 1,
            }}
          >
            {title}
          </Typography>
          {loading ? (
            <LinearProgress sx={{ mt: 1, borderRadius: 1 }} />
          ) : (
            <Typography 
              variant="h3" 
              sx={{ 
                fontWeight: 700,
                background: gradient,
                backgroundClip: 'text',
                WebkitBackgroundClip: 'text',
                WebkitTextFillColor: 'transparent',
                fontFamily: 'Montserrat, Roboto, Arial',
              }}
            >
              {value.toLocaleString()}
            </Typography>
          )}
        </Box>
        <Box 
          sx={{ 
            p: 2,
            borderRadius: 2,
            background: gradient,
            color: 'white',
            fontSize: '2.5rem',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            minWidth: 80,
            minHeight: 80,
          }}
        >
          {icon}
        </Box>
      </Box>
    </CardContent>
  </Card>
);

export default function Dashboard() {
  const [stats, setStats] = useState<DashboardStats>({
    totalEmployees: 0,
    activeEmployees: 0,
    todayAttendance: 0,
    totalDepartments: 0,
  });
  const [recentActivity, setRecentActivity] = useState<RecentActivity[]>([]);
  const [loading, setLoading] = useState(true);
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    // Set mounted to true after component mounts
    setMounted(true);
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      const [employees, departments, attendance] = await Promise.all([
        api.getEmployees(),
        api.getDepartments(),
        api.getAttendance(1) // Get attendance for first employee as sample
      ]);

      setStats({
        totalEmployees: employees.length,
        activeEmployees: employees.filter((emp) => emp.isActive).length,
        todayAttendance: attendance.filter(
          (att) =>
            convertUtcToLocalDate(att.date) === convertUtcToLocalDate(new Date().toISOString())
        ).length,
        totalDepartments: departments.length,
      });

      // Mock recent activity
      setRecentActivity([
        {
          id: 1,
          type: "employee",
          message: "New employee John Doe added",
          timestamp: "2 hours ago",
        },
        {
          id: 2,
          type: "attendance",
          message: "John Doe checked in",
          timestamp: "1 hour ago",
        },
        {
          id: 3,
          type: "report",
          message: "Monthly report generated",
          timestamp: "3 hours ago",
        },
        {
          id: 4,
          type: "department",
          message: "IT Department created",
          timestamp: "4 hours ago",
        },
      ]);
    } catch (error) {
      console.error("Error loading dashboard data:", error);
    } finally {
      setLoading(false);
    }
  };

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
            <Box sx={{ textAlign: { xs: "center", md: "left" } }}>
              <Typography 
                variant="h3" 
                fontWeight="700" 
                sx={{ 
                  mb: 2,
                  fontFamily: 'Montserrat, Roboto, Arial',
                  textShadow: '0 2px 4px rgba(0,0,0,0.1)',
                  fontSize: { xs: "1.8rem", sm: "2.2rem", md: "2.5rem", lg: "3rem" },
                }}
              >
                📊 Welcome to EMS Dashboard
              </Typography>
              <Typography 
                variant="h6" 
                sx={{ 
                  opacity: 0.9, 
                  fontWeight: 400,
                  fontSize: { xs: "1rem", sm: "1.1rem", md: "1.25rem" },
                }}
              >
                Monitor your workforce and track key performance indicators
              </Typography>
            </Box>
          </CardContent>
        </Card>
      </Fade>

      {/* Stats Cards */}
      <Box sx={{ 
        display: 'grid', 
        gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)', md: 'repeat(4, 1fr)' },
        gap: { xs: 2, sm: 3, md: 4, lg: 5 },
        mb: { xs: 3, sm: 4, md: 5, lg: 6 },
        width: "100%",
        maxWidth: { xs: "100%", sm: "100%", md: "1200px", lg: "1400px", xl: "1600px" },
        mx: "auto",
      }}>
        <Collapse in={mounted} timeout={800}>
          <StatCard
            title="Total Employees"
            value={stats.totalEmployees}
            icon={<PeopleIcon />}
            gradient="linear-gradient(135deg, #667eea 0%, #764ba2 100%)"
            loading={loading}
          />
        </Collapse>
        <Collapse in={mounted} timeout={1000}>
          <StatCard
            title="Active Employees"
            value={stats.activeEmployees}
            icon={<PeopleIcon />}
            gradient="linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)"
            loading={loading}
          />
        </Collapse>
        <Collapse in={mounted} timeout={1200}>
          <StatCard
            title="Today's Attendance"
            value={stats.todayAttendance}
            icon={<AccessTimeIcon />}
            gradient="linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)"
            loading={loading}
          />
        </Collapse>
        <Collapse in={mounted} timeout={1400}>
          <StatCard
            title="Departments"
            value={stats.totalDepartments}
            icon={<BusinessIcon />}
            gradient="linear-gradient(135deg, #f093fb 0%, #f5576c 100%)"
            loading={loading}
          />
        </Collapse>
      </Box>

      {/* Content Grid */}
      <Box sx={{ 
        display: 'grid', 
        gridTemplateColumns: { xs: '1fr', md: '2fr 1fr' },
        gap: { xs: 2, sm: 3, md: 4, lg: 5 },
        width: "100%",
        maxWidth: { xs: "100%", sm: "100%", md: "1200px", lg: "1400px", xl: "1600px" },
        mx: "auto",
      }}>
        <Collapse in={mounted} timeout={1600}>
          <Card 
            sx={{ 
              borderRadius: 3,
              boxShadow: '0 4px 20px rgba(0,0,0,0.08)',
              height: '100%',
            }}
          >
            <CardContent sx={{ p: 3 }}>
              <Typography
                variant="h5"
                gutterBottom
                sx={{ 
                  fontWeight: 600, 
                  color: "primary.main",
                  mb: 3,
                  fontFamily: 'Montserrat, Roboto, Arial',
                  display: 'flex',
                  alignItems: 'center',
                  gap: 1,
                }}
              >
                <TrendingUpIcon />
                Recent Activity
              </Typography>
              <List sx={{ '& .MuiListItem-root': { px: 0 } }}>
                {recentActivity.map((activity, index) => (
                  <ListItem 
                    key={activity.id} 
                    divider={index < recentActivity.length - 1}
                    sx={{ 
                      py: 2,
                      '&:hover': { 
                        bgcolor: 'rgba(102, 126, 234, 0.04)',
                        borderRadius: 2,
                      },
                      transition: 'all 0.2s ease',
                    }}
                  >
                    <ListItemText
                      primary={
                        <Typography variant="body1" fontWeight="500">
                          {activity.message}
                        </Typography>
                      }
                      secondary={
                        <Typography variant="body2" color="text.secondary">
                          {activity.timestamp}
                        </Typography>
                      }
                    />
                    <Chip
                      label={activity.type}
                      size="small"
                      sx={{
                        bgcolor: 
                          activity.type === "employee"
                            ? "rgba(102, 126, 234, 0.1)"
                            : activity.type === "attendance"
                            ? "rgba(67, 233, 123, 0.1)"
                            : activity.type === "department"
                            ? "rgba(240, 147, 251, 0.1)"
                            : "rgba(0, 0, 0, 0.1)",
                        color: 
                          activity.type === "employee"
                            ? "primary.main"
                            : activity.type === "attendance"
                            ? "success.main"
                            : activity.type === "department"
                            ? "secondary.main"
                            : "text.primary",
                        fontWeight: 500,
                        borderRadius: 2,
                      }}
                    />
                  </ListItem>
                ))}
              </List>
            </CardContent>
          </Card>
        </Collapse>
        
        <Collapse in={mounted} timeout={1800}>
          <Card 
            sx={{ 
              borderRadius: 3,
              boxShadow: '0 4px 20px rgba(0,0,0,0.08)',
              height: '100%',
            }}
          >
            <CardContent sx={{ p: 3 }}>
              <Typography
                variant="h5"
                gutterBottom
                sx={{ 
                  fontWeight: 600, 
                  color: "primary.main",
                  mb: 3,
                  fontFamily: 'Montserrat, Roboto, Arial',
                  display: 'flex',
                  alignItems: 'center',
                  gap: 1,
                }}
              >
                <AssessmentIcon />
                Quick Actions
              </Typography>
              <Stack spacing={2}>
                <Button
                  variant="contained"
                  startIcon={<AddIcon />}
                  fullWidth
                  sx={{
                    background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                    '&:hover': {
                      background: 'linear-gradient(135deg, #5a6fd8 0%, #6a4190 100%)',
                      transform: 'translateY(-1px)',
                      boxShadow: '0 4px 12px rgba(102, 126, 234, 0.4)',
                    },
                    transition: 'all 0.3s ease',
                    borderRadius: 2,
                    py: 1.5,
                  }}
                >
                  Add New Employee
                </Button>
                <Button
                  variant="outlined"
                  startIcon={<ReportIcon />}
                  fullWidth
                  sx={{ 
                    borderRadius: 2,
                    py: 1.5,
                    '&:hover': {
                      bgcolor: 'primary.50',
                      transform: 'translateY(-1px)',
                    },
                    transition: 'all 0.3s ease',
                  }}
                >
                  Generate Reports
                </Button>
                <Button
                  variant="outlined"
                  startIcon={<ViewIcon />}
                  fullWidth
                  sx={{ 
                    borderRadius: 2,
                    py: 1.5,
                    '&:hover': {
                      bgcolor: 'info.50',
                      transform: 'translateY(-1px)',
                    },
                    transition: 'all 0.3s ease',
                  }}
                >
                  View Attendance
                </Button>
                <Button
                  variant="outlined"
                  startIcon={<BusinessIcon />}
                  fullWidth
                  sx={{ 
                    borderRadius: 2,
                    py: 1.5,
                    '&:hover': {
                      bgcolor: 'secondary.50',
                      transform: 'translateY(-1px)',
                    },
                    transition: 'all 0.3s ease',
                  }}
                >
                  Manage Departments
                </Button>
              </Stack>
            </CardContent>
          </Card>
        </Collapse>
      </Box>
    </Box>
  );
}
