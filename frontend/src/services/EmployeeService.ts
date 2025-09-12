import { api } from '../lib/api';
import type { Employee, CreateEmployee, UpdateEmployee, PaginationRequest, PagedResult } from '../lib/api';

export class EmployeeService {
  async getEmployees(): Promise<Employee[]> {
    return await api.getEmployees();
  }

  async getEmployee(id: number): Promise<Employee> {
    return await api.getEmployee(id);
  }

  async createEmployee(employee: CreateEmployee): Promise<Employee> {
    return await api.createEmployee(employee);
  }

  async updateEmployee(id: number, employee: UpdateEmployee): Promise<Employee> {
    return await api.updateEmployee(id, employee);
  }

  async deleteEmployee(id: number): Promise<void> {
    return await api.deleteEmployee(id);
  }

  async bulkCreateEmployees(employees: CreateEmployee[]): Promise<Employee[]> {
    return await api.bulkCreateEmployees(employees);
  }

  async bulkDeleteEmployees(employeeIds: number[]): Promise<void> {
    return await api.bulkDeleteEmployees(employeeIds);
  }

  async getEmployeesPaged(request: PaginationRequest): Promise<PagedResult<Employee>> {
    return await api.getEmployeesPaged(request);
  }

  async getEmployeesByDepartmentPaged(departmentId: number, request: PaginationRequest): Promise<PagedResult<Employee>> {
    return await api.getEmployeesByDepartmentPaged(departmentId, request);
  }
}

export const employeeService = new EmployeeService();
