# EMS Frontend

React.js frontend for the Employee Management System built with TypeScript, Vite, and Material-UI.

## Technology Stack

- **React 18** with **TypeScript**
- **Vite** for build tooling and dev server
- **Material-UI (MUI)** for UI components
- **React Router** for navigation
- **Axios** for API communication
- **React Hook Form** with **Yup** for form validation
- **Zustand** for state management

## Getting Started

### Prerequisites

- Node.js 20.10.0+ (compatible with Vite 5)
- npm 10.8.2+

### Installation

```bash
# Install dependencies
npm install

# Start development server
npm run dev
```

The application will be available at `http://localhost:3000`

### Environment Configuration

Copy the example environment file and configure your settings:

```bash
# Copy the example file
cp .env.example .env
```

Edit `.env` to match your backend configuration:

```env
# API Configuration
VITE_API_URL=http://localhost:5001

# Development Settings
VITE_APP_TITLE=EMS Frontend
VITE_APP_VERSION=1.0.0

# Optional: Enable debug logging
VITE_DEBUG=false
```

### Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## Project Structure

```
frontend/
├── src/
│   ├── components/          # React components
│   │   ├── auth/           # Authentication components
│   │   ├── employees/      # Employee management
│   │   ├── attendance/     # Attendance tracking
│   │   └── reports/        # Reports and analytics
│   ├── lib/                # Utilities and API client
│   ├── store/              # State management (Zustand)
│   └── assets/             # Static assets
├── public/                 # Public static files
├── .env.example           # Environment variables template
├── .env                   # Local environment variables
└── package.json
```

## Features

- 🔐 User authentication and authorization
- 👥 Employee management (CRUD operations)
- ⏰ Attendance tracking (check-in/check-out)
- 📊 Reports and analytics
- 📱 Responsive design with Material-UI
- 🎯 Form validation with React Hook Form + Yup
- 🔄 State management with Zustand

## API Integration

The frontend communicates with the backend API through the `api.ts` client located in `src/lib/`. The API base URL is configured via the `VITE_API_URL` environment variable.

## Development Notes

- Uses Vite 5 for compatibility with Node.js 20.10.0
- TypeScript strict mode enabled
- ESLint configured for code quality
- Material-UI theming support
- Hot module replacement for fast development

## Troubleshooting

### Node.js Version Issues

If you encounter Node.js version errors, ensure you're using Node.js 20.10.0+:

```bash
node -v  # Should show v20.10.0 or higher
```

### Environment Variables

Make sure your `.env` file is properly configured and the API URL matches your backend server address.
