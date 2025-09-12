import { useEffect, useRef, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/auth';
import { useToast } from '../contexts/ToastContext';

interface UseAutoLogoutOptions {
  warningTimeMs?: number; // Time before expiry to show warning (default: 5 minutes)
  checkIntervalMs?: number; // How often to check token expiry (default: 30 seconds)
  enabled?: boolean; // Whether auto logout is enabled (default: true)
}

export function useAutoLogout(options: UseAutoLogoutOptions = {}) {
  const {
    warningTimeMs = 5 * 60 * 1000, // 5 minutes
    checkIntervalMs = 30 * 1000, // 30 seconds
    enabled = true,
  } = options;

  const navigate = useNavigate();
  const { showToast } = useToast();
  const { token, isTokenExpired, getTimeToExpiry, logout } = useAuthStore();
  
  const intervalRef = useRef<NodeJS.Timeout | null>(null);
  const warningShownRef = useRef(false);
  const isLoggingOutRef = useRef(false);

  const performLogout = useCallback(() => {
    if (isLoggingOutRef.current) return;
    isLoggingOutRef.current = true;
    
    logout();
    showToast('Your session has expired. Please log in again.', 'error');
    navigate('/login', { replace: true });
    
    // Reset the flag after a delay
    setTimeout(() => {
      isLoggingOutRef.current = false;
    }, 1000);
  }, [logout, navigate, showToast]);

  const showExpiryWarning = useCallback(() => {
    if (warningShownRef.current) return;
    warningShownRef.current = true;
    
    const remainingMinutes = Math.ceil(getTimeToExpiry() / (60 * 1000));
    showToast(
      `Your session will expire in ${remainingMinutes} minute${remainingMinutes !== 1 ? 's' : ''}. Please save your work.`,
      'warning'
    );
  }, [getTimeToExpiry, showToast]);

  const checkTokenExpiry = useCallback(() => {
    if (!token) return;

    const timeToExpiry = getTimeToExpiry();
    
    // If token is already expired, logout immediately
    if (isTokenExpired()) {
      performLogout();
      return;
    }

    // Show warning if approaching expiry and warning hasn't been shown
    if (timeToExpiry <= warningTimeMs && !warningShownRef.current) {
      showExpiryWarning();
    }
  }, [token, isTokenExpired, getTimeToExpiry, warningTimeMs, performLogout, showExpiryWarning]);

  // Start/stop the monitoring interval
  useEffect(() => {
    if (!enabled || !token) {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
      warningShownRef.current = false;
      return;
    }

    // Initial check
    checkTokenExpiry();

    // Set up interval for periodic checks
    intervalRef.current = setInterval(checkTokenExpiry, checkIntervalMs);

    return () => {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
    };
  }, [enabled, token, checkTokenExpiry, checkIntervalMs]);

  // Reset warning flag when token changes (e.g., after login)
  useEffect(() => {
    warningShownRef.current = false;
  }, [token]);

  // Cleanup on unmount
  useEffect(() => {
    return () => {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
      }
    };
  }, []);

  return {
    isTokenExpired,
    getTimeToExpiry,
    timeToExpiryMs: getTimeToExpiry(),
    performLogout,
  };
}
