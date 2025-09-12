import { Snackbar, Alert } from '@mui/material';

interface NotificationSnackbarProps {
  open: boolean;
  onClose: () => void;
  message: string;
  severity: 'success' | 'error' | 'warning' | 'info';
  autoHideDuration?: number;
}

export default function NotificationSnackbar({
  open,
  onClose,
  message,
  severity,
  autoHideDuration = 6000,
}: NotificationSnackbarProps) {
  return (
    <Snackbar
      open={open}
      autoHideDuration={autoHideDuration}
      onClose={onClose}
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
        onClose={onClose}
        severity={severity}
        sx={{ 
          borderRadius: 2,
          minWidth: 300,
          boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
        }}
      >
        {message}
      </Alert>
    </Snackbar>
  );
}
