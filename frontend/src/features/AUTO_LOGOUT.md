# Auto Logout Feature

## Overview
The auto logout feature automatically logs users out when their JWT token expires or becomes invalid, providing a secure and seamless user experience.

## Components

### 1. Enhanced Auth Store (`store/auth.ts`)
- **Token Expiry Tracking**: Stores and tracks token expiration time
- **Expiry Validation**: Methods to check if token is expired and calculate time remaining
- **Event Handling**: Listens for token expiry events from API client

### 2. Auto Logout Hook (`hooks/useAutoLogout.ts`)
- **Token Monitoring**: Periodically checks token expiry status
- **Session Warnings**: Shows warnings when session is about to expire
- **Automatic Logout**: Performs logout when token expires
- **Configurable Options**:
  - `warningTimeMs`: Time before expiry to show warning (default: 5 minutes)
  - `checkIntervalMs`: How often to check token expiry (default: 30 seconds)
  - `enabled`: Whether auto logout is enabled (default: true)

### 3. API Client Integration (`lib/api.ts`)
- **Response Interceptor**: Catches 401 responses and triggers logout
- **Token Expiry Event**: Dispatches custom event for auth store
- **Automatic Redirect**: Redirects to login page on token expiry

### 4. Session Status Component (`components/common/SessionStatus.tsx`)
- **Visual Indicator**: Displays remaining session time in the header
- **Warning State**: Changes appearance when session is about to expire
- **Tooltip**: Shows detailed expiry information on hover

## Features

### Automatic Token Expiry Detection
- Monitors JWT token expiration time continuously
- Checks both stored expiry time and server responses (401 errors)
- Handles token invalidation gracefully

### Session Timeout Warnings
- Shows warning notifications 5 minutes before expiry
- Visual indicator in the header showing remaining time
- Color-coded display (warning state when < 5 minutes remaining)

### Multiple Logout Triggers
1. **Time-based**: Automatic logout when token expires
2. **Server-based**: Logout on 401 responses from API
3. **Manual**: User-initiated logout

### User Experience Enhancements
- Non-intrusive session status display
- Clear warning messages
- Smooth transitions to login page
- Preserved navigation state

## Configuration

### Backend Token Configuration
In `appsettings.json`:
```json
{
  "Jwt": {
    "ExpiryMinutes": 60
  }
}
```

### Frontend Auto Logout Configuration
In `App.tsx`:
```typescript
useAutoLogout({
  warningTimeMs: 5 * 60 * 1000, // 5 minutes warning
  checkIntervalMs: 30 * 1000,   // Check every 30 seconds
  enabled: !!token,             // Enable when authenticated
});
```

## Security Benefits
- Prevents unauthorized access with expired tokens
- Reduces security risks from unattended sessions
- Ensures consistent authentication state across the application
- Protects against token replay attacks

## Implementation Details

### Token Storage
- Token and expiry time stored in localStorage
- Synchronized with Zustand auth store
- Cleared on logout or expiry

### Event Flow
1. User logs in → Token and expiry stored
2. Auto logout hook starts monitoring
3. Periodic checks for token expiry
4. Warning shown when approaching expiry
5. Automatic logout when expired
6. Redirect to login page

### Error Handling
- Graceful handling of network errors
- Fallback to time-based expiry if server unavailable
- Prevention of multiple logout attempts
- Clean state management on logout

## Browser Compatibility
- Uses localStorage for token persistence
- Custom events for cross-component communication
- Standard Web APIs for timers and navigation
- Compatible with all modern browsers
