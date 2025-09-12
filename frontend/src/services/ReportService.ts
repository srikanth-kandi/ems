import { api } from '../lib/api';

export class ReportService {
  // Employee Reports
  async downloadEmployeeReport(format: 'csv' | 'pdf' | 'excel'): Promise<void> {
    switch (format) {
      case 'csv':
        return await api.downloadEmployeeReportCsv();
      case 'pdf':
        return await api.downloadEmployeeReportPdf();
      case 'excel':
        return await api.downloadEmployeeReportExcel();
      default:
        throw new Error(`Unsupported format: ${format}`);
    }
  }

  async downloadEmployeeReportCsv(): Promise<void> {
    return await api.downloadEmployeeReportCsv();
  }

  async downloadEmployeeReportPdf(): Promise<void> {
    return await api.downloadEmployeeReportPdf();
  }

  async downloadEmployeeReportExcel(): Promise<void> {
    return await api.downloadEmployeeReportExcel();
  }

  // Department Reports
  async downloadDepartmentReportCsv(): Promise<void> {
    return await api.downloadDepartmentReportCsv();
  }

  async downloadDepartmentReportPdf(): Promise<void> {
    return await api.downloadDepartmentReportPdf();
  }

  async downloadDepartmentReportExcel(): Promise<void> {
    return await api.downloadDepartmentReportExcel();
  }

  // Attendance Reports
  async downloadAttendanceReportCsv(startDate?: string, endDate?: string): Promise<void> {
    return await api.downloadAttendanceReportCsv(startDate, endDate);
  }

  async downloadAttendanceReportPdf(startDate?: string, endDate?: string): Promise<void> {
    return await api.downloadAttendanceReportPdf(startDate, endDate);
  }

  async downloadAttendanceReportExcel(startDate?: string, endDate?: string): Promise<void> {
    return await api.downloadAttendanceReportExcel(startDate, endDate);
  }

  // Salary Reports
  async downloadSalaryReportCsv(): Promise<void> {
    return await api.downloadSalaryReportCsv();
  }

  async downloadSalaryReportPdf(): Promise<void> {
    return await api.downloadSalaryReportPdf();
  }

  async downloadSalaryReportExcel(): Promise<void> {
    return await api.downloadSalaryReportExcel();
  }

  // Additional Reports
  async downloadHiringTrendsReportCsv(): Promise<void> {
    return await api.downloadHiringTrendsReportCsv();
  }

  async downloadDepartmentGrowthReportCsv(): Promise<void> {
    return await api.downloadDepartmentGrowthReportCsv();
  }

  async downloadAttendancePatternsReportCsv(): Promise<void> {
    return await api.downloadAttendancePatternsReportCsv();
  }

  async downloadPerformanceMetricsReportCsv(employeeId?: number): Promise<void> {
    return await api.downloadPerformanceMetricsReportCsv(employeeId);
  }

  // Additional PDF Reports
  async downloadHiringTrendsReportPdf(): Promise<void> {
    return await api.downloadHiringTrendsReportPdf();
  }

  async downloadDepartmentGrowthReportPdf(): Promise<void> {
    return await api.downloadDepartmentGrowthReportPdf();
  }

  async downloadAttendancePatternsReportPdf(): Promise<void> {
    return await api.downloadAttendancePatternsReportPdf();
  }

  async downloadPerformanceMetricsReportPdf(employeeId?: number): Promise<void> {
    return await api.downloadPerformanceMetricsReportPdf(employeeId);
  }

  // Additional Excel Reports
  async downloadHiringTrendsReportExcel(): Promise<void> {
    return await api.downloadHiringTrendsReportExcel();
  }

  async downloadDepartmentGrowthReportExcel(): Promise<void> {
    return await api.downloadDepartmentGrowthReportExcel();
  }

  async downloadAttendancePatternsReportExcel(): Promise<void> {
    return await api.downloadAttendancePatternsReportExcel();
  }

  async downloadPerformanceMetricsReportExcel(employeeId?: number): Promise<void> {
    return await api.downloadPerformanceMetricsReportExcel(employeeId);
  }
}

export const reportService = new ReportService();
