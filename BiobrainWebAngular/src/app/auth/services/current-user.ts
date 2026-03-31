import { ScheduledPaymentStatus } from 'src/app/share/values/scheduled.payment.status';
import { hasValue } from '../../share/helpers/has-value';
import { UserRoles } from '../../share/values/user-roles.enum';

export class CurrentUser {
  public displayName: string;
  public initials: string;
  public highestRole: string;

  constructor(
    public readonly userId: string,
    public readonly name: string,
    public roles: string[],
    public schoolIds: string[] | undefined,
    public adminSchoolIds: string[] | undefined,
    public schoolName: string | undefined,
    public firstName: string | undefined,
    public lastName: string | undefined,
    public subscriptionStatus: string | undefined,
    public subscriptionType: string | undefined,
  ) {
    this.displayName = this._getDisplayName();
    this.initials = this._getInitials();
    this.highestRole = this._getHighestRole();
  }

  public hasRole(role: UserRoles): boolean {
    return this.roles.some(_ => _ === role);
  }

  public isStudent(): boolean {
    return this.hasRole(UserRoles.student);
  }

  public isLiveStudent(): boolean {
    return this.hasRole(UserRoles.student) && this.subscriptionType != '0' && this.subscriptionType != '2';
  }

  //  public isStandalone(): boolean {
  //   return !hasValue(this.schoolId) && this.isStudent();
  // }

  public isSubscriptionValid(): boolean {
    return hasValue(this.subscriptionStatus) && (+this.subscriptionStatus == ScheduledPaymentStatus.Success || +this.subscriptionStatus == ScheduledPaymentStatus.StoppedByUser);
  }

  public isDemoSubscription(): boolean{
    return  this.subscriptionType == '2';
  }

  public isTeacher(): boolean {
    return this.hasRole(UserRoles.teacher);
  }

  public isSysAdmin(): boolean {
    return this.hasRole(UserRoles.systemAdministrator);
  }

  public isSchoolAdmin(): boolean {
    return this.hasRole(UserRoles.schoolAdministrator) && (this.adminSchoolIds?.length ?? 0) > 0;
  }

  private _getDisplayName(): string {
    return hasValue(this.firstName) || hasValue(this.lastName) ? `${this.firstName ?? ''} ${this.lastName ?? ''}` : this.name;
  }

  private _getInitials(): string {
    const getLetter = (str: string | undefined): string => hasValue(str) && str.length > 0 ? str?.substr(0, 1)?.toUpperCase() : '';

    if (hasValue(this.firstName) || hasValue(this.lastName)) {
      return `${getLetter(this.firstName)}${getLetter(this.lastName)}`;
    }

    return getLetter(this.name);
  }

  private _getHighestRole(): string {
    const rolesByPriority = [
      UserRoles.systemAdministrator,
      UserRoles.schoolAdministrator,
      UserRoles.teacher,
      UserRoles.student,
    ];

    if (this.roles.length === 0) throw new Error('User has no roles.');

    for (const role of rolesByPriority) {
      if (this.hasRole(role)) return role;
    }

    throw new Error('Highest role was not found.');
  }
}
