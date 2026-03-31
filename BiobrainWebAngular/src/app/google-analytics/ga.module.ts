import {ModuleWithProviders, NgModule} from '@angular/core';

import {GoogleAnalyticsService} from './ga.service';

@NgModule({
	imports: [],
	declarations: [],
	exports: []
})
export class GoogleAnalyticsModule {
	static forRoot(): ModuleWithProviders<GoogleAnalyticsModule> {
		return {
			ngModule: GoogleAnalyticsModule,
			providers: [
				GoogleAnalyticsService
			]
		};
	}
}
