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
          mb: { xs: 3, sm: 4, md: 5 },
          width: "100%",
          maxWidth: { xs: "100%", sm: "100%", md: "1200px", lg: "1400px" },
          background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
          color: 'white',
          boxShadow: '0 8px 32px rgba(102, 126, 234, 0.3)',
          borderRadius: { xs: 2, sm: 3, md: 4 },
        }}
      >
        <CardContent sx={{ p: { xs: 3, sm: 4, md: 5 } }}>
          <Stack 
            direction={{ xs: 'column', md: 'row' }} 
            justifyContent="space-between" 
            alignItems={{ xs: 'stretch', md: 'center' }}
            spacing={{ xs: 2, sm: 3 }}
          >
            <Box sx={{ textAlign: { xs: "center", md: "left" } }}>
              <Typography 
                variant="h4" 
                fontWeight="700" 
                sx={{ 
                  mb: 1,
                  fontFamily: 'Montserrat, Roboto, Arial',
                  textShadow: '0 2px 4px rgba(0,0,0,0.1)',
                  fontSize: { xs: "1.5rem", sm: "1.8rem", md: "2.125rem", lg: "2.5rem" },
                }}
              >
                {title}
              </Typography>
              {subtitle && (
                <Typography 
                  variant="body1" 
                  sx={{ 
                    opacity: 0.9,
                    fontSize: { xs: "0.9rem", sm: "1rem", md: "1.1rem" },
                  }}
                >
                  {subtitle}
                </Typography>
              )}
            </Box>
            {actions && (
              <Stack 
                direction={{ xs: 'column', sm: 'row' }} 
                spacing={{ xs: 1, sm: 2 }}
                sx={{ width: { xs: "100%", md: "auto" } }}
              >
                {actions}
              </Stack>
            )}
          </Stack>
        </CardContent>
      </Card>
    </Fade>
  );
}
