import { api } from '../lib/api';
import type { SeedStatus } from '../lib/api';

export class SeedService {
  async seedData(): Promise<{ message: string; timestamp: string }> {
    return await api.seedData();
  }

  async reseedData(): Promise<{ message: string; timestamp: string }> {
    return await api.reseedData();
  }

  async clearData(): Promise<{ message: string; timestamp: string }> {
    return await api.clearData();
  }

  async getSeedStatus(): Promise<SeedStatus> {
    return await api.getSeedStatus();
  }
}

export const seedService = new SeedService();
