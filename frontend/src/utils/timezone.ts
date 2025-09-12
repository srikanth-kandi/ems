/**
 * Timezone utility functions for converting UTC datetime values to user's local system timezone
 */

/**
 * Gets the user's local timezone
 * @returns User's local timezone string
 */
export function getUserTimezone(): string {
  return Intl.DateTimeFormat().resolvedOptions().timeZone;
}

/**
 * Converts a UTC datetime string to user's local timezone
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
    hour12: false,
    timeZone: getUserTimezone()
  }
): string {
  if (!utcDateTime) return '-';
  
  try {
    // Ensure the date string is properly formatted as UTC
    const utcString = utcDateTime.endsWith('Z') ? utcDateTime : `${utcDateTime}Z`;
    const date = new Date(utcString);
    
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleString(navigator.language || 'en-US', {
      ...options,
      timeZone: getUserTimezone()
    });
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
    // Ensure the date string is properly formatted as UTC
    const utcString = utcDateTime.endsWith('Z') ? utcDateTime : `${utcDateTime}Z`;
    const date = new Date(utcString);
    
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleTimeString(navigator.language || 'en-US', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false,
      timeZone: getUserTimezone()
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
    // Ensure the date string is properly formatted as UTC
    const utcString = utcDateTime.endsWith('Z') ? utcDateTime : `${utcDateTime}Z`;
    const date = new Date(utcString);
    
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleDateString(navigator.language || 'en-US', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      timeZone: getUserTimezone()
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
    // Ensure the date string is properly formatted as UTC
    const utcString = utcDateTime.endsWith('Z') ? utcDateTime : `${utcDateTime}Z`;
    const date = new Date(utcString);
    
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleString(navigator.language || 'en-US', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false,
      timeZone: getUserTimezone()
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
    // Ensure the date string is properly formatted as UTC
    const utcString = utcDateTime.endsWith('Z') ? utcDateTime : `${utcDateTime}Z`;
    const date = new Date(utcString);
    
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
 * Formats a date for form input (YYYY-MM-DD format) in local timezone
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted date string for form input in local timezone
 */
export function formatDateForInput(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '';
  
  try {
    // Ensure the date string is properly formatted as UTC
    const utcString = utcDateTime.endsWith('Z') ? utcDateTime : `${utcDateTime}Z`;
    const date = new Date(utcString);
    
    if (isNaN(date.getTime())) return '';
    
    // Convert to local timezone and format for input
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    
    return `${year}-${month}-${day}`;
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
    
    // JavaScript Date automatically handles timezone conversion to UTC
    return date.toISOString();
  } catch (error) {
    console.error('Error converting local to UTC:', error);
    return '';
  }
}

/**
 * Creates a new Date object from UTC string ensuring proper parsing
 * @param utcDateTime - UTC datetime string from backend
 * @returns Date object or null if invalid
 */
export function parseUtcDate(utcDateTime: string | null | undefined): Date | null {
  if (!utcDateTime) return null;
  
  try {
    // Ensure the date string is properly formatted as UTC
    const utcString = utcDateTime.endsWith('Z') ? utcDateTime : `${utcDateTime}Z`;
    const date = new Date(utcString);
    
    return isNaN(date.getTime()) ? null : date;
  } catch (error) {
    console.error('Error parsing UTC date:', error);
    return null;
  }
}

/**
 * Formats time duration in HH:MM:SS format
 * @param hours - Hours as string (e.g., "08:55:00")
 * @returns Formatted duration string
 */
export function formatDuration(hours: string | null | undefined): string {
  if (!hours) return '-';
  
  try {
    // If it's already in HH:MM:SS format, return as is
    if (/^\d{2}:\d{2}:\d{2}$/.test(hours)) {
      return hours;
    }
    
    // If it's a decimal number, convert to HH:MM:SS
    const numHours = parseFloat(hours);
    if (!isNaN(numHours)) {
      const wholeHours = Math.floor(numHours);
      const minutes = Math.floor((numHours - wholeHours) * 60);
      const seconds = Math.floor(((numHours - wholeHours) * 60 - minutes) * 60);
      
      return `${String(wholeHours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
    }
    
    return hours;
  } catch (error) {
    console.error('Error formatting duration:', error);
    return hours || '-';
  }
}
