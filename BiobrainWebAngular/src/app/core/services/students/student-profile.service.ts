import { Injectable } from '@angular/core';
import { GetStudentProfileStateQuery } from 'src/app/api/accounts/get-student-profile-state.query';

import { Api } from '../../../api/api.service';


@Injectable({
  providedIn: 'root',
})
export class StudentProfileService {

  constructor(private readonly _api: Api) {
  }

  public async getStudentProfileState(studentId: string){
    var state = await this._api.send(new GetStudentProfileStateQuery(studentId)).toPromise();
    return state;
  }
}
