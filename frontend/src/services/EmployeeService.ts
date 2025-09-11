import { api } from '../lib/api';
import type { Employee } from '../lib/api';

export class EmployeeService {
  async getEmployees(): Promise<Employee[]> {
    return await api.getEmployees();
  }

  async getEmployee(id: number): Promise<Employee> {
    return await api.getEmployee(id);
  }

  async createEmployee(employee: Omit<Employee, 'id'>): Promise<Employee> {
    return await api.createEmployee(employee);
  }

  async updateEmployee(id: number, employee: Partial<Employee>): Promise<Employee> {
    return await api.updateEmployee(id, employee);
  }

  async deleteEmployee(id: number): Promise<void> {
    return await api.deleteEmployee(id);
  }
}

export const employeeService = new EmployeeService();
