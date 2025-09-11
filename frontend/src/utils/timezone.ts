/**
 * Timezone utility functions for converting UTC datetime values to local timezone
 */

/**
 * Converts a UTC datetime string to local timezone
 * @param utcDateTime - UTC datetime string from backend
 * @param options - Intl.DateTimeFormatOptions for formatting
 * @returns Formatted local datetime string
 */
export function convertUtcToLocal(
  utcDateTime: string | null | undefined,
  options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hour12: false
  }
): string {
  if (!utcDateTime) return '-';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleString(undefined, options);
  } catch (error) {
    console.error('Error converting UTC to local time:', error);
    return '-';
  }
}

/**
 * Converts a UTC datetime string to local time for display
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted local time string (HH:MM:SS)
 */
export function convertUtcToLocalTime(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '-';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleTimeString(undefined, {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false
    });
  } catch (error) {
    console.error('Error converting UTC to local time:', error);
    return '-';
  }
}

/**
 * Converts a UTC datetime string to local date for display
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted local date string (YYYY-MM-DD)
 */
export function convertUtcToLocalDate(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '-';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleDateString(undefined, {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    });
  } catch (error) {
    console.error('Error converting UTC to local date:', error);
    return '-';
  }
}

/**
 * Converts a UTC datetime string to local datetime for display
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted local datetime string (YYYY-MM-DD HH:MM:SS)
 */
export function convertUtcToLocalDateTime(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '-';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleString(undefined, {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false
    });
  } catch (error) {
    console.error('Error converting UTC to local datetime:', error);
    return '-';
  }
}

/**
 * Converts a UTC datetime string to relative time (e.g., "2 hours ago")
 * @param utcDateTime - UTC datetime string from backend
 * @returns Relative time string
 */
export function convertUtcToRelativeTime(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '-';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '-';
    
    const now = new Date();
    const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);
    
    if (diffInSeconds < 60) return 'Just now';
    if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)} minutes ago`;
    if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)} hours ago`;
    if (diffInSeconds < 2592000) return `${Math.floor(diffInSeconds / 86400)} days ago`;
    if (diffInSeconds < 31536000) return `${Math.floor(diffInSeconds / 2592000)} months ago`;
    
    return `${Math.floor(diffInSeconds / 31536000)} years ago`;
  } catch (error) {
    console.error('Error converting UTC to relative time:', error);
    return '-';
  }
}

/**
 * Gets the user's timezone
 * @returns User's timezone string
 */
export function getUserTimezone(): string {
  return Intl.DateTimeFormat().resolvedOptions().timeZone;
}

/**
 * Formats a date for form input (YYYY-MM-DD format)
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted date string for form input
 */
export function formatDateForInput(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '';
    
    return date.toISOString().split('T')[0];
  } catch (error) {
    console.error('Error formatting date for input:', error);
    return '';
  }
}

/**
 * Converts local datetime to UTC for sending to backend
 * @param localDateTime - Local datetime string
 * @returns UTC datetime string
 */
export function convertLocalToUtc(localDateTime: string): string {
  try {
    const date = new Date(localDateTime);
    if (isNaN(date.getTime())) return '';
    
    return date.toISOString();
  } catch (error) {
    console.error('Error converting local to UTC:', error);
    return '';
  }
}
