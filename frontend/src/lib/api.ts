import axios, { type AxiosInstance } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

// Add the interfaces here
export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  dateOfBirth: string;
  dateOfJoining: string;
  position?: string;
  salary: number;
  departmentId: number;
  departmentName: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateEmployee {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  dateOfBirth: string;
  dateOfJoining: string;
  position?: string;
  salary: number;
  departmentId: number;
}

export interface UpdateEmployee {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  dateOfBirth: string;
  dateOfJoining: string;
  position?: string;
  salary: number;
  departmentId: number;
  isActive: boolean;
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

export interface CheckIn {
  employeeId: number;
  notes?: string;
}

export interface CheckOut {
  employeeId: number;
  notes?: string;
}

export interface Department {
  id: number;
  name: string;
  description?: string;
  managerName?: string;
  employeeCount: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateDepartment {
  name: string;
  description?: string;
  managerName?: string;
}

export interface UpdateDepartment {
  name: string;
  description?: string;
  managerName?: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  role?: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  email: string;
  role: string;
  expiresAt: string;
}

export interface PaginationRequest {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface PagedResult<T> {
  data: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface SeedStatus {
  departments: number;
  employees: number;
  attendances: number;
  performanceMetrics: number;
  users: number;
  timestamp: string;
}

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: `${API_BASE_URL}/api`,
      withCredentials: true,
    });

    // Request interceptor to add auth token
    this.client.interceptors.request.use((config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers = config.headers ?? {};
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });

