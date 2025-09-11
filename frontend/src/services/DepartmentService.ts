import { api } from '../lib/api';
import type { Department } from '../lib/api';

export class DepartmentService {
  async getDepartments(): Promise<Department[]> {
    return await api.getDepartments();
  }

  async getDepartment(id: number): Promise<Department> {
    return await api.getDepartment(id);
  }

  async createDepartment(department: Omit<Department, 'id'>): Promise<Department> {
    return await api.createDepartment(department);
  }

  async updateDepartment(id: number, department: Partial<Department>): Promise<Department> {
    return await api.updateDepartment(id, department);
  }

  async deleteDepartment(id: number): Promise<void> {
    return await api.deleteDepartment(id);
  }
}

export const departmentService = new DepartmentService();
