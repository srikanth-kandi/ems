import { api } from '../lib/api';
import type { Department, CreateDepartment, UpdateDepartment } from '../lib/api';

export class DepartmentService {
  async getDepartments(): Promise<Department[]> {
    return await api.getDepartments();
  }

  async getDepartment(id: number): Promise<Department> {
    return await api.getDepartment(id);
  }

  async createDepartment(department: CreateDepartment): Promise<Department> {
    return await api.createDepartment(department);
  }

  async updateDepartment(id: number, department: UpdateDepartment): Promise<Department> {
    return await api.updateDepartment(id, department);
  }

  async deleteDepartment(id: number): Promise<void> {
    return await api.deleteDepartment(id);
  }

  async getDepartmentsWithEmployeeCount(): Promise<Department[]> {
    return await api.getDepartmentsWithEmployeeCount();
  }
}

export const departmentService = new DepartmentService();
