import {RouterConfiguration, Router} from 'aurelia-router';
import {PLATFORM} from 'aurelia-pal';

export class App {
  router: Router;

  configureRouter(config: RouterConfiguration, router: Router) {

    config.title = 'Support Wheel Of Fate';

    config.options.pushState = true;
    config.map([
      {
        route: '',
        redirect: 'schedule'
      },
      {
        route: 'schedule/:week?',
        name: 'week-schedule',
        title: '',
        nav: false,
        moduleId: PLATFORM.moduleName('./views/schedule/schedule')
      }]);

    config.mapUnknownRoutes((instruction) => {
        return {route: 'not-found', redirect: 'schedule'};
      }
    );

    this.router = router;
  }
}
