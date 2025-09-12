import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { api } from "../../lib/api";
import { useAuthStore } from "../../store/auth";
import { useDocumentTitle } from "../../hooks/useDocumentTitle";
import {
  Box,
  Button,
  Card,
  CardContent,
  TextField,
  Typography,
} from "@mui/material";

export default function Login() {
  const navigate = useNavigate();
  const location = useLocation();
  const setAuth = useAuthStore((s) => s.setAuth);
  const [username, setUsername] = useState("admin");
  const [password, setPassword] = useState("admin123");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Update document title and favicon
  useDocumentTitle(location.pathname);

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const res = await api.login({ username, password });
      setAuth({ token: res.token, username: res.username, role: res.role });
      navigate("/");
    } catch {
      setError("Invalid credentials");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        minHeight: "100vh",
        bgcolor: "background.default",
        p: { xs: 2, sm: 3, md: 4, lg: 5 },
        mx: "auto",
      }}
    >
      <Card
        className="border-rounded shadow-lg font-montserrat"
        sx={{
          width: { xs: "100%", sm: 400, md: 420 },
          maxWidth: 420,
          mx: "auto",
          bgcolor: "background.paper",
          boxShadow: "0 8px 32px rgba(0,0,0,0.12)",
          borderRadius: 3,
        }}
      >
        <CardContent sx={{ p: { xs: 3, sm: 4, md: 5 } }}>
          <Typography
            variant="h4"
            sx={{
              mb: 3,
              fontWeight: 700,
              color: "primary.main",
              textAlign: "center",
              fontFamily: "Montserrat, Roboto, Arial",
            }}
          >
            🔐 Login
          </Typography>
          <form onSubmit={onSubmit}>
            <TextField
              fullWidth
              label="Username"
              margin="normal"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              sx={{ mb: 2 }}
            />
            <TextField
              fullWidth
              label="Password"
              type="password"
              margin="normal"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              sx={{ mb: 2 }}
            />
            {error && (
              <Typography 
                color="error" 
                variant="body2" 
                sx={{ 
                  textAlign: "center", 
                  mb: 2,
                  p: 1,
                  bgcolor: "error.50",
                  borderRadius: 1,
                }}
              >
                {error}
              </Typography>
            )}
            <Button
              type="submit"
              variant="contained"
              fullWidth
              disabled={loading}
              sx={{ 
                mt: 2, 
                fontWeight: 600,
                py: 1.5,
                fontSize: "1.1rem",
                borderRadius: 2,
                background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
                "&:hover": {
                  background: "linear-gradient(135deg, #5a6fd8 0%, #6a4190 100%)",
                  transform: "translateY(-1px)",
                  boxShadow: "0 4px 12px rgba(102, 126, 234, 0.4)",
                },
                transition: "all 0.3s ease",
              }}
            >
              {loading ? "Signing in..." : "Sign In"}
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
}
