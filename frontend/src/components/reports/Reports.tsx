import { useState, useEffect } from 'react';
import { Box, Button, Stack, Typography, Card, CardContent, Grid, TextField, Alert, Snackbar, Chip, Paper, Tabs, Tab } from '@mui/material';
import { 
  Download as DownloadIcon,
  Assessment as AssessmentIcon,
  TrendingUp as TrendingUpIcon,
  People as PeopleIcon,
  AccessTime as AccessTimeIcon,
  AttachMoney as MoneyIcon
} from '@mui/icons-material';
import { api } from '../../lib/api';

type TabPanelProps = {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`report-tabpanel-${index}`}
      aria-labelledby={`report-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

export default function Reports() {
  const [tabValue, setTabValue] = useState(0);
  const [dateRange, setDateRange] = useState({
    startDate: new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0],
    endDate: new Date().toISOString().split('T')[0]
  });
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  const showSnackbar = (message: string, severity: 'success' | 'error' = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleDownload = async (reportType: string, filename: string, params?: any) => {
    try {
      if (params) {
        const queryString = new URLSearchParams(params).toString();
        await api.downloadReport(`${reportType}?${queryString}`, filename);
      } else {
        await api.downloadReport(reportType, filename);
      }
      showSnackbar(`${filename} downloaded successfully`);
    } catch (error) {
      showSnackbar('Error downloading report', 'error');
    }
  };

  const basicReports = [
    { name: 'Employee Directory', type: 'employees', icon: <PeopleIcon />, color: 'primary' },
    { name: 'Departments', type: 'departments', icon: <AssessmentIcon />, color: 'secondary' },
    { name: 'Attendance', type: 'attendance', icon: <AccessTimeIcon />, color: 'info' },
    { name: 'Salary Report', type: 'salaries', icon: <MoneyIcon />, color: 'success' },
  ];

  const advancedReports = [
    { name: 'Hiring Trends', type: 'hiring-trends', icon: <TrendingUpIcon />, color: 'warning' },
    { name: 'Department Growth', type: 'department-growth', icon: <AssessmentIcon />, color: 'info' },
    { name: 'Attendance Patterns', type: 'attendance-patterns', icon: <AccessTimeIcon />, color: 'secondary' },
    { name: 'Performance Metrics', type: 'performance-metrics', icon: <TrendingUpIcon />, color: 'success' },
  ];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Reports & Analytics
      </Typography>
      
      <Paper sx={{ width: '100%' }}>
        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs value={tabValue} onChange={(e, newValue) => setTabValue(newValue)}>
            <Tab label="Basic Reports" />
            <Tab label="Advanced Analytics" />
            <Tab label="Custom Reports" />
          </Tabs>
        </Box>

        <TabPanel value={tabValue} index={0}>
          <Typography variant="h6" gutterBottom>
            Standard Reports
          </Typography>
          <Grid container spacing={3}>
            {basicReports.map((report) => (
              <Grid item xs={12} sm={6} md={3} key={report.type}>
                <Card>
                  <CardContent>
                    <Box display="flex" alignItems="center" justifyContent="center" sx={{ mb: 2 }}>
                      <Box color={`${report.color}.main`} fontSize="3rem">
                        {report.icon}
                      </Box>
                    </Box>
                    <Typography variant="h6" align="center" gutterBottom>
                      {report.name}
                    </Typography>
                    <Button
                      variant="contained"
                      fullWidth
                      startIcon={<DownloadIcon />}
                      onClick={() => handleDownload(report.type, `${report.type}.csv`)}
                      color={report.color as any}
                    >
                      Download CSV
                    </Button>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </TabPanel>

        <TabPanel value={tabValue} index={1}>
          <Typography variant="h6" gutterBottom>
            Advanced Analytics & Insights
          </Typography>
          <Grid container spacing={3}>
            {advancedReports.map((report) => (
              <Grid item xs={12} sm={6} md={3} key={report.type}>
                <Card>
                  <CardContent>
                    <Box display="flex" alignItems="center" justifyContent="center" sx={{ mb: 2 }}>
                      <Box color={`${report.color}.main`} fontSize="3rem">
                        {report.icon}
                      </Box>
                    </Box>
                    <Typography variant="h6" align="center" gutterBottom>
                      {report.name}
                    </Typography>
                    <Button
                      variant="contained"
                      fullWidth
                      startIcon={<DownloadIcon />}
                      onClick={() => handleDownload(report.type, `${report.type}.csv`)}
                      color={report.color as any}
                    >
                      Download Report
                    </Button>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </TabPanel>

        <TabPanel value={tabValue} index={2}>
          <Typography variant="h6" gutterBottom>
            Custom Date Range Reports
          </Typography>
          <Box sx={{ mb: 3 }}>
            <Grid container spacing={2} alignItems="center">
              <Grid item xs={12} sm={4}>
                <TextField
                  label="Start Date"
                  type="date"
                  value={dateRange.startDate}
                  onChange={(e) => setDateRange({ ...dateRange, startDate: e.target.value })}
                  InputLabelProps={{ shrink: true }}
                  fullWidth
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <TextField
                  label="End Date"
                  type="date"
                  value={dateRange.endDate}
                  onChange={(e) => setDateRange({ ...dateRange, endDate: e.target.value })}
                  InputLabelProps={{ shrink: true }}
                  fullWidth
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <Button
                  variant="contained"
                  fullWidth
                  startIcon={<DownloadIcon />}
                  onClick={() => handleDownload('attendance', 'custom-attendance.csv', {
                    startDate: dateRange.startDate,
                    endDate: dateRange.endDate
                  })}
                >
                  Generate Report
                </Button>
              </Grid>
            </Grid>
          </Box>

          <Alert severity="info" sx={{ mb: 2 }}>
            <Typography variant="body2">
              <strong>Report Features:</strong>
            </Typography>
            <ul>
              <li>Employee directory with contact information</li>
              <li>Department-wise employee distribution</li>
              <li>Attendance tracking with check-in/out times</li>
              <li>Salary analysis and reporting</li>
              <li>Hiring trend analysis over time</li>
              <li>Department growth tracking</li>
              <li>Attendance pattern analysis</li>
              <li>Performance metrics with PDF export</li>
            </ul>
          </Alert>

          <Grid container spacing={2}>
            <Grid item xs={12} sm={6}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Quick Actions
                  </Typography>
                  <Stack spacing={1}>
                    <Button 
                      variant="outlined" 
                      startIcon={<DownloadIcon />}
                      onClick={() => handleDownload('employees', 'all-employees.csv')}
                    >
                      Export All Employees
                    </Button>
                    <Button 
                      variant="outlined" 
                      startIcon={<DownloadIcon />}
                      onClick={() => handleDownload('attendance', 'monthly-attendance.csv')}
                    >
                      Monthly Attendance
                    </Button>
                    <Button 
                      variant="outlined" 
                      startIcon={<DownloadIcon />}
                      onClick={() => handleDownload('salaries', 'salary-summary.csv')}
                    >
                      Salary Summary
                    </Button>
                  </Stack>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} sm={6}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Report Formats
                  </Typography>
                  <Stack spacing={1}>
                    <Chip label="CSV" color="primary" />
                    <Chip label="PDF" color="secondary" />
                    <Chip label="Excel" color="success" />
                  </Stack>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </TabPanel>
      </Paper>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
      >
        <Alert 
          onClose={() => setSnackbar({ ...snackbar, open: false })} 
          severity={snackbar.severity}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}


