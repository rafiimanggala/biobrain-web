import { Injectable, OnDestroy } from "@angular/core";
import { Router, NavigationEnd } from "@angular/router";
import { Subscription } from "rxjs";
import { GetAnalyticsDataQuery } from "src/app/api/analytics/get-analytics-data.query";
import { Api } from "src/app/api/api.service";
import { PageViewQuery } from "src/app/api/tracking/page-view.query";
import { CurrentUserService } from "src/app/auth/services/current-user.service";
import { GoogleAnalyticsService } from "src/app/google-analytics/ga.service";
import { firstValueFrom } from "src/app/share/helpers/first-value-from";
import { ActiveCourseService } from "./active-course.service";
import { ActiveSchoolService } from "./active-school.service";

@Injectable()
export class AnalyticsService implements OnDestroy {
    public isInited = false;
    private subscriptions: Subscription[] = [];
    // private trackTimer: Observable<number> = interval(60000);
    // private timerSubscription?: Subscription;

    constructor(
        private readonly router: Router,
        private readonly api: Api,
        private readonly gaService: GoogleAnalyticsService,
        private readonly activeSchoolService: ActiveSchoolService,
        private readonly activeCourseSerice: ActiveCourseService,
        private readonly userData: CurrentUserService
    ) { }

    ngOnDestroy(): void {
        this.unsubscribe();
    }

    public async initAnalytics() {
        try {
            if (this.isInited) return;
            var result = await this.api.send(new GetAnalyticsDataQuery()).toPromise();
            if (!result) throw new Error("Google analytics not inited");
            if (!result.trackingNumber) return;
            this.gaService.configure(result.trackingNumber);

            this.unsubscribe();
            // subscribe to router events and send page views to Google Analytics
            this.subscriptions.push(this.router.events.subscribe(this.handleNavigation.bind(this)));

            // subscribe to track timer
            // if (!this.timerSubscription) {
            //     this.handleTracking();
            //     this.timerSubscription = this.trackTimer.subscribe(this.handleTracking.bind(this));
            //     this.subscriptions.push(this.timerSubscription);
            // }

            this.isInited = true;
        } catch (e) {
            console.log(e);
        }
    }

    private unsubscribe() {
        this.subscriptions.forEach(x => x.unsubscribe());
    }

    public async handleNavigation(event: any) {
        var user = await this.userData.user;
        var school = await this.activeSchoolService.schoolName;
        var schoolId = await this.activeSchoolService.schoolId;
        var courseId = await this.activeCourseSerice.courseId;

        if (!user) return;

        try {
            if (event instanceof NavigationEnd) {
                var pagePath = this.resolveUrl(event.urlAfterRedirects);
                this.gaService.pageview.emit({
                    page: pagePath, fieldsObject: {
                        userId: user.userId,
                        dimension1: user.isStudent() ? ("Student")
                            : user.isTeacher() ? "Teacher" : "Admin",
                        dimension2: school ? school : " ",
                        dimension3: user.subscriptionType == '2' ? 'Trial' : user.subscriptionType == '3' ? 'Access Code' : this.getStatusCode(user.subscriptionStatus),
                    }
                });

                firstValueFrom(this.api.send(new PageViewQuery(pagePath, courseId, schoolId)));
            }
        } catch (e) {
            console.log(e);
        }
    }

    // private async handleTracking(){
    //     try{
    //         var schoolId = await this.activeSchoolService.schoolId;
    //         var courseId = await this.activeCourseSerice.courseId;
    //         await firstValueFrom(this.api.send(new TrackSessionQuery(courseId, schoolId)));
    //     }
    //     catch(e){
    //         console.log(e);
    //     }
    // }

    getStatusCode(subscriptionStatus: string | undefined) {
        switch (subscriptionStatus) {
            case '1': return "Active subscription";
            case '2': return "Payment failed";
            case '3': return "Stopped by user";
            default: return "Inactive";
        }
    }

    private resolveUrl(fullUrl: string) {
        if (fullUrl.includes("/materials/course")) return "/materials/course";
        if (fullUrl.includes("/materials-search")) return "/materials-search";
        if (fullUrl.includes("/quiz")) return "/quiz";
        if (fullUrl.includes("/quiz-result")) return "/quiz-result";
        if (fullUrl.includes("/teacher/student-results")) return "/teacher/student-results";
        if (fullUrl.includes("/assigned-work")) return "/assigned-work";
        if (fullUrl.includes("/my-courses")) return "/my-courses";
        if (fullUrl.includes("/admin/schools")) return "/admin/schools";
        if (fullUrl.includes("/login")) return "/login";
        if (fullUrl.includes("/subscription")) return "/subscription";
        if (fullUrl.includes("/student/my-courses")) return "/student/my-courses";
        if (fullUrl.includes("/perform-assigned-learning-material")) return "/perform-assigned-learning-material";
        return fullUrl;
    }
}
