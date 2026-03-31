/* eslint-disable @typescript-eslint/naming-convention */
export class JwtPayloadModel {
  sub?: string;
  name?: string;
  role?: string | string[];
  schoolId?: string;
  adminSchoolId?: string;
  schoolName?: string;
  given_name?: string;
  family_name?: string;
  subscription_status?: string;
  subscription_type?: string;
}

export function compareJwtPayloadModel(a: JwtPayloadModel | undefined, b: JwtPayloadModel | undefined): boolean {
  if (a !== undefined && b !== undefined) {
    return a.sub === b.sub
      && a.name === b.name
      && JSON.stringify(a.role) === JSON.stringify(b.role)
      && a.schoolId === b.schoolId
      && a.given_name === b.given_name
      && a.family_name === b.family_name
      && a.subscription_status === b.subscription_status
      && a.subscription_type === b.subscription_type;
  }

  return a === undefined && b === undefined;
}
