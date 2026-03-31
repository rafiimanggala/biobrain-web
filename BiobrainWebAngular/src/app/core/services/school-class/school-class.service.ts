import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { Api } from "src/app/api/api.service";
import { GetSchoolClassByIdQuery } from "src/app/api/school-classes/get-school-class-by-id.query";

@Injectable({
    providedIn: 'root',
  })
export class SchoolClassService {
    constructor(private readonly _api: Api) {
    }

    public getById(schoolClassId: string): Observable<SchoolClassModel> {
        return this._api.send(new GetSchoolClassByIdQuery(schoolClassId)).pipe(
            map(schoolClass =>
                new SchoolClassModel(
                    schoolClass.schoolClassId,
                    schoolClass.schoolId,
                    schoolClass.courseId,
                    schoolClass.year,
                    schoolClass.name,
                    schoolClass.autoJoinClassCode,
                    schoolClass.teacherIds,
                    schoolClass.students
                ),
            ),
        );
    }
}

export class SchoolClassModel {
    // ${this.year == 10 || this.year == 9 ? '' : this.year.toString() + " "}
    public get fullName() {return `${this.name}`;}
    constructor(
        public readonly schoolClassId: string,
        public readonly schoolId: string,
        public readonly courseId: string,
        public readonly year: number,
        public readonly name: string,
        public readonly autoJoinClassCode: string,
        public readonly teacherIds: string[],
        public readonly students: {studentId: string, email: string}[]
    ) { }
}