import { TitleCasePipe } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BehaviorSubject, combineLatest, Subscription } from 'rxjs';
import { startWith } from 'rxjs/operators';
import { CourseListStore } from 'src/app/admin/content/content-mapper/course-list-store';
import { Api } from 'src/app/api/api.service';
import { AddNewQuestionsCommand } from 'src/app/api/content/add-new-questions.command';
import { PublishCourseCommand } from 'src/app/api/content/publish-course.command';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';
import { InformationDialog } from 'src/app/share/dialogs/information/information.dialog';
import { InformationDialogData } from 'src/app/share/dialogs/information/information.dialog-data';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { StringsService } from 'src/app/share/strings.service';
import { GetContentTreeListQuery_Result } from '../../../api/content/get-content-tree-list.query';
import { GetContentTreeMetaListQuery_Result } from '../../../api/content/get-content-tree-meta-list.query';
import { GetCoursesListQuery_Result } from '../../../api/content/get-courses-list.query';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';
import { ContentMapperTreeComponent } from '../content-mapper-tree/content-mapper-tree.component';
import { ManageAutoMapOperation } from '../operations/manage-auto-map.operation';
import { ContentTreeNodeService } from '../services/content-tree-nodes.service';
import { CreateMaterialDialogComponent } from '../dialogs/create-material-dialog/create-material-dialog.component';
import { CreateMaterialDialogData } from '../dialogs/create-material-dialog/create-material-dialog-data';
import { CreateQuestionDialogComponent } from '../dialogs/create-question-dialog/create-question-dialog.component';
import { CreateQuestionDialogData } from '../dialogs/create-question-dialog/create-question-dialog-data';
import { QuestionType } from 'src/app/api/enums/question-type.enum';
import { ContentTreeListStore } from './content-tree-list-store';
import { ContentTreeMetaListStore } from './content-tree-meta-list-store';

@Component({
  selector: 'app-content-mapper',
  templateUrl: './content-mapper.component.html',
  styleUrls: ['./content-mapper.component.scss'],
  providers: [
    CourseListStore,
    ContentTreeMetaListStore,
    ContentTreeListStore,
    ContentTreeNodeService,
    ManageAutoMapOperation
  ]
})
export class ContentMapperComponent implements OnInit, OnDestroy {

  @ViewChild("tree") tree!: ContentMapperTreeComponent;

  subscriptions: Subscription[] = [];

  baseCourses$: BehaviorSubject<GetCoursesListQuery_Result[]> = new BehaviorSubject<GetCoursesListQuery_Result[]>([]);
  courses$: BehaviorSubject<GetCoursesListQuery_Result[]> = new BehaviorSubject<GetCoursesListQuery_Result[]>([]);

  contentTreeMeta: GetContentTreeMetaListQuery_Result[] = [];

  filteredBaseCourses: GetCoursesListQuery_Result[] = [];
  baseCourse?: GetCoursesListQuery_Result;
  course?: GetCoursesListQuery_Result;

  constructor(
    public readonly strings: StringsService,
    public readonly courseListStore: CourseListStore,
    public readonly contentTreeMetaListStore: ContentTreeMetaListStore,
    public readonly contentTreeListStore: ContentTreeListStore,
    private readonly _contentTreeNodeService: ContentTreeNodeService,
    private readonly _manageAutoMapOperation: ManageAutoMapOperation,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
    private readonly _dialog: Dialog,
    private readonly _api: Api
  ) { }

