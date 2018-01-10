import * as moment from 'moment';

export class DateFormatValueConverter {
  toView(value, format, inUTC) {
    if (inUTC) {
      return moment(value).utc().format(format);
    } else {
      return moment(value).format(format);
    }
  }
}
