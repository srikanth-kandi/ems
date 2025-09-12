# UTC to Local Timezone Conversion Guide

## Overview

This guide explains how UTC timestamps from the backend are converted to the user's local system timezone in the frontend components.

## Problem

The backend sends datetime values in UTC format like:
```json
{
  "checkInTime": "2025-09-12T08:57:00.000Z",
  "checkOutTime": "2025-09-12T17:52:00.000Z",
  "totalHours": "08:55:00",
  "date": "2025-09-12T00:00:00.000Z"
}
```

These need to be displayed in the user's local timezone for better UX.

## Solution

We've implemented comprehensive timezone utilities using **native JavaScript** methods that:

1. **Auto-detect user timezone** using `Intl.DateTimeFormat().resolvedOptions().timeZone`
2. **Properly parse UTC strings** ensuring they end with 'Z' for correct UTC interpretation
3. **Convert to local time** using browser's built-in timezone conversion
4. **Format consistently** across all components

## Implementation

### Core Functions (`src/utils/timezone.ts`)

#### `convertUtcToLocalTime(utcDateTime)`
Converts UTC to local time format (HH:MM:SS)
```typescript
convertUtcToLocalTime("2025-09-12T08:57:00.000Z") 
// Output: "14:27:00" (if user is in UTC+5:30)
```

#### `convertUtcToLocalDate(utcDateTime)`
Converts UTC to local date format (YYYY-MM-DD)
```typescript
convertUtcToLocalDate("2025-09-12T00:00:00.000Z")
// Output: "2025-09-12" (may differ based on timezone)
```

#### `convertUtcToLocalDateTime(utcDateTime)`
Converts UTC to full local datetime (YYYY-MM-DD HH:MM:SS)
```typescript
convertUtcToLocalDateTime("2025-09-12T08:57:00.000Z")
// Output: "2025-09-12 14:27:00" (if user is in UTC+5:30)
```

#### `formatDuration(hours)`
Formats duration strings consistently
```typescript
formatDuration("08:55:00") // Output: "08:55:00"
formatDuration("8.92") // Output: "08:55:12"
```

#### `getUserTimezone()`
Gets user's system timezone
```typescript
getUserTimezone() // Output: "Asia/Kolkata", "America/New_York", etc.
```

### Key Features

1. **Automatic UTC Detection**: Handles both `"2025-09-12T08:57:00.000Z"` and `"2025-09-12T08:57:00.000"` formats
2. **Error Handling**: Returns "-" for invalid dates instead of crashing
3. **Browser Compatibility**: Uses standard `Intl` and `Date` APIs supported by all modern browsers
4. **No Dependencies**: Pure JavaScript implementation, no external libraries needed

### Usage in Components

#### Attendance Component
```typescript
import { convertUtcToLocalTime, convertUtcToLocalDate, formatDuration } from "../../utils/timezone";

// In component render:
<Typography>
  In: {convertUtcToLocalTime(attendance.checkInTime)} | 
  Out: {convertUtcToLocalTime(attendance.checkOutTime)} | 
  Hours: {formatDuration(attendance.totalHours)}
</Typography>
```

#### Employee List Component
```typescript
import { convertUtcToLocalDate, formatDateForInput } from '../../utils/timezone';

// For display:
<Typography>{convertUtcToLocalDate(employee.dateOfJoining)}</Typography>

// For form inputs:
<TextField value={formatDateForInput(employee.dateOfBirth)} />
```

## Testing

Use the test utility to verify timezone conversion:

```typescript
import { testTimezoneConversion } from './timezoneTest';

// In browser console or component:
testTimezoneConversion();
```

This will output:
- User's timezone
- Original UTC values
- Converted local values
- Test with various UTC formats

## Alternative: Moment.js Implementation

If you prefer using Moment.js, we've provided an alternative implementation in `timezone-moment.ts`.

### Installation
```bash
npm install moment
npm install @types/moment --save-dev
```

### Usage
```typescript
// Replace imports:
// import { convertUtcToLocalTime } from './timezone';
import { convertUtcToLocalTime } from './timezone-moment';
```

## Browser Support

- **Native Implementation**: All modern browsers (Chrome 24+, Firefox 29+, Safari 10+, Edge 12+)
- **Moment.js**: Universal browser support including older browsers

## Performance

- **Native Implementation**: Faster, smaller bundle size, no additional dependencies
- **Moment.js**: Slightly slower, larger bundle size, but more features for complex date manipulation

## Examples with Your Data

Given your backend data:
```json
{
  "checkInTime": "2025-09-12T08:57:00.000Z",
  "checkOutTime": "2025-09-12T17:52:00.000Z",
  "totalHours": "08:55:00"
}
```

The functions will output (assuming user is in IST UTC+5:30):
- **Check In**: `14:27:00` (8:57 AM UTC + 5:30 hours)
- **Check Out**: `23:22:00` (5:52 PM UTC + 5:30 hours) 
- **Total Hours**: `08:55:00` (formatted duration)

For users in different timezones:
- **PST (UTC-8)**: Check In would be `00:57:00`
- **CET (UTC+1)**: Check In would be `09:57:00`
- **JST (UTC+9)**: Check In would be `17:57:00`

## Migration Notes

- Old functions were hardcoded to IST (`Asia/Kolkata`)
- New functions automatically detect user's system timezone
- All existing component imports continue to work
- No breaking changes to function signatures
- Improved error handling and UTC parsing
