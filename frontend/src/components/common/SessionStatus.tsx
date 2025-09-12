import { useState, useEffect } from 'react';
import { Chip, Tooltip } from '@mui/material';
import { AccessTime, Warning } from '@mui/icons-material';
import { useAuthStore } from '../../store/auth';

interface SessionStatusProps {
  showTimeRemaining?: boolean;
  warningThresholdMs?: number;
}

export default function SessionStatus({ 
  showTimeRemaining = true,
  warningThresholdMs = 5 * 60 * 1000 // 5 minutes
}: SessionStatusProps) {
  const { token, getTimeToExpiry, isTokenExpired } = useAuthStore();
  const [timeRemaining, setTimeRemaining] = useState<number>(0);

  useEffect(() => {
    if (!token || isTokenExpired()) {
      setTimeRemaining(0);
      return;
    }

    const updateTimeRemaining = () => {
      const remaining = getTimeToExpiry();
      setTimeRemaining(remaining);
    };

    // Update immediately
    updateTimeRemaining();

    // Update every minute
    const interval = setInterval(updateTimeRemaining, 60000);

    return () => clearInterval(interval);
  }, [token, getTimeToExpiry, isTokenExpired]);

  if (!token || !showTimeRemaining || timeRemaining <= 0) {
    return null;
  }

  const minutes = Math.floor(timeRemaining / (60 * 1000));
  const hours = Math.floor(minutes / 60);
  const remainingMinutes = minutes % 60;

  const isWarning = timeRemaining <= warningThresholdMs;
  
  const timeText = hours > 0 
    ? `${hours}h ${remainingMinutes}m`
    : `${remainingMinutes}m`;

  const tooltipText = `Session expires in ${timeText}`;

  return (
    <Tooltip title={tooltipText}>
      <Chip
        icon={isWarning ? <Warning /> : <AccessTime />}
        label={timeText}
        size="small"
        color={isWarning ? 'warning' : 'default'}
        variant={isWarning ? 'filled' : 'outlined'}
        sx={{
          fontSize: '0.75rem',
          height: 24,
          '& .MuiChip-icon': {
            fontSize: '0.875rem'
          }
        }}
      />
    </Tooltip>
  );
}
