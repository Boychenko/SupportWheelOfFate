import {autoinject} from 'aurelia-framework';
import {Router} from 'aurelia-router';
import {IWeekSchedule, ScheduleService} from '../../services/schedule';
import {DateFormatValueConverter} from '../../resources/value-converters/dateformat';

@autoinject()
export class Schedule {
  weekSchedule: IWeekSchedule;

  constructor(private readonly router: Router,
              private readonly dateFormat: DateFormatValueConverter,
              private readonly scheduleService: ScheduleService) {
  }

  async activate(params) {
    this.weekSchedule = await this.scheduleService.getWeekSchedule(params.week);
    this.updateTitle();
  }

  attached() {
    this.updateTitle()
  }

  private formatDate(date: string): string {
    return `${this.dateFormat.toView(date, 'YYYY-MM-DD', true)}`
  }

  private updateTitle() {
    if (this.router.currentInstruction) {
      this.router.currentInstruction.config.navModel.setTitle(`${this.formatDate(this.weekSchedule.start)} - ${this.formatDate(this.weekSchedule.end)}`);
    }
  }
}
