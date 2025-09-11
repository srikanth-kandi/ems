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
      const res = await api.login(username, password);
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
        minHeight: "80vh",
        bgcolor: "background.default",
      }}
    >
      <Card
        className="border-rounded shadow-lg font-montserrat"
        sx={{
          width: { xs: "95vw", sm: 360 },
          mx: "auto",
          bgcolor: "background.paper",
          boxShadow: "0 4px 24px rgba(0,0,0,0.08)",
          borderRadius: 3,
        }}
      >
        <CardContent>
          <Typography
            variant="h5"
            sx={{
              mb: 2,
              fontWeight: 700,
              color: "primary.main",
              textAlign: "center",
              fontFamily: "Montserrat, Roboto, Arial",
            }}
          >
            Login
          </Typography>
          <form onSubmit={onSubmit}>
            <TextField
              fullWidth
              label="Username"
              margin="normal"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
            <TextField
              fullWidth
              label="Password"
              type="password"
              margin="normal"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            {error && (
              <Typography color="error" variant="body2">
                {error}
              </Typography>
            )}
            <Button
              type="submit"
              variant="contained"
              fullWidth
              disabled={loading}
              sx={{ mt: 2, fontWeight: 600 }}
            >
              {loading ? "Signing in..." : "Sign In"}
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
}
