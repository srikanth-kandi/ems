import { api } from '../lib/api';

export class ReportService {
  async downloadEmployeeReport(format: 'pdf' | 'excel' | 'csv'): Promise<void> {
    const filename = `employees.${format === 'csv' ? 'csv' : format === 'pdf' ? 'pdf' : 'xlsx'}`;
    const endpoint = format === 'csv' ? 'employees' : `employees/${format}`;
    return await api.downloadReport(endpoint, filename);
  }

  async downloadDepartmentReport(format: 'pdf' | 'excel' | 'csv'): Promise<void> {
    const filename = `departments.${format === 'csv' ? 'csv' : format === 'pdf' ? 'pdf' : 'xlsx'}`;
    const endpoint = format === 'csv' ? 'departments' : `departments/${format}`;
    return await api.downloadReport(endpoint, filename);
  }

  async downloadAttendanceReport(format: 'pdf' | 'excel' | 'csv', startDate?: string, endDate?: string): Promise<void> {
    const filename = `attendance.${format === 'csv' ? 'csv' : format === 'pdf' ? 'pdf' : 'xlsx'}`;
    const endpoint = format === 'csv' ? 'attendance' : `attendance/${format}`;
    return await api.downloadReport(endpoint, filename);
  }
}

export const reportService = new ReportService();