  ngOnInit(): void {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(this._titlecasePipe.transform(this.strings.contentMapper));
    }, 0);

    this.subscriptions.push(this.courseListStore.items$.subscribe(x => this.updateLists(x)));
    this.subscriptions.push(this.contentTreeListStore.items$.subscribe(x => this.initTree(x)));
    this.subscriptions.push(this.contentTreeMetaListStore.items$.subscribe(x => this.contentTreeMeta = x));
    // this.subscriptions.push(this._contentTreeNodeService.createdNodes$.subscribe(x => this.addNodes(x)));

    this.contentTreeListStore.attachReload(
      combineLatest([
        this._contentTreeNodeService.createdNodes$.pipe(startWith({})),
        this._contentTreeNodeService.updatedNode$.pipe(startWith({})),
        this._contentTreeNodeService.deletedNode$.pipe(startWith({})),
        this._contentTreeNodeService.switchedNodes$.pipe(startWith({})),
        this._contentTreeNodeService.attachedMaterials$.pipe(startWith({})),
        this._contentTreeNodeService.attachedQuestions$.pipe(startWith({})),
        this._contentTreeNodeService.copiedNodes$.pipe(startWith({})),
      ]));

    this.courseListStore.bind({});
  }

  initTree(treeNodes: GetContentTreeListQuery_Result[]): void {
    this.tree?.initTree.bind(this.tree)(treeNodes);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(x => x.unsubscribe());
  }

  updateLists(items: GetCoursesListQuery_Result[] | undefined) {
    if (!items || items.length < 1) {
      this.baseCourses$.next([]);
      this.courses$.next([]);
      return;
    }
    this.baseCourses$.next(items.filter(x => x.isBase));
    this.courses$.next(items.filter(x => !x.isBase));
  }

  courseSelected(newItem: GetCoursesListQuery_Result) {
    this.contentTreeMetaListStore.bind({ courseId: newItem.courseId });
    this.contentTreeListStore.bind({ courseId: newItem.courseId });
    this.filteredBaseCourses = this.baseCourses$.value;
    this.baseCourse = this.filteredBaseCourses.find(_ => _.subjectCode == newItem.subjectCode);
  }

  baseCourseSelected(newItem: GetCoursesListQuery_Result) {
  }

  onAdd(node: GetContentTreeListQuery_Result | null) {
    let meta = node ? this.contentTreeMeta.find(x => x.depth == node.depth + 1) : this.contentTreeMeta.find(x => x.depth == 0);
    let courseId = this.course?.courseId ?? "";
    let parentId = node ? node.entityId : null;
    let levelName = meta?.name ?? this.strings.node;
    let order = node ? node.children.length + 1 : this.tree.getOrderForNewArea();
    this._contentTreeNodeService.create(courseId, parentId, levelName, order);
  }

  onCopyFromGeneric(node: GetContentTreeListQuery_Result){
    // Provide parentId as nodeId because need to attach to level not materials folder
    this._contentTreeNodeService.copyNodeFromGeneric(node.entityId, this.baseCourse?.courseId ?? '');
  }

  onEdit(node: GetContentTreeListQuery_Result) {
    let meta = this.contentTreeMeta.find(x => x.depth == node.depth);
    this._contentTreeNodeService.edit(node.entityId, node.header, meta?.name ?? this.strings.node, node.isAvailableInDemo);
  }

  onMoveUp(node: GetContentTreeListQuery_Result) {
    let node2 = this.tree.getUpperSibling(node);
    if (!node2) return;
    this._contentTreeNodeService.swap(node.entityId, node2.entityId);
  }

  onMoveDown(node: GetContentTreeListQuery_Result) {
    let node2 = this.tree.getLowerSibling(node);
    if (!node2) return;
    this._contentTreeNodeService.swap(node.entityId, node2.entityId);
  }

  async onInclude(node: GetContentTreeListQuery_Result) {
    await this._contentTreeNodeService.includeQuestionToAutoMap(node.entityId, node.parentId);
    this.contentTreeListStore.reload();
  }

  async onExclude(node: GetContentTreeListQuery_Result) {
    await this._contentTreeNodeService.excludeQuestionFromAutoMap(node.entityId, node.parentId);
    this.contentTreeListStore.reload();    
  }

  async onAvailableInDemo(node: GetContentTreeListQuery_Result) {
    await this._contentTreeNodeService.saveAvailableInDemo(node.entityId, node.header, !node.isAvailableInDemo);
    this.contentTreeListStore.reload();    
  }

  onAttachMaterials(node: GetContentTreeListQuery_Result) {
    let stringToSearch = "";
    let parent = this.tree.getNode(node.parentId);
    if (parent) {
      let praParent = this.tree.getNode(parent.parentId);
      stringToSearch = praParent?.header ?? "";
    }
    // Provide parentId as nodeId because need to attach to level not materials folder
    this._contentTreeNodeService.attachMaterials(node.parentId, this.baseCourse?.courseId ?? '', node.children?.map(x => x.entityId) ?? [], stringToSearch);
  }

  onAttachQuestions(node: GetContentTreeListQuery_Result) {
    let stringToSearch = "";
    let parent = this.tree.getNode(node.parentId);
    if (parent) {
      let praParent = this.tree.getNode(parent.parentId);
      stringToSearch = praParent?.header ?? "";
    }

    this._contentTreeNodeService.attachQuestions(node.parentId, this.baseCourse?.courseId ?? '', node.children?.map(x => x.entityId) ?? [], stringToSearch);
  }

  async onCreateMaterial(node: GetContentTreeListQuery_Result) {
    if (!this.course) return;
    const data = new CreateMaterialDialogData(
      this.course.courseId,
      node.parentId,
      null,
      '',
      '',
      ''
    );
    const result = await this._dialog.show(CreateMaterialDialogComponent, data, { width: '720px' });
    if (result.action === DialogAction.save) {
      this.contentTreeListStore.reload();
    }
  }

  async onCreateQuestion(node: GetContentTreeListQuery_Result) {
    if (!this.course) return;
    const data = new CreateQuestionDialogData(
      this.course.courseId,
      node.parentId,
      null,
      QuestionType.multipleChoice,
      '',
      '',
      '',
      '',
      []
    );
    const result = await this._dialog.show(CreateQuestionDialogComponent, data, { width: '720px' });
    if (result.action === DialogAction.save) {
      this.contentTreeListStore.reload();
    }
  }

  async onManageAutoMap(node: GetContentTreeListQuery_Result) {
    var baseCourses = await firstValueFrom(this.baseCourses$);
    await this._manageAutoMapOperation.perform(node.entityId, node.parentId, baseCourses, this.course?.subjectCode ?? -1, node.isAutoMapped);
    this.contentTreeListStore.reload();
  }

  onUpdateMaterials(node: GetContentTreeListQuery_Result) {
    console.log("Update materials nodes: ");
    console.log(node.children);
    this._contentTreeNodeService.updateMaterials(node.parentId, node.children?.map(x => x.entityId) ?? []);
  }

  onUpdateQuestions(node: GetContentTreeListQuery_Result) {
    console.log("Update questions nodes: ");
    console.log(node.children);
    this._contentTreeNodeService.updateQuestions(node.parentId, node.children?.map(x => x.entityId) ?? []);
  }

  async onAddQuestions() {
    if (!this.course) return;

    var result = await this._dialog.show(ConfirmationDialog, new ConfirmationDialogData(this.strings.addNewQuestions, this.strings.addNewQuestionsConfirmation))
    if (result.action != DialogAction.yes) return;

    var log = await firstValueFrom(this._api.send(new AddNewQuestionsCommand(this.course.courseId)));

    if (log.every(y => y.numberOfQuestions == 0)) {
      await this._dialog.show(InformationDialog, new InformationDialogData(this.strings.addNewQuestions, this.strings.noQuestionsToAdd));
      return;
    }
    await this._dialog.show(InformationDialog, new InformationDialogData(this.strings.addNewQuestions, log.filter(y => y.numberOfQuestions > 0).map(y => `${y.nodeName}: ${y.numberOfQuestions} questions added`).join("\n")));
    location.reload();
  }
  
  async publishCourse(course: GetCoursesListQuery_Result){
    if(!course) return;    

    var result = await this._dialog.show(ConfirmationDialog, new ConfirmationDialogData(this.strings.publish, this.strings.publishConfirmation, this.strings.publish, this.strings.cancel));
    if (result.action != DialogAction.yes) return;

    var data = await firstValueFrom(this._api.send(new PublishCourseCommand(course.courseId)));
    console.log(`version: ${data.version}`);

    await this._dialog.show(InformationDialog, new InformationDialogData(this.strings.publish, this.strings.publishResult));
  }
}

