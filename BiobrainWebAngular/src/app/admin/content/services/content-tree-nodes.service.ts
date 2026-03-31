import { KeyValue } from '@angular/common';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { filter, map, shareReplay, switchMap, take, tap } from 'rxjs/operators';
import {
  AttachMaterialsToNodeCommand,
  AttachMaterialsToNodeCommand_Result
} from 'src/app/api/content/attach-materials-to-node.command';
import {
  AttachQuestionsToNodeCommand,
  AttachQuestionsToNodeCommand_Result
} from 'src/app/api/content/attach-questions-to-node.command';
import { CopyNodeCommand, CopyNodeCommand_Result } from 'src/app/api/content/copy-node.command';
import {
  CreateContentTreeNodeCommand,
  CreateContentTreeNodeCommand_Result
} from 'src/app/api/content/create-content-tree-node.command';
import {
  DeleteContentTreeNodeCommand,
  DeleteContentTreeNodeCommand_Result
} from 'src/app/api/content/delete-content-tree-node.command';
import { ExcludeQuestionFromAutoMapCommand } from 'src/app/api/content/exclude-question-from-automap.command';
import { GetContentTreeListQuery } from 'src/app/api/content/get-content-tree-list.query';
import { IncludeQuestionToAutoMapCommand } from 'src/app/api/content/include-question-to-automap.command';
import {
  SwichOrderForNodeCommand,
  SwichOrderForNodeCommand_Result
} from 'src/app/api/content/swich-order-for-node.command';
import {
  UpdateContentTreeNodeCommand,
  UpdateContentTreeNodeCommand_Result
} from 'src/app/api/content/update-content-tree-node.command';
import { TreeMode } from 'src/app/api/enums/tree-mode.enum';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { RequestValidationException, validationExceptionToString } from 'src/app/core/exceptions/request-validation.exception';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { hasValue } from 'src/app/share/helpers/has-value';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';

import { Api } from '../../../api/api.service';
import { AttachContentDialogData } from '../dialogs/attach-content-dialog/attach-content-dialog-data';
import { AttachContentDialog } from '../dialogs/attach-content-dialog/attach-content-dialog.component';
import { ContentTreeNodeDialogData, Result } from '../dialogs/content-tree-node-dialog/content-tree-node-dialog-data';
import { ContentTreeNodeDialogComponent } from '../dialogs/content-tree-node-dialog/content-tree-node-dialog.component';

@Injectable()
export class ContentTreeNodeService extends DisposableSubscriptionService {
  createdNodes$: ReplaySubject<CreateContentTreeNodeCommand_Result[]>;
  copiedNodes$: ReplaySubject<CopyNodeCommand_Result[]>;
  updatedNode$: ReplaySubject<UpdateContentTreeNodeCommand_Result>;
  deletedNode$: ReplaySubject<DeleteContentTreeNodeCommand_Result>;
  switchedNodes$: ReplaySubject<SwichOrderForNodeCommand_Result>;
  attachedMaterials$: ReplaySubject<AttachMaterialsToNodeCommand_Result>;
  attachedQuestions$: ReplaySubject<AttachQuestionsToNodeCommand_Result>;


