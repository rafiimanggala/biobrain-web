import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs/operators';
import { SignUpStandaloneStudentCommand } from 'src/app/api/accounts/sign-up-standalone-student.command';
import { SignUpStudentWithAccessCodeCommand } from 'src/app/api/accounts/sign-up-student-with-access-code.command';
import { UnprocessableEntityException } from 'src/app/core/exceptions/unprocessable-entity.exception';
import { AppSettings } from 'src/app/share/values/app-settings';

import { SignUpStudentCommand } from '../../../api/accounts/sign-up-student.command';
import { Api } from '../../../api/api.service';
import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { RequestValidationException, validationExceptionToString } from '../../../core/exceptions/request-validation.exception';
import { SnackBarService } from '../../../share/services/snack-bar.service';
import { StringsService } from '../../../share/strings.service';
import { LoginModel } from '../../models/login.model';
import { LoginOperation } from '../../operations/login.operation';
import { RoutingService } from '../../services/routing.service';
import {
  SignUpDetailsData,
} from '../sign-up-details/sign-up-details-data';
import { SignUpStudentWithVoucherCommand } from 'src/app/api/accounts/sign-up-student-with-voucher.command';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss'],
})
export class SignUpComponent {

  private _isFreeTrial: boolean = false;
  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _routingService: RoutingService,
    private readonly _appEvents: AppEventProvider,
    private readonly _snackBarService: SnackBarService,
    private readonly _loginOperation: LoginOperation,
    activatedRoute: ActivatedRoute,
  ) {
    this._isFreeTrial = activatedRoute.snapshot.url[0].path == AppSettings.freeTrialPath;
  }

  onSignUp($event: SignUpDetailsData): Promise<void> {
    // should allow to sign-up as standalone teacher & standalone student
    // so need to resolve it by option and select these options somehow
    if ($event.classCode)
      return this._signUpAsSchoolStudent($event);

    if ($event.accessCode)
      return this._signUpWithAccessCodeStudent($event);

    if ($event.voucher)
      return this._signUpWithVoucherStudent($event);

    return this._signUpAsStandaloneStudent($event);
  }

  async onLogin(): Promise<void> {
    await this._routingService.navigateToLoginPage();
  }

  private async _signUpAsSchoolStudent($event: SignUpDetailsData): Promise<void> {
    try {
      const cmd = this._signUpDataToSignUpAsSchoolStudentCommand($event);
      await this._api.send(cmd).pipe(take(1)).toPromise();

      let model = new LoginModel();
      model.username = $event.email;
      model.password = $event.password;
      let result = await this._loginOperation.perform(model);
      if (result.isFailed()) {
        this._appEvents.errorEmit(result.reason);
        return;
      }
      await this._routingService.navigateToHome(true);
      // this._snackBarService.showMessage(this.strings.youHaveSuccessfullyRegistered);
      // await this._routingService.navigateToLoginPage();
    } catch (e) {
      const isClassNotFoundByClassCode = (err: RequestValidationException): boolean => {
        const pattern = `'SchoolClassEntity' related to 'Class Code' = '${$event.classCode ?? ''}' was not found.`;
        for (const key in err.errors) {
          if (key === 'ClassCode') {
            const fieldErrors = err.errors[key];
            if (fieldErrors.some(x => x === pattern)) {
              return true;
            }
          }
        }

        return false;
      };

      if (e instanceof RequestValidationException) {
        if (isClassNotFoundByClassCode(e)) {
          this._snackBarService.showMessage(this.strings.wrongClassCode);
        } else {
          for (const key in e.errors) {
            this._snackBarService.showMessage(e.errors[key].join(', '));
          }
        }
      } else if (e instanceof UnprocessableEntityException) {
        if (e.name == 'NotEnoughStudentsLicensesException')
          this._snackBarService.showMessage(this.strings.studentsLicenseLimitExceeded);
        if (e.name == 'ClassCodeNotAvailableException')
          this._snackBarService.showMessage(this.strings.classCodeNotAvailableError);
        else
          this._snackBarService.showMessage(e.name);

      } else {
        this._appEvents.errorEmit(<string>e);
      }
    }
  }

  private async _signUpWithAccessCodeStudent($event: SignUpDetailsData): Promise<void> {
    try {
      const cmd = this._signUpDataToSignUpStudentWithAccessCodeCommand($event);
      await this._api.send(cmd).pipe(take(1)).toPromise();

      let model = new LoginModel();
      model.username = $event.email;
      model.password = $event.password;
      let result = await this._loginOperation.perform(model);
      if (result.isFailed()) {
        this._appEvents.errorEmit(result.reason);
        return;
      }
      await this._routingService.navigateToHome(true);
      // this._snackBarService.showMessage(this.strings.youHaveSuccessfullyRegistered);
      // await this._routingService.navigateToLoginPage();
    } catch (e) {

      if (e instanceof RequestValidationException) {
        this._snackBarService.showMessage(validationExceptionToString(e, ' '));
      } else if (e instanceof UnprocessableEntityException) {
        if (e.name == 'NotEnoughStudentsLicensesException')
          this._snackBarService.showMessage(this.strings.studentsLicenseLimitExceeded);
        else
          this._snackBarService.showMessage(e.name);

      } else {
        this._appEvents.errorEmit(<string>e);
      }
    }
  }

  private async _signUpWithVoucherStudent($event: SignUpDetailsData): Promise<void> {
    try {
      const cmd = this._signUpDataToSignUpStudentWithVoucherCommand($event);
      await this._api.send(cmd).pipe(take(1)).toPromise();

      let model = new LoginModel();
      model.username = $event.email;
      model.password = $event.password;
      let result = await this._loginOperation.perform(model);
      if (result.isFailed()) {
        this._appEvents.errorEmit(result.reason);
        return;
      }
      await this._routingService.navigateToSubscriptionPage(true, false);
      // this._snackBarService.showMessage(this.strings.youHaveSuccessfullyRegistered);
      // await this._routingService.navigateToLoginPage();
    } catch (e) {

      if (e instanceof RequestValidationException) {
        this._snackBarService.showMessage(validationExceptionToString(e, ' '));
      } else if (e instanceof UnprocessableEntityException) {
        if (e.name == 'NotEnoughStudentsLicensesException')
          this._snackBarService.showMessage(this.strings.studentsLicenseLimitExceeded);
        else
          this._snackBarService.showMessage(e.name);

      } else {
        this._appEvents.errorEmit(<string>e);
      }
    }
  }

  private async _signUpAsStandaloneStudent($event: SignUpDetailsData): Promise<void> {
    try {
      // signup
      const cmd = this._signUpDataToSignUpAsStandaloneStudentCommand($event);
      await this._api.send(cmd).pipe(take(1)).toPromise();

      // login
      let model = new LoginModel();
      model.username = $event.email;
      model.password = $event.password;
      let result = await this._loginOperation.perform(model);
      if (result.isFailed()) {
        this._appEvents.errorEmit(result.reason);
        return;
      }

      await this._routingService.navigateToSubscriptionPage(true, this._isFreeTrial);
    } catch (e) {
      const isClassNotFoundByClassCode = (err: RequestValidationException): boolean => {
        const pattern = `'SchoolClassEntity' related to 'Class Code' = '${$event.classCode ?? ''}' was not found.`;
        for (const key in err.errors) {
          if (key === 'ClassCode') {
            const fieldErrors = err.errors[key];
            if (fieldErrors.some(x => x === pattern)) {
              return true;
            }
          }
        }

        return false;
      };

      if (e instanceof RequestValidationException) {
        if (isClassNotFoundByClassCode(e)) {
          this._snackBarService.showMessage(this.strings.wrongClassCode);
        } else {
          for (const key in e.errors) {
            this._snackBarService.showMessage(e.errors[key].join(', '));
          }
        }
      } else {
        this._appEvents.errorEmit(<string>e);
      }
    }
  }

  private _signUpDataToSignUpAsSchoolStudentCommand($event: SignUpDetailsData): SignUpStudentCommand {
    return new SignUpStudentCommand(
      $event.email,
      $event.password,
      $event.firstName,
      $event.lastName,
      $event.classCode ?? '',
      $event.country,
      $event.state,
      $event.curriculumCode
    );
  }

  private _signUpDataToSignUpAsStandaloneStudentCommand($event: SignUpDetailsData): SignUpStandaloneStudentCommand {
    return new SignUpStandaloneStudentCommand(
      $event.email,
      $event.password,
      $event.firstName,
      $event.lastName,
      $event.country,
      $event.state,
      $event.year ?? 0,
      $event.curriculumCode
    );
  }

  private _signUpDataToSignUpStudentWithAccessCodeCommand($event: SignUpDetailsData): SignUpStudentWithAccessCodeCommand {
    return new SignUpStudentWithAccessCodeCommand(
      $event.email,
      $event.password,
      $event.firstName,
      $event.lastName,
      $event.accessCode ?? '',
      $event.country,
      $event.state,
      $event.curriculumCode
    );
  }

  private _signUpDataToSignUpStudentWithVoucherCommand($event: SignUpDetailsData): SignUpStudentWithVoucherCommand {
    return new SignUpStudentWithVoucherCommand(
      $event.email,
      $event.password,
      $event.firstName,
      $event.lastName,
      $event.voucher ?? '',
      $event.schoolName,
      $event.country,
      $event.state,
      $event.curriculumCode
    );
  }
}
