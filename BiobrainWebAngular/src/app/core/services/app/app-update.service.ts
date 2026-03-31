import { Injectable, OnDestroy } from "@angular/core";
import { SwUpdate } from "@angular/service-worker";
import moment, { Moment } from "moment";
import { Subscription, timer } from "rxjs";
import { LoaderService } from "src/app/share/services/loader.service";
import { AppEventProvider } from "../../app/app-event-provider.service";

@Injectable({
    providedIn: 'root'
})
export class AppUpdateService implements OnDestroy {

    private _lastCheck?: Moment;
    private _timer = timer(1000, 1000);
    private _timerSubscrption: Subscription;

    constructor(private readonly updates: SwUpdate, private readonly appEvents: AppEventProvider, private readonly loaderService: LoaderService) {
        this._timerSubscrption = this._timer.subscribe(() => this.checkForUpdateIfNeed());

        if (!this.updates.isEnabled) return;
        this.updates.available.subscribe((() => {
            this.doAppUpdate();
        }).bind(this));
    }

    private checkForUpdateIfNeed(){
        if(this._lastCheck && (moment.duration(moment().diff(this._lastCheck)).asSeconds() > 60 )){
            this.checkForUpdates();
        }
        this._lastCheck = moment();
    }

    ngOnDestroy(): void {
        this._timerSubscrption.unsubscribe();
    }

    
    checkForUpdates(){       
        if (!this.updates.isEnabled) return;
        this.updates.checkForUpdate();    
    }

    async doAppUpdate() {
        try{
            this.loaderService.show();
            await this.updates.activateUpdate();
            window.location.reload();
        }
        catch(e: any){
            console.log(e);
            this.appEvents.errorEmit(e.message);
        }
        finally{
            this.loaderService.hideIfVisible();
        }
    }
}

