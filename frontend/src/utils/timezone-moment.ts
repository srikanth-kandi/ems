/**
 * Alternative timezone utility functions using moment.js
 * To use this implementation, install moment.js:
 * npm install moment
 * npm install @types/moment --save-dev
 * 
 * Then replace imports from './timezone' with './timezone-moment'
 */

// Uncomment these imports after installing moment.js
// import moment from 'moment';

/**
 * Converts a UTC datetime string to local time using moment.js
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted local time string (HH:mm:ss)
 */
export function convertUtcToLocalTime(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '-';
  
  try {
    // Using moment.js (uncomment after installing)
    // return moment.utc(utcDateTime).local().format('HH:mm:ss');
    
    // Fallback to native JavaScript implementation
    const utcString = utcDateTime.endsWith('Z') ? utcDateTime : `${utcDateTime}Z`;
    const date = new Date(utcString);
    
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleTimeString(navigator.language || 'en-US', {
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
 * Converts a UTC datetime string to local date using moment.js
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted local date string (YYYY-MM-DD)
 */
export function convertUtcToLocalDate(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '-';
  
  try {
    // Using moment.js (uncomment after installing)
    // return moment.utc(utcDateTime).local().format('YYYY-MM-DD');
    
    // Fallback to native JavaScript implementation
    const utcString = utcDateTime.endsWith('Z') ? utcDateTime : `${utcDateTime}Z`;
    const date = new Date(utcString);
    
    if (isNaN(date.getTime())) return '-';
    
    return date.toLocaleDateString('en-CA'); // en-CA gives YYYY-MM-DD format
  } catch (error) {
    console.error('Error converting UTC to local date:', error);
    return '-';
  }
}

/**
 * Converts a UTC datetime string to local datetime using moment.js
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted local datetime string (YYYY-MM-DD HH:mm:ss)
 */
export function convertUtcToLocalDateTime(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '-';
  
  try {
    // Using moment.js (uncomment after installing)
    // return moment.utc(utcDateTime).local().format('YYYY-MM-DD HH:mm:ss');
    
    // Fallback to native JavaScript implementation
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
      hour12: false
    });
  } catch (error) {
    console.error('Error converting UTC to local datetime:', error);
    return '-';
  }
}

/**
 * Formats time duration using moment.js
 * @param hours - Hours as string (e.g., "08:55:00")
 * @returns Formatted duration string
 */
export function formatDuration(hours: string | null | undefined): string {
  if (!hours) return '-';
  
  try {
    // Using moment.js for duration formatting (uncomment after installing)
    // const duration = moment.duration(hours);
    // return moment.utc(duration.asMilliseconds()).format('HH:mm:ss');
    
    // Fallback to native JavaScript implementation
    if (/^\d{2}:\d{2}:\d{2}$/.test(hours)) {
      return hours;
    }
    
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

/**
 * Gets the user's local timezone
 * @returns User's local timezone string
 */
export function getUserTimezone(): string {
  // Using moment.js (uncomment after installing)
  // return moment.tz.guess();
  
  // Fallback to native JavaScript implementation
  return Intl.DateTimeFormat().resolvedOptions().timeZone;
}

/**
 * Converts local datetime to UTC using moment.js
 * @param localDateTime - Local datetime string
 * @returns UTC datetime string
 */
export function convertLocalToUtc(localDateTime: string): string {
  try {
    // Using moment.js (uncomment after installing)
    // return moment(localDateTime).utc().toISOString();
    
    // Fallback to native JavaScript implementation
    const date = new Date(localDateTime);
    if (isNaN(date.getTime())) return '';
    
    return date.toISOString();
  } catch (error) {
    console.error('Error converting local to UTC:', error);
    return '';
  }
}

/**
 * Formats a date for form input using moment.js
 * @param utcDateTime - UTC datetime string from backend
 * @returns Formatted date string for form input (YYYY-MM-DD)
 */
export function formatDateForInput(utcDateTime: string | null | undefined): string {
  if (!utcDateTime) return '';
  
  try {
    // Using moment.js (uncomment after installing)
    // return moment.utc(utcDateTime).local().format('YYYY-MM-DD');
    
    // Fallback to native JavaScript implementation
    const utcString = utcDateTime.endsWith('Z') ? utcDateTime : `${utcDateTime}Z`;
    const date = new Date(utcString);
    
    if (isNaN(date.getTime())) return '';
    
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    
    return `${year}-${month}-${day}`;
  } catch (error) {
    console.error('Error formatting date for input:', error);
    return '';
  }
}
