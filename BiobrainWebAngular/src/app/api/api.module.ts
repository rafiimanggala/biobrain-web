import { ApiPath } from './api-path.service';
import { NgModule } from '@angular/core';

const components: any[] = [];

const dialogs: any[] = [];

const services: any[] = [];

@NgModule({
  imports: [],
  declarations: [components, dialogs],
  providers: [services, ApiPath],
  entryComponents: [dialogs],
})
export class ApiModule {}
