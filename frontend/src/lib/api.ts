import axios, { type AxiosInstance } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5000';

// Add the interfaces here
export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  name: string;
  email: string;
  department: string;
  departmentId: number;
  departmentName: string;
  position: string;
  salary: number;
  isActive: boolean;
  dateOfJoining: string;
}

export interface Attendance {
  id: number;
  employeeId: number;
  employeeName: string;
  checkInTime: string;
  checkOutTime?: string;
  totalHours?: string;
  notes?: string;
  date: string;
  createdAt: string;
}

export interface Department {
  id: number;
  name: string;
  description?: string;
  managerName?: string;
  employeeCount?: number;
}

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: `${API_BASE_URL}/api`,
      withCredentials: true,
    });

    this.client.interceptors.request.use((config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers = config.headers ?? {};
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });
  }

  // Auth
  async login(username: string, password: string) {
    const { data } = await this.client.post('/auth/login', { username, password });
    return data as { token: string; username: string; email: string; role: string; expiresAt: string };
  }

  async register(username: string, email: string, password: string) {
    const { data } = await this.client.post('/auth/register', { username, email, password });
    return data;
  }

  // Employees
  async getEmployees(): Promise<Employee[]> {
    const { data } = await this.client.get('/employees');
    return data;
  }

  async getEmployee(id: number) {
    const { data } = await this.client.get(`/employees/${id}`);
    return data;
  }

  async createEmployee(payload: Omit<Employee, 'id'>) {
    const { data } = await this.client.post('/employees', payload);
    return data;
  }

  async updateEmployee(id: number, payload: Partial<Employee>) {
    const { data } = await this.client.put(`/employees/${id}`, payload);
    return data;
  }

  async deleteEmployee(id: number) {
    await this.client.delete(`/employees/${id}`);
  }

  // Departments
  async getDepartments(): Promise<Department[]> {
    const { data } = await this.client.get('/departments');
    return data;
  }

  async getDepartment(id: number) {
    const { data } = await this.client.get(`/departments/${id}`);
    return data;
  }

  async createDepartment(payload: Omit<Department, 'id'>) {
    const { data } = await this.client.post('/departments', payload);
    return data;
  }

  async updateDepartment(id: number, payload: Partial<Department>) {
    const { data } = await this.client.put(`/departments/${id}`, payload);
    return data;
  }

  async deleteDepartment(id: number) {
    await this.client.delete(`/departments/${id}`);
  }

  // Attendance
  async checkIn(employeeId: number, notes?: string) {
    const { data } = await this.client.post('/attendance/check-in', { employeeId, notes });
    return data;
  }

  async checkOut(employeeId: number, notes?: string) {
    const { data } = await this.client.post('/attendance/check-out', { employeeId, notes });
    return data;
  }

  async getAttendance(employeeId: number, startDate?: string, endDate?: string): Promise<Attendance[]> {
    const params: Record<string, string> = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    const { data } = await this.client.get(`/attendance/employee/${employeeId}`, { params });
    return data;
  }

  // Reports
  async downloadReport(path: string, filename: string) {
    const { data } = await this.client.get(`/reports/${path}`, { responseType: 'blob' });
    const url = window.URL.createObjectURL(new Blob([data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', filename);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  }
}

export const api = new ApiClient();
