import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../../lib/api';
import { useAuthStore } from '../../store/auth';
import { Box, Button, Card, CardContent, TextField, Typography } from '@mui/material';

export default function Login() {
  const navigate = useNavigate();
  const setAuth = useAuthStore((s) => s.setAuth);
  const [username, setUsername] = useState('admin');
  const [password, setPassword] = useState('admin123');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const res = await api.login(username, password);
      setAuth({ token: res.token, username: res.username, role: res.role });
      navigate('/');
    } catch (err) {
      setError('Invalid credentials');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '80vh' }}>
      <Card sx={{ width: 360 }}>
        <CardContent>
          <Typography variant="h5" sx={{ mb: 2 }}>Login</Typography>
          <form onSubmit={onSubmit}>
            <TextField fullWidth label="Username" margin="normal" value={username} onChange={(e) => setUsername(e.target.value)} />
            <TextField fullWidth label="Password" type="password" margin="normal" value={password} onChange={(e) => setPassword(e.target.value)} />
            {error && <Typography color="error" variant="body2">{error}</Typography>}
            <Button type="submit" variant="contained" fullWidth disabled={loading} sx={{ mt: 2 }}>
              {loading ? 'Signing in...' : 'Sign In'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
}


