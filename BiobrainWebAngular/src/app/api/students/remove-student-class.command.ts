import { Injectable } from '@angular/core';

import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class RemoveStudentClassCommand  extends Command<EmptyCommandResult>{
  constructor(
    public readonly studentId: string,
    public readonly schoolClassId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/RemoveStudentClass`;
  }
}
