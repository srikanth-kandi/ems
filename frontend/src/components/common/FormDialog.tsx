import React from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Stack,
} from '@mui/material';

interface FormDialogProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  title: string;
  children: React.ReactNode;
  saveButtonText?: string;
  cancelButtonText?: string;
  loading?: boolean;
}

export default function FormDialog({
  open,
  onClose,
  onSave,
  title,
  children,
  saveButtonText = 'Save',
  cancelButtonText = 'Cancel',
  loading = false,
}: FormDialogProps) {
  return (
    <Dialog 
      open={open} 
      onClose={onClose} 
      fullWidth 
      maxWidth="sm"
      PaperProps={{
        sx: {
          borderRadius: { xs: 2, sm: 3, md: 4 },
          boxShadow: '0 8px 32px rgba(0,0,0,0.12)',
          mx: { xs: 2, sm: 0 },
          my: { xs: 2, sm: 0 },
        }
      }}
    >
      <DialogTitle sx={{ 
        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
        color: 'white',
        fontWeight: 600,
        textAlign: "center",
        fontSize: { xs: "1.1rem", sm: "1.25rem", md: "1.5rem" },
        py: { xs: 2, sm: 3 },
      }}>
        {title}
      </DialogTitle>
      <DialogContent sx={{ p: { xs: 2, sm: 3, md: 4 } }}>
        <Stack spacing={{ xs: 2, sm: 3 }} sx={{ mt: 1 }}>
          {children}
        </Stack>
      </DialogContent>
      <DialogActions sx={{ 
        p: { xs: 2, sm: 3, md: 4 }, 
        pt: 0,
        justifyContent: "center",
        flexDirection: { xs: "column", sm: "row" },
        gap: { xs: 1, sm: 2 },
      }}>
        <Button 
          onClick={onClose}
          disabled={loading}
          sx={{ 
            color: 'text.secondary',
            '&:hover': { bgcolor: 'grey.100' },
            width: { xs: "100%", sm: "auto" },
            py: { xs: 1.5, sm: 1 },
          }}
        >
          {cancelButtonText}
        </Button>
        <Button 
          variant="contained" 
          onClick={onSave}
          disabled={loading}
          sx={{
            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            '&:hover': {
              background: 'linear-gradient(135deg, #5a6fd8 0%, #6a4190 100%)',
              transform: 'translateY(-1px)',
              boxShadow: '0 4px 12px rgba(102, 126, 234, 0.4)',
            },
            transition: 'all 0.3s ease',
            width: { xs: "100%", sm: "auto" },
            py: { xs: 1.5, sm: 1 },
          }}
        >
          {saveButtonText}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
