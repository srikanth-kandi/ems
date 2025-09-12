import { useState, useEffect, useCallback } from 'react';
import { employeeService } from '../services/EmployeeService';
import { departmentService } from '../services/DepartmentService';
import { reportService } from '../services/ReportService';
import type { Employee } from '../lib/api';

interface Department {
  id: number;
  name: string;
}

interface EmployeeForm {
  firstName: string;
  lastName: string;
  email: string;
  departmentId: number;
  salary: number;
  position: string;
  dateOfBirth: string;
  dateOfJoining: string;
  isActive: boolean;
}

interface NotificationState {
  open: boolean;
  message: string;
  severity: 'success' | 'error';
}

export function useEmployeeManagement() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<EmployeeForm>({
    firstName: '',
    lastName: '',
    email: '',
    departmentId: 1,
    salary: 0,
    position: '',
    dateOfBirth: '',
    dateOfJoining: new Date().toISOString().split('T')[0],
    isActive: true
  });
  const [editingId, setEditingId] = useState<number | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedDepartment, setSelectedDepartment] = useState<number | ''>('');
  const [notification, setNotification] = useState<NotificationState>({
    open: false,
    message: '',
    severity: 'success'
  });

  const showNotification = useCallback((message: string, severity: 'success' | 'error' = 'success') => {
    setNotification({ open: true, message, severity });
  }, []);

  const closeNotification = useCallback(() => {
    setNotification(prev => ({ ...prev, open: false }));
  }, []);

  const loadEmployees = useCallback(async () => {
    setLoading(true);
    try {
      const data = await employeeService.getEmployees();
      setEmployees(data);
    } catch (error) {
      showNotification('Error loading employees', 'error');
    } finally {
      setLoading(false);
    }
  }, [showNotification]);

  const loadDepartments = useCallback(async () => {
    try {
      const data = await departmentService.getDepartments();
      setDepartments(data);
    } catch (error) {
      showNotification('Error loading departments', 'error');
    }
  }, [showNotification]);

  const handleCreate = useCallback(() => {
    setEditingId(null);
    setForm({
      firstName: '',
      lastName: '',
      email: '',
      departmentId: departments[0]?.id || 1,
      salary: 0,
      position: '',
      dateOfBirth: '',
      dateOfJoining: new Date().toISOString().split('T')[0],
      isActive: true
    });
    setOpen(true);
  }, [departments]);

  const handleEdit = useCallback((employee: Employee) => {
    setEditingId(employee.id);
    setForm({
      firstName: employee.firstName,
      lastName: employee.lastName,
      email: employee.email,
      departmentId: employee.departmentId,
      salary: employee.salary,
      position: employee.position || '',
      dateOfBirth: employee.dateOfBirth || '',
      dateOfJoining: employee.dateOfJoining || new Date().toISOString().split('T')[0],
      isActive: employee.isActive
    });
    setOpen(true);
  }, []);

  const handleSave = useCallback(async () => {
    try {
      if (editingId) {
        await employeeService.updateEmployee(editingId, form);
        showNotification('Employee updated successfully');
      } else {
        await employeeService.createEmployee(form);
        showNotification('Employee created successfully');
      }
      setOpen(false);
      await loadEmployees();
    } catch (error) {
      showNotification('Error saving employee', 'error');
    }
  }, [editingId, form, showNotification, loadEmployees]);

  const handleDelete = useCallback(async (id: number) => {
    try {
      await employeeService.deleteEmployee(id);
      showNotification('Employee deleted successfully');
      await loadEmployees();
    } catch (error) {
      showNotification('Error deleting employee', 'error');
    }
  }, [showNotification, loadEmployees]);

  const handleDownload = useCallback(async (format: 'pdf' | 'excel' | 'csv') => {
    try {
      const filename = `employees.${format === 'csv' ? 'csv' : format === 'pdf' ? 'pdf' : 'xlsx'}`;
      await reportService.downloadEmployeeReport(format);
      showNotification(`${filename} downloaded successfully`);
    } catch (error) {
      showNotification('Error downloading report', 'error');
    }
  }, [showNotification]);

  const filteredEmployees = employees.filter(employee => {
    const matchesSearch = !searchTerm || 
      employee.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      employee.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      employee.email.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesDepartment = !selectedDepartment || employee.departmentId === selectedDepartment;
    
    return matchesSearch && matchesDepartment;
  });

  useEffect(() => {
    loadEmployees();
    loadDepartments();
  }, [loadEmployees, loadDepartments]);

  return {
    // State
    employees: filteredEmployees,
    departments,
    loading,
    open,
    form,
    editingId,
    searchTerm,
    selectedDepartment,
    notification,
    
    // Actions
    setOpen,
    setForm,
    setSearchTerm,
    setSelectedDepartment,
    handleCreate,
    handleEdit,
    handleSave,
    handleDelete,
    handleDownload,
    showNotification,
    closeNotification,
    loadEmployees,
  };
}
