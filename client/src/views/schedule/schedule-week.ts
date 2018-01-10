import {bindable} from 'aurelia-framework';
import {IScheduleEntry, IWeekSchedule} from '../../services/schedule';
import * as moment from 'moment';

interface IWeekDayInfo {
  date: string;
  entries: IScheduleEntry[];
  today: boolean;
}

export class ScheduleWeek {
  @bindable() weekSchedule: IWeekSchedule;

  weekView: IWeekDayInfo[];

  weekScheduleChanged(val: IWeekSchedule) {
    if (val) {
      const grouped = val.entries.reduce((result, val) => {
        if (result.has(val.date)) {
          result.get(val.date).push(val);
        } else {
          result.set(val.date, [val]);
        }
        return result;
      }, new Map<string, IScheduleEntry[]>());
      this.weekView = Array.from(grouped.keys()).map((date) => {
        return {
          date: date,
          entries: grouped.get(date),
          today: this.isToday(date)
        } as IWeekDayInfo
      });
    } else {
      this.weekView = [];
    }
  }

  private isToday(date: string) {
    return moment.utc(date).isSame(moment.utc().startOf('day'))
  }
}
