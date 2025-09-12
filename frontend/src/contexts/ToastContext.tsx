import { createContext, useContext, useState, type ReactNode } from 'react';
import { Snackbar, Alert } from '@mui/material';

interface ToastContextType {
  showToast: (message: string, severity: 'success' | 'error' | 'warning' | 'info') => void;
  hideToast: () => void;
}

const ToastContext = createContext<ToastContextType | undefined>(undefined);

interface ToastProviderProps {
  children: ReactNode;
}

export function ToastProvider({ children }: ToastProviderProps) {
  const [toast, setToast] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error' | 'warning' | 'info',
  });

  const showToast = (message: string, severity: 'success' | 'error' | 'warning' | 'info' = 'success') => {
    setToast({ open: true, message, severity });
  };

  const hideToast = () => {
    setToast(prev => ({ ...prev, open: false }));
  };

  return (
    <ToastContext.Provider value={{ showToast, hideToast }}>
      {children}
      <Snackbar
        open={toast.open}
        autoHideDuration={6000}
        onClose={hideToast}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        sx={{
          '& .MuiSnackbar-root': {
            position: 'fixed',
            bottom: 16,
            right: 16,
            zIndex: 9999,
          }
        }}
      >
        <Alert
          onClose={hideToast}
          severity={toast.severity}
          sx={{ 
            borderRadius: 2,
            minWidth: 300,
            boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
          }}
        >
          {toast.message}
        </Alert>
      </Snackbar>
    </ToastContext.Provider>
  );
}

export function useToast() {
  const context = useContext(ToastContext);
  if (context === undefined) {
    throw new Error('useToast must be used within a ToastProvider');
  }
  return context;
}
