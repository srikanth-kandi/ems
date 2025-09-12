import { create } from 'zustand';

type AuthState = {
  token: string | null;
  username: string | null;
  role: string | null;
  expiresAt: string | null;
  setAuth: (payload: { 
    token: string; 
    username: string; 
    role: string; 
    expiresAt: string; 
  }) => void;
  logout: () => void;
  isTokenExpired: () => boolean;
  getTimeToExpiry: () => number;
};

export const useAuthStore = create<AuthState>((set, get) => {
  // Listen for token expiry events from the API client
  if (typeof window !== 'undefined') {
    window.addEventListener('tokenExpired', () => {
      const store = get();
      if (store.token) {
        store.logout();
      }
    });
  }

  return {
    token: localStorage.getItem('token'),
    username: localStorage.getItem('username'),
    role: localStorage.getItem('role'),
    expiresAt: localStorage.getItem('tokenExpiresAt'),
    
    setAuth: ({ token, username, role, expiresAt }) => {
      localStorage.setItem('token', token);
      localStorage.setItem('username', username);
      localStorage.setItem('role', role);
      localStorage.setItem('tokenExpiresAt', expiresAt);
      set({ token, username, role, expiresAt });
    },
    
    logout: () => {
      localStorage.removeItem('token');
      localStorage.removeItem('username');
      localStorage.removeItem('role');
      localStorage.removeItem('tokenExpiresAt');
      set({ token: null, username: null, role: null, expiresAt: null });
    },
    
    isTokenExpired: () => {
      const { expiresAt } = get();
      if (!expiresAt) return true;
      return new Date() >= new Date(expiresAt);
    },
    
    getTimeToExpiry: () => {
      const { expiresAt } = get();
      if (!expiresAt) return 0;
      return Math.max(0, new Date(expiresAt).getTime() - new Date().getTime());
    },
  };
});


