import { api } from '../lib/api';

export class ReportService {
  // Employee Reports
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
}

export const reportService = new ReportService();
