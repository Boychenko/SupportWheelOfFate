import {inject} from 'aurelia-framework';
import {Endpoint, Rest} from 'aurelia-api';

@inject(Endpoint.of('api'))
export abstract class ApiService {

  protected constructor(protected readonly api: Rest) {
  }
}
