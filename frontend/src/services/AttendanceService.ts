import { api } from '../lib/api';
import type { Attendance, CheckIn, CheckOut } from '../lib/api';

export class AttendanceService {
  async checkIn(checkInData: CheckIn): Promise<Attendance> {
    return await api.checkIn(checkInData);
  }

  async checkOut(checkOutData: CheckOut): Promise<Attendance> {
    return await api.checkOut(checkOutData);
  }

  async getEmployeeAttendance(employeeId: number, startDate?: string, endDate?: string): Promise<Attendance[]> {
    return await api.getEmployeeAttendance(employeeId, startDate, endDate);
  }

  async getTodayAttendance(employeeId: number): Promise<Attendance | null> {
    return await api.getTodayAttendance(employeeId);
  }

  async getAllAttendance(startDate?: string, endDate?: string): Promise<Attendance[]> {
    return await api.getAllAttendance(startDate, endDate);
  }
}

export const attendanceService = new AttendanceService();
