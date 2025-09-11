import type { ReactNode } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import {
  AppBar,
  Toolbar,
  Typography,
  Box,
  IconButton,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  useTheme,
  useMediaQuery,
  Divider,
} from "@mui/material";
import {
  Menu as MenuIcon,
  Dashboard as DashboardIcon,
  People as PeopleIcon,
  AccessTime as AccessTimeIcon,
  Assessment as AssessmentIcon,
  Business as BusinessIcon,
  Brightness4 as DarkModeIcon,
  Brightness7 as LightModeIcon,
  Logout as LogoutIcon,
} from "@mui/icons-material";
import { useState } from "react";
import { useAuthStore } from "../../store/auth";
import { useThemeStore } from "../../store/theme";

const drawerWidth = 240;

interface LayoutProps {
  children: ReactNode;
}

export default function Layout({ children }: LayoutProps) {
  const navigate = useNavigate();
  const location = useLocation();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("md"));
  const [mobileOpen, setMobileOpen] = useState(false);

  const { username, logout } = useAuthStore();
  const { mode, toggleMode } = useThemeStore();

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const menuItems = [
    { text: "Dashboard", icon: <DashboardIcon />, path: "/" },
    { text: "Employees", icon: <PeopleIcon />, path: "/employees" },
    { text: "Departments", icon: <BusinessIcon />, path: "/departments" },
    { text: "Attendance", icon: <AccessTimeIcon />, path: "/attendance" },
    { text: "Reports", icon: <AssessmentIcon />, path: "/reports" },
  ];

  const getPageTitle = () => {
    const currentItem = menuItems.find(item => item.path === location.pathname);
    return currentItem ? currentItem.text : "Employee Management System";
  };

  const drawer = (
    <Box>
      <Toolbar>
        <Typography variant="h6" noWrap component="div">
          EMS
        </Typography>
      </Toolbar>
      <Divider />
      <List>
        {menuItems.map((item) => (
          <ListItem key={item.text} disablePadding>
            <ListItemButton
              selected={location.pathname === item.path}
              onClick={() => {
                navigate(item.path);
                if (isMobile) setMobileOpen(false);
              }}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Box>
  );

  return (
    <Box
      sx={{
        display: "flex",
        minHeight: "100vh",
        bgcolor: "background.default",
      }}
    >
      <AppBar
        position="fixed"
        elevation={8}
        sx={{
          width: { md: `calc(100% - ${drawerWidth}px)` },
          ml: { md: `${drawerWidth}px` },
          bgcolor: mode === "dark" ? "grey.900" : "primary.main",
          color: mode === "dark" ? "grey.100" : "common.white",
          boxShadow: "0 4px 16px rgba(0,0,0,0.12)",
          borderBottom: "2px solid",
          borderColor: mode === "dark" ? "grey.800" : "primary.light",
        }}
      >
        <Toolbar sx={{ px: { xs: 2, md: 4 }, py: 1 }}>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={handleDrawerToggle}
            sx={{
              mr: 2,
              display: { md: "none" },
              borderRadius: 2,
              bgcolor: mode === "dark" ? "grey.800" : "primary.light",
              boxShadow: "0 2px 8px rgba(0,0,0,0.10)",
            }}
          >
            <MenuIcon />
          </IconButton>
          <Typography
            variant="h5"
            noWrap
            component="div"
            sx={{
              flexGrow: 1,
              fontWeight: 700,
              letterSpacing: 1,
              fontFamily: "Montserrat, Roboto, Arial",
              color: "inherit",
            }}
          >
            {getPageTitle()}
          </Typography>
          <IconButton
            color="inherit"
            onClick={toggleMode}
            sx={{
              mx: 1,
              borderRadius: 2,
              bgcolor: mode === "dark" ? "grey.800" : "primary.light",
              boxShadow: "0 2px 8px rgba(0,0,0,0.10)",
            }}
          >
            {mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
          </IconButton>
          <Typography
            variant="body2"
            sx={{
              mr: 2,
              fontWeight: 500,
              fontFamily: "Montserrat, Roboto, Arial",
            }}
          >
            Welcome, {username}
          </Typography>
          <IconButton
            color="inherit"
            onClick={handleLogout}
            sx={{
              borderRadius: 2,
              bgcolor: mode === "dark" ? "grey.800" : "primary.light",
              boxShadow: "0 2px 8px rgba(0,0,0,0.10)",
            }}
          >
            <LogoutIcon />
          </IconButton>
        </Toolbar>
      </AppBar>

      <Box
        component="nav"
        sx={{ width: { md: drawerWidth }, flexShrink: { md: 0 } }}
      >
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: "block", md: "none" },
            "& .MuiDrawer-paper": {
              boxSizing: "border-box",
              width: drawerWidth,
              bgcolor: mode === "dark" ? "grey.900" : "background.paper",
              color: mode === "dark" ? "grey.100" : "text.primary",
              borderRight: "2px solid",
              borderColor: mode === "dark" ? "grey.800" : "primary.light",
              boxShadow: "0 4px 16px rgba(0,0,0,0.12)",
            },
          }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: "none", md: "block" },
            "& .MuiDrawer-paper": {
              boxSizing: "border-box",
              width: drawerWidth,
              bgcolor: mode === "dark" ? "grey.900" : "background.paper",
              color: mode === "dark" ? "grey.100" : "text.primary",
              borderRight: "2px solid",
              borderColor: mode === "dark" ? "grey.800" : "primary.light",
              boxShadow: "0 4px 16px rgba(0,0,0,0.12)",
            },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: { xs: 1, sm: 2, md: 3 },
          width: { md: `calc(100% - ${drawerWidth}px)` },
          mt: 8,
          background: mode === "dark" 
            ? "linear-gradient(135deg, #0f0f23 0%, #1a1a2e 50%, #16213e 100%)"
            : "linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)",
          borderRadius: { xs: 0, md: 3 },
          boxShadow: { xs: "none", md: "0 8px 32px rgba(0,0,0,0.12)" },
          minHeight: "calc(100vh - 64px)",
          position: "relative",
          "&::before": {
            content: '""',
            position: "absolute",
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            background: mode === "dark"
              ? "radial-gradient(circle at 20% 80%, rgba(120, 119, 198, 0.1) 0%, transparent 50%), radial-gradient(circle at 80% 20%, rgba(255, 119, 198, 0.1) 0%, transparent 50%)"
              : "radial-gradient(circle at 20% 80%, rgba(120, 119, 198, 0.05) 0%, transparent 50%), radial-gradient(circle at 80% 20%, rgba(255, 119, 198, 0.05) 0%, transparent 50%)",
            borderRadius: "inherit",
            pointerEvents: "none",
          },
        }}
      >
        <Box sx={{ position: "relative", zIndex: 1 }}>
          {children}
        </Box>
      </Box>
    </Box>
  );
}
