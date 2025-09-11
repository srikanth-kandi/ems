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
          borderRadius: 3,
          boxShadow: '0 8px 32px rgba(0,0,0,0.12)',
        }
      }}
    >
      <DialogTitle sx={{ 
        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
        color: 'white',
        fontWeight: 600,
      }}>
        {title}
      </DialogTitle>
      <DialogContent sx={{ p: 3 }}>
        <Stack spacing={3} sx={{ mt: 1 }}>
          {children}
        </Stack>
      </DialogContent>
      <DialogActions sx={{ p: 3, pt: 0 }}>
        <Button 
          onClick={onClose}
          disabled={loading}
          sx={{ 
            color: 'text.secondary',
            '&:hover': { bgcolor: 'grey.100' }
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
          }}
        >
          {saveButtonText}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