    // Response interceptor to handle token expiration
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Token is invalid or expired
          this.handleTokenExpiry();
        }
        return Promise.reject(error);
      }
    );
  }

  private handleTokenExpiry() {
    // Clear auth data
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('role');
    localStorage.removeItem('tokenExpiresAt');
    
    // Trigger a custom event that the auth store can listen to
    window.dispatchEvent(new CustomEvent('tokenExpired'));
    
    // Redirect to login if not already there
    if (window.location.pathname !== '/login') {
      window.location.href = '/login';
    }
  }

  // Auth
  async login(loginData: LoginRequest): Promise<AuthResponse> {
    const { data } = await this.client.post('/auth/login', loginData);
    return data;
  }

  async register(registerData: RegisterRequest): Promise<AuthResponse> {
    const { data } = await this.client.post('/auth/register', registerData);
    return data;
  }

  // Employees
  async getEmployees(): Promise<Employee[]> {
    const { data } = await this.client.get('/employees');
    return data;
  }

  async getEmployee(id: number): Promise<Employee> {
    const { data } = await this.client.get(`/employees/${id}`);
    return data;
  }

  async createEmployee(payload: CreateEmployee): Promise<Employee> {
    const { data } = await this.client.post('/employees', payload);
    return data;
  }

  async updateEmployee(id: number, payload: UpdateEmployee): Promise<Employee> {
    const { data } = await this.client.put(`/employees/${id}`, payload);
    return data;
  }

  async deleteEmployee(id: number): Promise<void> {
    await this.client.delete(`/employees/${id}`);
  }

  async bulkCreateEmployees(employees: CreateEmployee[]): Promise<Employee[]> {
    const { data } = await this.client.post('/employees/bulk', employees);
    return data;
  }

  async bulkDeleteEmployees(employeeIds: number[]): Promise<void> {
    await this.client.delete('/employees/bulk', { data: employeeIds });
  }

  async getEmployeesPaged(request: PaginationRequest): Promise<PagedResult<Employee>> {
    const { data } = await this.client.get('/employees/paged', { params: request });
    return data;
  }

  async getEmployeesByDepartmentPaged(departmentId: number, request: PaginationRequest): Promise<PagedResult<Employee>> {
    const { data } = await this.client.get(`/employees/department/${departmentId}/paged`, { params: request });
    return data;
  }

  // Departments
  async getDepartments(): Promise<Department[]> {
    const { data } = await this.client.get('/departments');
    return data;
  }

  async getDepartment(id: number): Promise<Department> {
    const { data } = await this.client.get(`/departments/${id}`);
    return data;
  }

  async createDepartment(payload: CreateDepartment): Promise<Department> {
    const { data } = await this.client.post('/departments', payload);
    return data;
  }

  async updateDepartment(id: number, payload: UpdateDepartment): Promise<Department> {
    const { data } = await this.client.put(`/departments/${id}`, payload);
    return data;
  }

  async deleteDepartment(id: number): Promise<void> {
    await this.client.delete(`/departments/${id}`);
  }

  async getDepartmentsWithEmployeeCount(): Promise<Department[]> {
    const { data } = await this.client.get('/departments/with-employee-count');
    return data;
  }

  // Attendance
  async checkIn(checkInData: CheckIn): Promise<Attendance> {
    const { data } = await this.client.post('/attendance/check-in', checkInData);
    return data;
  }

  async checkOut(checkOutData: CheckOut): Promise<Attendance> {
    const { data } = await this.client.post('/attendance/check-out', checkOutData);
    return data;
  }

  async getEmployeeAttendance(employeeId: number, startDate?: string, endDate?: string): Promise<Attendance[]> {
    const params: Record<string, string> = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    const { data } = await this.client.get(`/attendance/employee/${employeeId}`, { params });
    return data;
  }

  async getTodayAttendance(employeeId: number): Promise<Attendance | null> {
    const { data } = await this.client.get(`/attendance/employee/${employeeId}/today`);
    return data;
  }

  async getAllAttendance(startDate?: string, endDate?: string): Promise<Attendance[]> {
    const params: Record<string, string> = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    const { data } = await this.client.get('/attendance', { params });
    return data;
  }

  // Reports - CSV
  async downloadEmployeeReportCsv(): Promise<void> {
    const { data } = await this.client.get('/reports/employees', { responseType: 'blob' });
    this.downloadBlob(data, 'employees.csv');
  }

  async downloadDepartmentReportCsv(): Promise<void> {
    const { data } = await this.client.get('/reports/departments', { responseType: 'blob' });
    this.downloadBlob(data, 'departments.csv');
  }

  async downloadAttendanceReportCsv(startDate?: string, endDate?: string): Promise<void> {
    const params: Record<string, string> = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    const { data } = await this.client.get('/reports/attendance', { responseType: 'blob', params });
    this.downloadBlob(data, 'attendance.csv');
  }

  async downloadSalaryReportCsv(): Promise<void> {
    const { data } = await this.client.get('/reports/salaries', { responseType: 'blob' });
    this.downloadBlob(data, 'salaries.csv');
  }

  async downloadHiringTrendsReportCsv(): Promise<void> {
    const { data } = await this.client.get('/reports/hiring-trends', { responseType: 'blob' });
    this.downloadBlob(data, 'hiring-trends.csv');
  }

  async downloadDepartmentGrowthReportCsv(): Promise<void> {
    const { data } = await this.client.get('/reports/department-growth', { responseType: 'blob' });
    this.downloadBlob(data, 'department-growth.csv');
  }

  async downloadAttendancePatternsReportCsv(): Promise<void> {
    const { data } = await this.client.get('/reports/attendance-patterns', { responseType: 'blob' });
    this.downloadBlob(data, 'attendance-patterns.csv');
  }

  async downloadPerformanceMetricsReportCsv(employeeId?: number): Promise<void> {
    const params: Record<string, string> = {};
    if (employeeId) params.employeeId = employeeId.toString();
    const { data } = await this.client.get('/reports/performance-metrics', { responseType: 'blob', params });
    this.downloadBlob(data, 'performance-metrics.csv');
  }

  // Reports - PDF
  async downloadEmployeeReportPdf(): Promise<void> {
    const { data } = await this.client.get('/reports/employees/pdf', { responseType: 'blob' });
    this.downloadBlob(data, 'employees.pdf');
  }

  async downloadDepartmentReportPdf(): Promise<void> {
    const { data } = await this.client.get('/reports/departments/pdf', { responseType: 'blob' });
    this.downloadBlob(data, 'departments.pdf');
  }

  async downloadAttendanceReportPdf(startDate?: string, endDate?: string): Promise<void> {
    const params: Record<string, string> = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    const { data } = await this.client.get('/reports/attendance/pdf', { responseType: 'blob', params });
    this.downloadBlob(data, 'attendance.pdf');
  }

  async downloadSalaryReportPdf(): Promise<void> {
    const { data } = await this.client.get('/reports/salaries/pdf', { responseType: 'blob' });
    this.downloadBlob(data, 'salaries.pdf');
  }

  async downloadHiringTrendsReportPdf(): Promise<void> {
    const { data } = await this.client.get('/reports/hiring-trends/pdf', { responseType: 'blob' });
    this.downloadBlob(data, 'hiring-trends.pdf');
  }

  async downloadDepartmentGrowthReportPdf(): Promise<void> {
    const { data } = await this.client.get('/reports/department-growth/pdf', { responseType: 'blob' });
    this.downloadBlob(data, 'department-growth.pdf');
  }

  async downloadAttendancePatternsReportPdf(): Promise<void> {
    const { data } = await this.client.get('/reports/attendance-patterns/pdf', { responseType: 'blob' });
    this.downloadBlob(data, 'attendance-patterns.pdf');
  }

  async downloadPerformanceMetricsReportPdf(employeeId?: number): Promise<void> {
    const params: Record<string, string> = {};
    if (employeeId) params.employeeId = employeeId.toString();
    const { data } = await this.client.get('/reports/performance-metrics/pdf', { responseType: 'blob', params });
    this.downloadBlob(data, 'performance-metrics.pdf');
  }

  // Reports - Excel
  async downloadEmployeeReportExcel(): Promise<void> {
    const { data } = await this.client.get('/reports/employees/excel', { responseType: 'blob' });
    this.downloadBlob(data, 'employees.xlsx');
  }

  async downloadDepartmentReportExcel(): Promise<void> {
    const { data } = await this.client.get('/reports/departments/excel', { responseType: 'blob' });
    this.downloadBlob(data, 'departments.xlsx');
  }

  async downloadAttendanceReportExcel(startDate?: string, endDate?: string): Promise<void> {
    const params: Record<string, string> = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    const { data } = await this.client.get('/reports/attendance/excel', { responseType: 'blob', params });
    this.downloadBlob(data, 'attendance.xlsx');
  }

  async downloadSalaryReportExcel(): Promise<void> {
    const { data } = await this.client.get('/reports/salaries/excel', { responseType: 'blob' });
    this.downloadBlob(data, 'salaries.xlsx');
  }

  async downloadHiringTrendsReportExcel(): Promise<void> {
    const { data } = await this.client.get('/reports/hiring-trends/excel', { responseType: 'blob' });
    this.downloadBlob(data, 'hiring-trends.xlsx');
  }

  async downloadDepartmentGrowthReportExcel(): Promise<void> {
    const { data } = await this.client.get('/reports/department-growth/excel', { responseType: 'blob' });
    this.downloadBlob(data, 'department-growth.xlsx');
  }

  async downloadAttendancePatternsReportExcel(): Promise<void> {
    const { data } = await this.client.get('/reports/attendance-patterns/excel', { responseType: 'blob' });
    this.downloadBlob(data, 'attendance-patterns.xlsx');
  }

  async downloadPerformanceMetricsReportExcel(employeeId?: number): Promise<void> {
    const params: Record<string, string> = {};
    if (employeeId) params.employeeId = employeeId.toString();
    const { data } = await this.client.get('/reports/performance-metrics/excel', { responseType: 'blob', params });
    this.downloadBlob(data, 'performance-metrics.xlsx');
  }

  // Seed Management
  async seedData(): Promise<{ message: string; timestamp: string }> {
    const { data } = await this.client.post('/seed/seed');
    return data;
  }

  async reseedData(): Promise<{ message: string; timestamp: string }> {
    const { data } = await this.client.post('/seed/reseed');
    return data;
  }

  async clearData(): Promise<{ message: string; timestamp: string }> {
    const { data } = await this.client.delete('/seed/clear');
    return data;
  }

  async getSeedStatus(): Promise<SeedStatus> {
    const { data } = await this.client.get('/seed/status');
    return data;
  }

  // Generic download method for reports
  async downloadReport(endpoint: string, filename: string): Promise<void> {
    const { data } = await this.client.get(`/reports/${endpoint}`, { responseType: 'blob' });
    this.downloadBlob(data, filename);
  }

  // Helper method for downloading blobs
  private downloadBlob(data: Blob, filename: string): void {
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
