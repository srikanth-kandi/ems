import { useEffect, useState } from "react";
import { api, type Attendance, type Employee } from "../../lib/api";
import {
  Box,
  Grid,
  Card,
  CardContent,
  Typography,
  Paper,
  List,
  ListItem,
  ListItemText,
  Chip,
} from "@mui/material";
import {
  People as PeopleIcon,
  AccessTime as AccessTimeIcon,
  Assessment as AssessmentIcon,
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
  color,
}: {
  title: string;
  value: number;
  icon: JSX.Element;
  color: string;
}) => (
  <Card>
    <CardContent>
      <Box display="flex" alignItems="center" justifyContent="space-between">
        <Box>
          <Typography color="textSecondary" gutterBottom variant="h6">
            {title}
          </Typography>
          <Typography variant="h4">{value}</Typography>
        </Box>
        <Box color={color} fontSize="3rem">
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
    totalDepartments: 4,
  });
  const [recentActivity, setRecentActivity] = useState<RecentActivity[]>([]);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      const employees: Employee[] = await api.getEmployees();
      const attendance: Attendance[] = await api.getAttendance(1); // Get attendance for first employee as sample

      setStats({
        totalEmployees: employees.length,
        activeEmployees: employees.filter((emp) => emp.isActive)
          .length,
        todayAttendance: attendance.filter(
          (att) =>
            new Date(att.date).toDateString() === new Date().toDateString()
        ).length,
        totalDepartments: 4,
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
      ]);
    } catch (error) {
      console.error("Error loading dashboard data:", error);
    }
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
        variant="h4"
        gutterBottom
        sx={{
          fontWeight: 700,
          color: "primary.main",
          mb: 3,
          textAlign: { xs: "center", md: "left" },
        }}
      >
        Dashboard
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Total Employees"
            value={stats.totalEmployees}
            icon={<PeopleIcon />}
            color="primary.main"
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Active Employees"
            value={stats.activeEmployees}
            icon={<PeopleIcon />}
            color="success.main"
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Today's Attendance"
            value={stats.todayAttendance}
            icon={<AccessTimeIcon />}
            color="info.main"
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Departments"
            value={stats.totalDepartments}
            icon={<AssessmentIcon />}
            color="secondary.main"
          />
        </Grid>
        <Grid item xs={12} md={8}>
          <Paper
            className="border-rounded shadow-lg"
            sx={{ p: { xs: 1, sm: 2 }, mb: 2 }}
          >
            <Typography
              variant="h6"
              gutterBottom
              sx={{ fontWeight: 600, color: "secondary.main" }}
            >
              Recent Activity
            </Typography>
            <List>
              {recentActivity.map((activity) => (
                <ListItem key={activity.id} divider>
                  <ListItemText
                    primary={activity.message}
                    secondary={activity.timestamp}
                  />
                  <Chip
                    label={activity.type}
                    size="small"
                    color={
                      activity.type === "employee"
                        ? "primary"
                        : activity.type === "attendance"
                        ? "success"
                        : "default"
                    }
                  />
                </ListItem>
              ))}
            </List>
          </Paper>
        </Grid>
        <Grid item xs={12} md={4}>
          <Paper
            className="border-rounded shadow-lg"
            sx={{ p: { xs: 1, sm: 2 }, mb: 2 }}
          >
            <Typography
              variant="h6"
              gutterBottom
              sx={{ fontWeight: 600, color: "primary.main" }}
            >
              Quick Actions
            </Typography>
            <List>
              <ListItem>
                <ListItemText primary="Add New Employee" />
              </ListItem>
              <ListItem>
                <ListItemText primary="Generate Reports" />
              </ListItem>
              <ListItem>
                <ListItemText primary="View Attendance" />
              </ListItem>
            </List>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}
