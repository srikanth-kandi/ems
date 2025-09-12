import { useState } from "react";
import {
  Box,
  Button,
  Stack,
  Typography,
  Card,
  CardContent,
  TextField,
  Alert,
  Snackbar,
  Chip,
  Paper,
  Tabs,
  Tab,
  Divider,
} from "@mui/material";
import {
  Download as DownloadIcon,
  Assessment as AssessmentIcon,
  TrendingUp as TrendingUpIcon,
  People as PeopleIcon,
  AccessTime as AccessTimeIcon,
  AttachMoney as MoneyIcon,
  PictureAsPdf as PdfIcon,
  TableChart as ExcelIcon,
  FileDownload as CsvIcon,
  Business as BusinessIcon,
  Analytics as AnalyticsIcon,
  BarChart as BarChartIcon,
} from "@mui/icons-material";
import { api } from "../../lib/api";

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
    startDate: new Date(new Date().getFullYear(), 0, 1)
      .toISOString()
      .split("T")[0],
    endDate: new Date().toISOString().split("T")[0],
  });
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success" as "success" | "error",
  });

  const showSnackbar = (
    message: string,
    severity: "success" | "error" = "success"
  ) => {
    setSnackbar({ open: true, message, severity });
  };

  const handleDownload = async (
    reportType: string,
    filename: string,
    format: 'csv' | 'pdf' | 'excel' = 'csv',
    params?: Record<string, string>
  ) => {
    try {
      const endpoint = format === 'csv' ? reportType : `${reportType}/${format}`;
      const finalFilename = filename.replace(/\.[^/.]+$/, `.${format === 'csv' ? 'csv' : format === 'pdf' ? 'pdf' : 'xlsx'}`);
      
      if (params) {
        const queryString = new URLSearchParams(params).toString();
        await api.downloadReport(`${endpoint}?${queryString}`, finalFilename);
      } else {
        await api.downloadReport(endpoint, finalFilename);
      }
      showSnackbar(`${finalFilename} downloaded successfully`);
    } catch {
      showSnackbar("Error downloading report", "error");
    }
  };

  const basicReports = [
    {
      name: "Employee Directory",
      type: "employees",
      icon: <PeopleIcon />,
      color: "primary",
      description: "Complete employee information and contact details",
    },
    {
      name: "Departments",
      type: "departments",
      icon: <BusinessIcon />,
      color: "secondary",
      description: "Department structure and employee distribution",
    },
    {
      name: "Attendance",
      type: "attendance",
      icon: <AccessTimeIcon />,
      color: "info",
      description: "Employee attendance records and time tracking",
    },
    {
      name: "Salary Report",
      type: "salaries",
      icon: <MoneyIcon />,
      color: "success",
      description: "Salary information and compensation analysis",
    },
  ];

  const advancedReports = [
    {
      name: "Hiring Trends",
      type: "hiring-trends",
      icon: <TrendingUpIcon />,
      color: "warning",
      description: "Analysis of hiring patterns over time",
    },
    {
      name: "Department Growth",
      type: "department-growth",
      icon: <BarChartIcon />,
      color: "info",
      description: "Department expansion and growth metrics",
    },
    {
      name: "Attendance Patterns",
      type: "attendance-patterns",
      icon: <AnalyticsIcon />,
      color: "secondary",
      description: "Employee attendance behavior analysis",
    },
    {
      name: "Performance Metrics",
      type: "performance-metrics",
      icon: <AssessmentIcon />,
      color: "success",
      description: "Employee performance evaluation data",
    },
  ];

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
              📈 Reports & Analytics
            </Typography>
            <Typography 
              variant="h6" 
              sx={{ 
                opacity: 0.9, 
                fontWeight: 400,
                fontSize: { xs: "1rem", sm: "1.1rem", md: "1.25rem" },
              }}
            >
              Generate comprehensive reports and gain insights into your workforce
            </Typography>
          </Box>
        </CardContent>
      </Card>

      <Paper 
        sx={{ 
          width: "100%",
          maxWidth: { xs: "100%", sm: "100%", md: "1200px", lg: "1400px", xl: "1600px" },
          borderRadius: { xs: 2, sm: 3, md: 4 },
          boxShadow: '0 8px 32px rgba(0,0,0,0.08)',
          overflow: 'hidden',
          mx: "auto",
        }}
      >
        <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
            <Tabs
            value={tabValue}
            onChange={(_, newValue) => setTabValue(newValue)}
            sx={{
              '& .MuiTab-root': {
                fontWeight: 600,
                fontSize: '1rem',
                textTransform: 'none',
                minHeight: 60,
              },
              '& .Mui-selected': {
                color: 'primary.main',
              },
            }}
          >
            <Tab label="Basic Reports" />
            <Tab label="Advanced Analytics" />
            <Tab label="Custom Reports" />
          </Tabs>
        </Box>
        <TabPanel value={tabValue} index={0}>
          <Box sx={{ p: { xs: 2, sm: 3, md: 4 } }}>
            <Typography
              variant="h5"
              gutterBottom
              sx={{ 
                fontWeight: 600, 
                color: "primary.main",
                mb: { xs: 2, sm: 3, md: 4 },
                fontFamily: 'Montserrat, Roboto, Arial',
                textAlign: "center",
                fontSize: { xs: "1.3rem", sm: "1.5rem", md: "1.75rem" },
              }}
            >
              Standard Reports
            </Typography>
            <Box 
              sx={{ 
                display: 'grid',
                gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)', md: 'repeat(4, 1fr)' },
                gap: { xs: 2, sm: 3, md: 4 },
                width: '100%',
              }}
            >
              {basicReports.map((report) => (
                <Box key={report.type}>
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
                    <CardContent sx={{ p: 3, height: '100%', display: 'flex', flexDirection: 'column' }}>
                      <Box
                        display="flex"
                        alignItems="center"
                        justifyContent="center"
                        sx={{ 
                          mb: 2,
                          p: 2,
                          borderRadius: 2,
                          background: `linear-gradient(135deg, ${report.color === 'primary' ? '#667eea' : report.color === 'secondary' ? '#f093fb' : report.color === 'info' ? '#4facfe' : '#43e97b'} 0%, ${report.color === 'primary' ? '#764ba2' : report.color === 'secondary' ? '#f5576c' : report.color === 'info' ? '#00f2fe' : '#38f9d7'} 100%)`,
                          color: 'white',
                        }}
                      >
                        <Box fontSize="2.5rem">
                          {report.icon}
                        </Box>
                      </Box>
                      <Typography 
                        variant="h6" 
                        align="center" 
                        gutterBottom
                        sx={{ 
                          fontWeight: 600,
                          mb: 1,
                          color: 'text.primary',
                        }}
                      >
                        {report.name}
                      </Typography>
                      <Typography 
                        variant="body2" 
                        align="center" 
                        sx={{ 
                          color: 'text.secondary',
                          mb: 3,
                          flexGrow: 1,
                        }}
                      >
                        {report.description}
                      </Typography>
                      <Stack spacing={1}>
                        <Button
                          variant="contained"
                          fullWidth
                          startIcon={<CsvIcon />}
                          onClick={() => handleDownload(report.type, `${report.type}.csv`, 'csv')}
                          sx={{
                            background: `linear-gradient(135deg, ${report.color === 'primary' ? '#667eea' : report.color === 'secondary' ? '#f093fb' : report.color === 'info' ? '#4facfe' : '#43e97b'} 0%, ${report.color === 'primary' ? '#764ba2' : report.color === 'secondary' ? '#f5576c' : report.color === 'info' ? '#00f2fe' : '#38f9d7'} 100%)`,
                            '&:hover': {
                              transform: 'translateY(-1px)',
                              boxShadow: '0 4px 12px rgba(0,0,0,0.2)',
                            },
                            transition: 'all 0.3s ease',
                          }}
                        >
                          Download CSV
                        </Button>
                        <Stack direction="row" spacing={1}>
                          <Button
                            variant="outlined"
                            size="small"
                            startIcon={<PdfIcon />}
                            onClick={() => handleDownload(report.type, `${report.type}.pdf`, 'pdf')}
                            sx={{ flex: 1 }}
                          >
                            PDF
                          </Button>
                          <Button
                            variant="outlined"
                            size="small"
                            startIcon={<ExcelIcon />}
                            onClick={() => handleDownload(report.type, `${report.type}.xlsx`, 'excel')}
                            sx={{ flex: 1 }}
                          >
                            Excel
                          </Button>
                        </Stack>
                      </Stack>
                    </CardContent>
                  </Card>
                </Box>
              ))}
            </Box>
          </Box>
        </TabPanel>
        <TabPanel value={tabValue} index={1}>
          <Box sx={{ p: { xs: 2, sm: 3, md: 4 } }}>
            <Typography
              variant="h5"
              gutterBottom
              sx={{ 
                fontWeight: 600, 
                color: "primary.main",
                mb: { xs: 2, sm: 3, md: 4 },
                fontFamily: 'Montserrat, Roboto, Arial',
                textAlign: "center",
                fontSize: { xs: "1.3rem", sm: "1.5rem", md: "1.75rem" },
              }}
            >
              Advanced Analytics & Insights
            </Typography>
            <Box 
              sx={{ 
                display: 'grid',
                gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)', md: 'repeat(4, 1fr)' },
                gap: { xs: 2, sm: 3, md: 4 },
                width: '100%',
              }}
            >
              {advancedReports.map((report) => (
                <Box key={report.type}>
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
                    <CardContent sx={{ p: 3, height: '100%', display: 'flex', flexDirection: 'column' }}>
                      <Box
                        display="flex"
                        alignItems="center"
                        justifyContent="center"
                        sx={{ 
                          mb: 2,
                          p: 2,
                          borderRadius: 2,
                          background: `linear-gradient(135deg, ${report.color === 'warning' ? '#f093fb' : report.color === 'info' ? '#4facfe' : report.color === 'secondary' ? '#f093fb' : '#43e97b'} 0%, ${report.color === 'warning' ? '#f5576c' : report.color === 'info' ? '#00f2fe' : report.color === 'secondary' ? '#f5576c' : '#38f9d7'} 100%)`,
                          color: 'white',
                        }}
                      >
                        <Box fontSize="2.5rem">
                          {report.icon}
                        </Box>
                      </Box>
                      <Typography 
                        variant="h6" 
                        align="center" 
                        gutterBottom
                        sx={{ 
                          fontWeight: 600,
                          mb: 1,
                          color: 'text.primary',
                        }}
                      >
                        {report.name}
                      </Typography>
                      <Typography 
                        variant="body2" 
                        align="center" 
                        sx={{ 
                          color: 'text.secondary',
                          mb: 3,
                          flexGrow: 1,
                        }}
                      >
                        {report.description}
                      </Typography>
                      <Stack spacing={1}>
                        <Button
                          variant="contained"
                          fullWidth
                          startIcon={<CsvIcon />}
                          onClick={() => handleDownload(report.type, `${report.type}.csv`, 'csv')}
                          sx={{
                            background: `linear-gradient(135deg, ${report.color === 'warning' ? '#f093fb' : report.color === 'info' ? '#4facfe' : report.color === 'secondary' ? '#f093fb' : '#43e97b'} 0%, ${report.color === 'warning' ? '#f5576c' : report.color === 'info' ? '#00f2fe' : report.color === 'secondary' ? '#f5576c' : '#38f9d7'} 100%)`,
                            '&:hover': {
                              transform: 'translateY(-1px)',
                              boxShadow: '0 4px 12px rgba(0,0,0,0.2)',
                            },
                            transition: 'all 0.3s ease',
                          }}
                        >
                          Download CSV
                        </Button>
                        <Stack direction="row" spacing={1}>
                          <Button
                            variant="outlined"
                            size="small"
                            startIcon={<PdfIcon />}
                            onClick={() => handleDownload(report.type, `${report.type}.pdf`, 'pdf')}
                            sx={{ flex: 1 }}
                          >
                            PDF
                          </Button>
                          <Button
                            variant="outlined"
                            size="small"
                            startIcon={<ExcelIcon />}
                            onClick={() => handleDownload(report.type, `${report.type}.xlsx`, 'excel')}
                            sx={{ flex: 1 }}
                          >
                            Excel
                          </Button>
                        </Stack>
                      </Stack>
                    </CardContent>
                  </Card>
                </Box>
              ))}
            </Box>
          </Box>
        </TabPanel>
        <TabPanel value={tabValue} index={2}>
          <Box sx={{ p: { xs: 2, sm: 3, md: 4 } }}>
            <Typography
              variant="h5"
              gutterBottom
              sx={{ 
                fontWeight: 600, 
                color: "primary.main",
                mb: { xs: 2, sm: 3, md: 4 },
                fontFamily: 'Montserrat, Roboto, Arial',
                textAlign: "center",
                fontSize: { xs: "1.3rem", sm: "1.5rem", md: "1.75rem" },
              }}
            >
              Custom Date Range Reports
            </Typography>
            
            {/* Date Range Selector */}
            <Card sx={{ mb: 4, borderRadius: 3, boxShadow: '0 4px 20px rgba(0,0,0,0.08)' }}>
              <CardContent sx={{ p: 3 }}>
                <Typography variant="h6" gutterBottom sx={{ fontWeight: 600, mb: 3 }}>
                  Select Date Range
                </Typography>
                <Box 
                  sx={{ 
                    display: 'grid',
                    gridTemplateColumns: { xs: '1fr', sm: 'repeat(3, 1fr)' },
                    gap: { xs: 2, sm: 3 },
                    alignItems: 'center',
                    width: '100%',
                  }}
                >
                  <Box>
                    <TextField
                      label="Start Date"
                      type="date"
                      value={dateRange.startDate}
                      onChange={(e) =>
                        setDateRange({ ...dateRange, startDate: e.target.value })
                      }
                      InputLabelProps={{ shrink: true }}
                      fullWidth
                      sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2 } }}
                    />
                  </Box>
                  <Box>
                    <TextField
                      label="End Date"
                      type="date"
                      value={dateRange.endDate}
                      onChange={(e) =>
                        setDateRange({ ...dateRange, endDate: e.target.value })
                      }
                      InputLabelProps={{ shrink: true }}
                      fullWidth
                      sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2 } }}
                    />
                  </Box>
                  <Box>
                    <Stack direction="row" spacing={1}>
                      <Button
                        variant="contained"
                        startIcon={<CsvIcon />}
                        onClick={() =>
                          handleDownload("attendance", "custom-attendance.csv", 'csv', {
                            startDate: dateRange.startDate,
                            endDate: dateRange.endDate,
                          })
                        }
                        sx={{ 
                          flex: 1,
                          background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                          '&:hover': {
                            transform: 'translateY(-1px)',
                            boxShadow: '0 4px 12px rgba(102, 126, 234, 0.4)',
                          },
                          transition: 'all 0.3s ease',
                        }}
                      >
                        CSV
                      </Button>
                      <Button
                        variant="outlined"
                        startIcon={<PdfIcon />}
                        onClick={() =>
                          handleDownload("attendance", "custom-attendance.pdf", 'pdf', {
                            startDate: dateRange.startDate,
                            endDate: dateRange.endDate,
                          })
                        }
                        sx={{ flex: 1 }}
                      >
                        PDF
                      </Button>
                      <Button
                        variant="outlined"
                        startIcon={<ExcelIcon />}
                        onClick={() =>
                          handleDownload("attendance", "custom-attendance.xlsx", 'excel', {
                            startDate: dateRange.startDate,
                            endDate: dateRange.endDate,
                          })
                        }
                        sx={{ flex: 1 }}
                      >
                        Excel
                      </Button>
                    </Stack>
                  </Box>
                </Box>
              </CardContent>
            </Card>

            {/* Features Info */}
            <Alert 
              severity="info" 
              sx={{ 
                mb: 4, 
                borderRadius: 3,
                '& .MuiAlert-message': { width: '100%' }
              }}
            >
              <Typography variant="h6" gutterBottom sx={{ fontWeight: 600 }}>
                Report Features
              </Typography>
              <Box 
                sx={{ 
                  display: 'grid',
                  gridTemplateColumns: { xs: '1fr', md: 'repeat(2, 1fr)' },
                  gap: { xs: 2, sm: 3 },
                  width: '100%',
                }}
              >
                <Box>
                  <Typography variant="body2" component="div">
                    <strong>Basic Reports:</strong>
                    <ul style={{ marginTop: 8, marginBottom: 0 }}>
                      <li>Employee directory with contact information</li>
                      <li>Department-wise employee distribution</li>
                      <li>Attendance tracking with check-in/out times</li>
                      <li>Salary analysis and reporting</li>
                    </ul>
                  </Typography>
                </Box>
                <Box>
                  <Typography variant="body2" component="div">
                    <strong>Advanced Analytics:</strong>
                    <ul style={{ marginTop: 8, marginBottom: 0 }}>
                      <li>Hiring trend analysis over time</li>
                      <li>Department growth tracking</li>
                      <li>Attendance pattern analysis</li>
                      <li>Performance metrics with PDF export</li>
                    </ul>
                  </Typography>
                </Box>
              </Box>
            </Alert>

            {/* Quick Actions */}
            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'repeat(2, 1fr)' }, gap: 3 }}>
              <Box>
                <Card sx={{ borderRadius: 3, boxShadow: '0 4px 20px rgba(0,0,0,0.08)' }}>
                  <CardContent sx={{ p: 3 }}>
                    <Typography
                      variant="h6"
                      gutterBottom
                      sx={{ 
                        fontWeight: 600, 
                        color: "primary.main",
                        mb: 3,
                        display: 'flex',
                        alignItems: 'center',
                        gap: 1,
                      }}
                    >
                      <DownloadIcon />
                      Quick Actions
                    </Typography>
                    <Stack spacing={2}>
                      <Button
                        variant="outlined"
                        startIcon={<CsvIcon />}
                        onClick={() => handleDownload("employees", "all-employees.csv", 'csv')}
                        fullWidth
                        sx={{ justifyContent: 'flex-start', borderRadius: 2 }}
                      >
                        Export All Employees (CSV)
                      </Button>
                      <Button
                        variant="outlined"
                        startIcon={<PdfIcon />}
                        onClick={() => handleDownload("employees", "all-employees.pdf", 'pdf')}
                        fullWidth
                        sx={{ justifyContent: 'flex-start', borderRadius: 2 }}
                      >
                        Export All Employees (PDF)
                      </Button>
                      <Button
                        variant="outlined"
                        startIcon={<ExcelIcon />}
                        onClick={() => handleDownload("employees", "all-employees.xlsx", 'excel')}
                        fullWidth
                        sx={{ justifyContent: 'flex-start', borderRadius: 2 }}
                      >
                        Export All Employees (Excel)
                      </Button>
                      <Divider />
                      <Button
                        variant="outlined"
                        startIcon={<CsvIcon />}
                        onClick={() => handleDownload("attendance", "monthly-attendance.csv", 'csv')}
                        fullWidth
                        sx={{ justifyContent: 'flex-start', borderRadius: 2 }}
                      >
                        Monthly Attendance Report
                      </Button>
                      <Button
                        variant="outlined"
                        startIcon={<CsvIcon />}
                        onClick={() => handleDownload("salaries", "salary-summary.csv", 'csv')}
                        fullWidth
                        sx={{ justifyContent: 'flex-start', borderRadius: 2 }}
                      >
                        Salary Summary Report
                      </Button>
                    </Stack>
                  </CardContent>
                </Card>
              </Box>
              <Box>
                <Card sx={{ borderRadius: 3, boxShadow: '0 4px 20px rgba(0,0,0,0.08)' }}>
                  <CardContent sx={{ p: 3 }}>
                    <Typography
                      variant="h6"
                      gutterBottom
                      sx={{ 
                        fontWeight: 600, 
                        color: "secondary.main",
                        mb: 3,
                        display: 'flex',
                        alignItems: 'center',
                        gap: 1,
                      }}
                    >
                      <AssessmentIcon />
                      Supported Formats
                    </Typography>
                    <Stack spacing={2}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                        <Chip 
                          icon={<CsvIcon />} 
                          label="CSV" 
                          color="primary" 
                          variant="outlined"
                          sx={{ borderRadius: 2 }}
                        />
                        <Typography variant="body2" color="text.secondary">
                          Comma-separated values for data analysis
                        </Typography>
                      </Box>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                        <Chip 
                          icon={<PdfIcon />} 
                          label="PDF" 
                          color="error" 
                          variant="outlined"
                          sx={{ borderRadius: 2 }}
                        />
                        <Typography variant="body2" color="text.secondary">
                          Portable document format for sharing
                        </Typography>
                      </Box>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                        <Chip 
                          icon={<ExcelIcon />} 
                          label="Excel" 
                          color="success" 
                          variant="outlined"
                          sx={{ borderRadius: 2 }}
                        />
                        <Typography variant="body2" color="text.secondary">
                          Spreadsheet format for detailed analysis
                        </Typography>
                      </Box>
                    </Stack>
                  </CardContent>
                </Card>
              </Box>
            </Box>
          </Box>
        </TabPanel>
      </Paper>
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
