import { Injectable } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { DisposableSubscriptionService } from "src/app/core/services/disposable-subscription.service";
import { RequestHandlingService } from "src/app/share/services/request-parse.service";
import { StringsService } from "src/app/share/strings.service";

@Injectable()
export class QuizResultHistoryPageService extends DisposableSubscriptionService {

    constructor(
        public strings: StringsService,
        public readonly requestService: RequestHandlingService,
    ) {
        super();
    }

    init(activatedRoute: ActivatedRoute) {
    }

   
}