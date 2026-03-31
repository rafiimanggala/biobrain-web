import { Injectable } from '@angular/core';
import { StringsService } from 'src/app/share/strings.service';
import { UserRoles } from 'src/app/share/values/user-roles.enum';
import { hasValue } from '../../share/helpers/has-value';

import { CurrentUserService } from './current-user.service';

@Injectable({
  providedIn: 'root',
})
export class HomePageService {
  constructor(
    private readonly _currentUserService: CurrentUserService,
    private readonly _strings: StringsService,
  ) {
  }

  public async getHomePage(): Promise<string> {
    const user = await this._currentUserService.user;

    if (!hasValue(user)) {
      return 'login';
    }

    switch (user.highestRole) {
      case UserRoles.systemAdministrator:
        return 'admin/schools';
      case UserRoles.schoolAdministrator:
      case UserRoles.teacher:
        return 'teacher/my-classes';
      case UserRoles.student:
        return 'student/my-courses';
      default:
        throw new Error(this._strings.userDataError);
    }
  }
}
