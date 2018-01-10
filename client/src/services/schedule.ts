import {ApiService} from './api';

export interface IScheduleEntry {
  engineer: string;
  date: string; //ISO date
  shift: number;
}

export interface IWeekSchedule {
  start: string; //ISO date
  end: string; //ISO date
  entries: IScheduleEntry[];
  nextWeek?: string;
  previousWeek?: string;
}

export class ScheduleService extends ApiService {
  getWeekSchedule(week?: string): Promise<IWeekSchedule> {
    return this.api.find(`schedule/${week || ''}`);
  }
}
