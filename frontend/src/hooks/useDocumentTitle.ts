import { useEffect } from 'react';

interface RouteConfig {
  path: string;
  title: string;
  favicon?: string;
  emoji?: string;
}

const routeConfigs: RouteConfig[] = [
  { path: '/', title: 'Dashboard', emoji: '📊', favicon: '📊' },
  { path: '/employees', title: 'Employees', emoji: '👥', favicon: '👥' },
  { path: '/departments', title: 'Departments', emoji: '🏢', favicon: '🏢' },
  { path: '/attendance', title: 'Attendance', emoji: '⏰', favicon: '⏰' },
  { path: '/reports', title: 'Reports', emoji: '📈', favicon: '📈' },
  { path: '/login', title: 'Login', emoji: '🔐', favicon: '🔐' },
];

export const useDocumentTitle = (pathname: string) => {
  useEffect(() => {
    const config = routeConfigs.find(route => route.path === pathname) || 
                  { path: pathname, title: 'Employee Management System', emoji: '👥', favicon: '👥' };
    
    // Update document title
    document.title = `${config.title} - EMS`;
    
    // Update favicon using emoji
    const updateFavicon = (emoji: string) => {
      const canvas = document.createElement('canvas');
      canvas.width = 32;
      canvas.height = 32;
      const ctx = canvas.getContext('2d');
      
      if (ctx) {
        // Set background
        ctx.fillStyle = '#1976d2';
        ctx.fillRect(0, 0, 32, 32);
        
        // Draw emoji
        ctx.font = '20px Arial';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(emoji, 16, 16);
        
        // Convert to data URL and set as favicon
        const dataURL = canvas.toDataURL('image/png');
        const link = document.querySelector("link[rel*='icon']") as HTMLLinkElement;
        if (link) {
          link.href = dataURL;
        } else {
          const newLink = document.createElement('link');
          newLink.rel = 'icon';
          newLink.href = dataURL;
          document.head.appendChild(newLink);
        }
      }
    };
    
    updateFavicon(config.favicon || config.emoji || '👥');
  }, [pathname]);
};
