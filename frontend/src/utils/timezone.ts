/**
 * Timezone utility functions for converting UTC datetime values to IST (Indian Standard Time)
 */

// IST timezone offset: UTC+5:30
const IST_OFFSET = 5.5 * 60 * 60 * 1000; // 5.5 hours in milliseconds

/**
 * Converts a UTC datetime string to IST
 * @param utcDateTime - UTC datetime string from backend
 * @param options - Intl.DateTimeFormatOptions for formatting
 * @returns Formatted IST datetime string
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
    timeZone: 'Asia/Kolkata'
  }
): string {
  if (!utcDateTime) return '-';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleString('en-IN', options);
  } catch (error) {
    console.error('Error converting UTC to IST:', error);
    return '-';
  }
}

/**
 * Converts a UTC datetime string to IST time for display
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted IST time string (HH:MM:SS)
 */
export function convertUtcToLocalTime(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '-';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleTimeString('en-IN', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false,
      timeZone: 'Asia/Kolkata'
    });
  } catch (error) {
    console.error('Error converting UTC to IST time:', error);
    return '-';
  }
}

/**
 * Converts a UTC datetime string to IST date for display
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted IST date string (YYYY-MM-DD)
 */
export function convertUtcToLocalDate(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '-';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleDateString('en-IN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      timeZone: 'Asia/Kolkata'
    });
  } catch (error) {
    console.error('Error converting UTC to IST date:', error);
    return '-';
  }
}

/**
 * Converts a UTC datetime string to IST datetime for display
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted IST datetime string (YYYY-MM-DD HH:MM:SS)
 */
export function convertUtcToLocalDateTime(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '-';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleString('en-IN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false,
      timeZone: 'Asia/Kolkata'
    });
  } catch (error) {
    console.error('Error converting UTC to IST datetime:', error);
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
 * Gets the IST timezone
 * @returns IST timezone string
 */
export function getUserTimezone(): string {
  return 'Asia/Kolkata';
}

/**
 * Formats a date for form input (YYYY-MM-DD format) in IST
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted date string for form input in IST
 */
export function formatDateForInput(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '';
  
  try {
    const date = new Date(utcDateTime);
    if (isNaN(date.getTime())) return '';
    
    // Convert to IST and format for input
    const istDate = new Date(date.getTime() + IST_OFFSET);
    return istDate.toISOString().split('T')[0];
  } catch (error) {
    console.error('Error formatting date for input:', error);
    return '';
  }
}

/**
 * Converts IST datetime to UTC for sending to backend
 * @param istDateTime - IST datetime string
 * @returns UTC datetime string
 */
export function convertLocalToUtc(istDateTime: string): string {
  try {
    const date = new Date(istDateTime);
    if (isNaN(date.getTime())) return '';
    
    // Convert IST to UTC by subtracting the offset
    const utcDate = new Date(date.getTime() - IST_OFFSET);
    return utcDate.toISOString();
  } catch (error) {
    console.error('Error converting IST to UTC:', error);
    return '';
  }
}