  private readonly _defaultOrder = 1;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _loaderService: LoaderService,
    private readonly _appEvents: AppEventProvider
  ) {
    super();
    const bufferSize = 1;
    this.createdNodes$ = new ReplaySubject<CreateContentTreeNodeCommand_Result[]>(bufferSize);
    this.copiedNodes$ = new ReplaySubject<CopyNodeCommand_Result[]>(bufferSize);
    this.updatedNode$ = new ReplaySubject<UpdateContentTreeNodeCommand_Result>(bufferSize);
    this.deletedNode$ = new ReplaySubject<DeleteContentTreeNodeCommand_Result>(bufferSize);
    this.switchedNodes$ = new ReplaySubject<SwichOrderForNodeCommand_Result>(bufferSize);
    this.attachedMaterials$ = new ReplaySubject<AttachMaterialsToNodeCommand_Result>(bufferSize);
    this.attachedQuestions$ = new ReplaySubject<AttachQuestionsToNodeCommand_Result>(bufferSize);
  }

  create(courseId: string, parentId: string | null, levelHeader: string, order: number): void {
    const dialogData = new ContentTreeNodeDialogData('', courseId, parentId, '', order, levelHeader, false);
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._dialog.observe(ContentTreeNodeDialogComponent, dialogData)
        .pipe(
          map(_ => _.data),
          filter(hasValue),
          filter(x => isValidForAdd(x)),
          tap(() => this._loaderService.show()),
          map(x => new CreateContentTreeNodeCommand(x.courseId, x.parentId, x.header ?? '', x.order ?? 0, x.isAvailableInDemo)),
          switchMap(x => this._api.send(x)),
          tap(x => this.createdNodes$.next(x))
        )
        .subscribe(onFinish, onFinish)
    );
  }

  edit(entityId: string, header: string, levelHeader: string, isAvailableInDemo: boolean): void {
    const dialogData = new ContentTreeNodeDialogData(entityId, '', '', header, this._defaultOrder, levelHeader, isAvailableInDemo);
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    const dialogResult$ = this._dialog.observe(ContentTreeNodeDialogComponent, dialogData).pipe(
      map(_ => _.data),
      filter(hasValue),
      shareReplay(1)
    );

    // TODO: implement server error processing
    this.subscriptions.push(
      dialogResult$
        .pipe(
          filter(x => x.result === Result.update),
          filter(x => isValidForEdit(x)),
          tap(() => this._loaderService.show()),
          map(x => new UpdateContentTreeNodeCommand(x.entityId, x.header ?? '', x.isAvailableInDemo)),
          switchMap(x => this._api.send(x)),
          tap(x => this.updatedNode$.next(x))
        )
        .subscribe(onFinish, onFinish),

      dialogResult$
        .pipe(
          filter(x => x.result === Result.delete),
          map(x => new DeleteConfirmationDialogData(x.levelHeader, x.header)),
          switchMap(x => this._dialog.observe(DeleteConfirmationDialog, x)),
          map(_ => _.data),
          filter(x => x?.confirmed === true),
          tap(() => this._loaderService.show()),
          map(() => new DeleteContentTreeNodeCommand(entityId)),
          switchMap(x => this._api.send(x)),
          tap(x => this.deletedNode$.next(x))
        )
        .subscribe(onFinish, onFinish)
    );
  }

  swap(entity1Id: string, entity2Id: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    const command = new SwichOrderForNodeCommand(entity1Id, entity2Id);
    this._api.send(command)
      .pipe(
        take(1),
        tap(() => this._loaderService.show()),
        tap(x => this.switchedNodes$.next(x))
      ).subscribe(onFinish, onFinish);
  }

  copyNodeFromGeneric(nodeId: string, baseCourseId: string, ): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._api
        .send(new GetContentTreeListQuery(baseCourseId, TreeMode.Topics))
        .pipe(
          take(1),
          map(x => new AttachContentDialogData(x, [], nodeId, '')),
          switchMap(x => this._dialog.observe(AttachContentDialog, x)),
          map(_ => _.data),
          filter(hasValue),
          tap(() => this._loaderService.show()),
          map(dd => new CopyNodeCommand(nodeId, dd.idsToAttach)),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.copiedNodes$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }

  attachMaterials(nodeId: string, baseCourseId: string, currentMaterials: string[], stringToSearch: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._api
        .send(new GetContentTreeListQuery(baseCourseId, TreeMode.Materials))
        .pipe(
          take(1),
          map(x => new AttachContentDialogData(x, currentMaterials, nodeId, stringToSearch)),
          switchMap(x => this._dialog.observe(AttachContentDialog, x)),
          map(_ => _.data),
          filter(hasValue),
          tap(() => this._loaderService.show()),
          map(dd => new AttachMaterialsToNodeCommand(nodeId, this.getOrderIdDict(dd.idsToAttach), dd.isReplace)),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.attachedMaterials$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }

  attachQuestions(nodeId: string, baseCourseId: string, currentQuestions: string[], stringToSearch: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._api
        .send(new GetContentTreeListQuery(baseCourseId, TreeMode.Questions))
        .pipe(
          take(1),
          map(x => new AttachContentDialogData(x, currentQuestions, nodeId, stringToSearch)),
          switchMap(x => this._dialog.observe(AttachContentDialog, x)),
          map(_ => _.data),
          filter(hasValue),
          tap(() => this._loaderService.show()),
          map(dd => new AttachQuestionsToNodeCommand(nodeId, this.getOrderIdDict(dd.idsToAttach), dd.isReplace)),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.attachedQuestions$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }

  async excludeQuestionFromAutoMap(questionId: string, quizId: string){
    try{
      this._loaderService.show();
      await firstValueFrom(this._api.send(new ExcludeQuestionFromAutoMapCommand(quizId, questionId)));
    }
    catch (e) {
      if(e instanceof RequestValidationException){
        this._appEvents.errorEmit(validationExceptionToString(e));
        return;
      }
      console.log(e);
      this._appEvents.errorEmit(this._strings.errors.errorRetrivingDataFromServer);
    }
    finally{
      this._loaderService.hide();
    }
  }

  async includeQuestionToAutoMap(questionId: string, quizId: string){
    try{
      this._loaderService.show();
      await firstValueFrom(this._api.send(new IncludeQuestionToAutoMapCommand(quizId, questionId)));
    }
    catch (e) {
      if(e instanceof RequestValidationException){
        this._appEvents.errorEmit(validationExceptionToString(e));
        return;
      }
      console.log(e);
      this._appEvents.errorEmit(this._strings.errors.errorRetrivingDataFromServer);
    }
    finally{
      this._loaderService.hide();
    }
  }

  async saveAvailableInDemo(levelId: string, header: string, isAvailableInDemo: boolean){
    try{
      this._loaderService.show();
      await firstValueFrom(this._api.send(new UpdateContentTreeNodeCommand(levelId, header, isAvailableInDemo)));
    }
    catch (e) {
      if(e instanceof RequestValidationException){
        this._appEvents.errorEmit(validationExceptionToString(e));
        return;
      }
      console.log(e);
      this._appEvents.errorEmit(this._strings.errors.errorRetrivingDataFromServer);
    }
    finally{
      this._loaderService.hide();
    }
  }

  updateMaterials(nodeId: string, currentMaterials: string[]): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    const cmd = new AttachMaterialsToNodeCommand(nodeId, this.getOrderIdDict(currentMaterials), false);
    this._api.send(cmd)
      .pipe(
        take(1),
        tap(() => this._loaderService.show()),
        tap(result => this.attachedMaterials$.next(result))
      ).subscribe(onFinish, onFinish);
  }

  updateQuestions(nodeId: string, currentQuestions: string[]): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    const cmd = new AttachQuestionsToNodeCommand(nodeId, this.getOrderIdDict(currentQuestions), false);
    this._api.send(cmd)
      .pipe(
        tap(() => this._loaderService.show()),
        tap(result => this.attachedQuestions$.next(result))
      ).subscribe(onFinish, onFinish);
  }

  getOrderIdDict(idsToAttach: string[]): Array<KeyValue<number, string>> {
    const result = new Array<KeyValue<number, string>>();
    idsToAttach.forEach(x => result.push({ 'key': idsToAttach.indexOf(x) + 1, 'value': x }));
    return result;
  }
}


export function isValidForAdd(data: ContentTreeNodeDialogData): boolean {
  if (!data.header) {
    return false;
  }

  if (!data.courseId) {
    return false;
  }

  if (data.order <= 0) {
    return false;
  }

  return true;
}

export function isValidForEdit(data: ContentTreeNodeDialogData): boolean {
  if (!data.header) {
    return false;
  }

  return true;
}
