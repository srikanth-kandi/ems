import React from 'react';
import {
  Card,
  CardContent,
  Typography,
  Stack,
  Box,
  Fade,
} from '@mui/material';

interface PageHeaderProps {
  title: string;
  subtitle?: string;
  actions?: React.ReactNode;
}

export default function PageHeader({ title, subtitle, actions }: PageHeaderProps) {
  return (
    <Fade in timeout={600}>
      <Card 
        sx={{ 
          mb: 3, 
          background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
          color: 'white',
          boxShadow: '0 8px 32px rgba(102, 126, 234, 0.3)',
        }}
      >
        <CardContent sx={{ p: 3 }}>
          <Stack 
            direction={{ xs: 'column', md: 'row' }} 
            justifyContent="space-between" 
            alignItems={{ xs: 'stretch', md: 'center' }}
            spacing={2}
          >
            <Box>
              <Typography 
                variant="h4" 
                fontWeight="700" 
                sx={{ 
                  mb: 1,
                  fontFamily: 'Montserrat, Roboto, Arial',
                  textShadow: '0 2px 4px rgba(0,0,0,0.1)',
                }}
              >
                {title}
              </Typography>
              {subtitle && (
                <Typography variant="body1" sx={{ opacity: 0.9 }}>
                  {subtitle}
                </Typography>
              )}
            </Box>
            {actions && (
              <Stack direction="row" spacing={2}>
                {actions}
              </Stack>
            )}
          </Stack>
        </CardContent>
      </Card>
    </Fade>
  );
}
