import { NestedTreeControl } from '@angular/cdk/tree';
import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { Subscription } from 'rxjs';
import { filter, map, tap } from 'rxjs/operators';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';

import { GetContentTreeListQuery_Result } from '../../../api/content/get-content-tree-list.query';

@Component({
  selector: 'app-content-mapper-tree',
  templateUrl: './content-mapper-tree.component.html',
  styleUrls: ['./content-mapper-tree.component.scss']
})
export class ContentMapperTreeComponent implements OnInit, OnDestroy {

  @Output() add = new EventEmitter<GetContentTreeListQuery_Result | null>();
  @Output() copy = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() edit = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() moveUp = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() moveDown = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() attachMaterials = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() attachQuestions = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() createMaterial = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() createQuestion = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() updateMaterials = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() updateQuestions = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() manageAutoMap = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() excludeQuestion = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() includeQuestion = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() availableInDemo = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() quizSettings = new EventEmitter<GetContentTreeListQuery_Result>();
  @Output() manageQuiz = new EventEmitter<GetContentTreeListQuery_Result>();

  subscriptions: Subscription[] = [];

  nestedTreeControl: NestedTreeControl<GetContentTreeListQuery_Result>;
  nestedDataSource: MatTreeNestedDataSource<GetContentTreeListQuery_Result>;
  expandedNodes: GetContentTreeListQuery_Result[] = [];
  selectedNodeId: string | null = null;

  selectNode(node: GetContentTreeListQuery_Result): void {
    this.selectedNodeId = node.entityId;
  }

  isSelected(node: GetContentTreeListQuery_Result): boolean {
    return this.selectedNodeId === node.entityId;
  }

  getFolderIcon(node: GetContentTreeListQuery_Result): string {
    if (node.isMaterialsFolder) {
      return (node.children?.length ?? 0) > 0 ? 'menu_book' : 'book';
    }
    if (node.isQuestionsFolder) {
      if ((node.children?.length ?? 0) === 0) return 'help_outline';
      return node.isAutoMapped ? 'auto_awesome' : 'quiz';
    }
    return '';
  }

  getFolderIconColor(node: GetContentTreeListQuery_Result): string {
    const count = node.children?.length ?? 0;
    if (count === 0) return 'muted';
    if (node.isMaterialsFolder) return 'materials';
    if (node.isQuestionsFolder) return node.isAutoMapped ? 'auto' : 'manual';
    return '';
  }

  constructor(
    public readonly strings: StringsService,
    private readonly _loaderService: LoaderService,
    private readonly _dialog: Dialog,
  ) {
    this.nestedTreeControl = new NestedTreeControl<GetContentTreeListQuery_Result>(this.getChildren.bind(this));
    this.nestedDataSource = new MatTreeNestedDataSource<GetContentTreeListQuery_Result>();
  }
  ngOnDestroy(): void {
    this.subscriptions.forEach(x => x.unsubscribe());
  }

  ngOnInit(): void {
  }

  public initTree(tree: GetContentTreeListQuery_Result[]): void {
    // this.nestedDataSource.data = null;
    this.saveExpandedNodes();
    this.nestedDataSource.data = [];
    this.nestedDataSource.data = tree;
    this.restoreExpandedNodes();
  }

  expandable(node: GetContentTreeListQuery_Result): boolean {
    return node.children?.length > 0 ?? false;
  }

  hasNestedChild = (_: number, nodeData: GetContentTreeListQuery_Result): boolean =>
    nodeData.isCanAddChildren || nodeData.isMaterialsFolder || nodeData.isQuestionsFolder || this.expandable(nodeData);

  getChildren(node: GetContentTreeListQuery_Result): GetContentTreeListQuery_Result[] {
    return node.children;
  }

  getOrderForNewArea(): number {
    return this.nestedDataSource.data.length + 1;
  }

  public getNode(id: string): GetContentTreeListQuery_Result | null {
    for (let i = 0; i < this.nestedDataSource.data.length; i++) {
      const root = this.nestedDataSource.data[i];
      if (root.entityId == id) {
        return root;
      }

      const descendants = this.nestedTreeControl.getDescendants(root);
      const node = descendants.find(n => n.entityId === id);
      if (node) {
        return node;
      }
    }
    return null;
  }

  public getUpperSibling(node: GetContentTreeListQuery_Result): GetContentTreeListQuery_Result | null {
    const siblings = this.getSiblings(node);
    const uppers = siblings.filter(x => x.order < node.order).sort((a, b) => b.order - a.order);
    return uppers.length > 0 ? uppers[0] : null;
  }

  public getLowerSibling(node: GetContentTreeListQuery_Result): GetContentTreeListQuery_Result | null {
    const siblings = this.getSiblings(node);
    const uppers = siblings.filter(x => x.order > node.order).sort((a, b) => a.order - b.order);
    return uppers.length > 0 ? uppers[0] : null;
  }

  private getSiblings(node: GetContentTreeListQuery_Result): GetContentTreeListQuery_Result[] {
    let siblings: GetContentTreeListQuery_Result[] = [];
    if (!node.parentId) {
      siblings = this.nestedDataSource.data;
    } else {
      const parent = this.getNode(node.parentId);
      // ToDo: redo
      if (!parent) {
        throw new Error("Can't find parent");
      }
      siblings = parent.children;
    }
    return siblings;
  }

  onAdd(node: GetContentTreeListQuery_Result | null) {
    this.add.emit(node);
  }

  onCopy(node: GetContentTreeListQuery_Result) {
    this.copy.emit(node);
  }

  onEdit(node: GetContentTreeListQuery_Result) {
    this.edit.emit(node);
  }

  onMoveUp(node: GetContentTreeListQuery_Result) {
    this.moveUp.emit(node);
  }

  onMoveDown(node: GetContentTreeListQuery_Result) {
    this.moveDown.emit(node);
  }

  onAttachMaterials(node: GetContentTreeListQuery_Result) {
    this.attachMaterials.emit(node);
  }

  onAttachQuestions(node: GetContentTreeListQuery_Result) {
    this.attachQuestions.emit(node);
  }

  onCreateMaterial(node: GetContentTreeListQuery_Result) {
    this.createMaterial.emit(node);
  }

  onCreateQuestion(node: GetContentTreeListQuery_Result) {
    this.createQuestion.emit(node);
  }

  onInclude(node: GetContentTreeListQuery_Result) {
    this.includeQuestion.emit(node);
  }

  onExclude(node: GetContentTreeListQuery_Result) {
    this.excludeQuestion.emit(node);
  }

  onManageAutoMap(node: GetContentTreeListQuery_Result) {
    this.manageAutoMap.emit(node);
  }

  onAvailableInDemo(node: GetContentTreeListQuery_Result) {
    this.availableInDemo.emit(node);
  }

  onQuizSettings(node: GetContentTreeListQuery_Result) {
    this.quizSettings.emit(node);
  }

  onManageQuiz(node: GetContentTreeListQuery_Result) {
    this.manageQuiz.emit(node);
  }

  onContentMoveUp(node: GetContentTreeListQuery_Result) {
    const parent = this.getNode(node.parentId);
    if (!parent) {
      return;
    }
    const index = parent.children.indexOf(node);
    if (index < 1) {
      return;
    }
    parent.children.splice(index, 1);
    parent.children.splice(index - 1, 0, node);
    if (parent.isMaterialsFolder) {
      this.updateMaterials.emit(parent);
    }
    if (parent.isQuestionsFolder) {
      this.updateQuestions.emit(parent);
    }
  }

  onContentMoveDown(node: GetContentTreeListQuery_Result) {
    const parent = this.getNode(node.parentId);
    if (!parent) {
      return;
    }
    const index = parent.children.indexOf(node);
    if (index >= parent.children.length - 1) {
      return;
    }
    parent.children.splice(index, 1);
    parent.children.splice(index + 1, 0, node);
    if (parent.isMaterialsFolder) {
      this.updateMaterials.emit(parent);
    }
    if (parent.isQuestionsFolder) {
      this.updateQuestions.emit(parent);
    }
  }

  onContentDelete(node: GetContentTreeListQuery_Result) {
    const parent = this.getNode(node.parentId);
    if (!parent) {
      return;
    }

    console.log(`Delete MaterialId = ${node.entityId} NodeId = ${parent.parentId}`);

    let type: string = this.strings.node;
    if (parent.isMaterialsFolder) {
      type = this.strings.material;
    }
    if (parent.isQuestionsFolder) {
      type = this.strings.question;
    }

    this.subscriptions.push(
      this._dialog.observe(DeleteConfirmationDialog, new DeleteConfirmationDialogData(type, node.header)).pipe(
        map(_ => _.data),
        filter(x => x?.confirmed === true),
        tap(() => {
          try {
            if (!parent) {
              return;
            }
            const index = parent.children.indexOf(node);
            parent.children.splice(index, 1);
            if (parent.isMaterialsFolder) {
              this.updateMaterials.emit(parent);
            }
            if (parent.isQuestionsFolder) {
              this.updateQuestions.emit(parent);
            }
          } catch (e) {
            console.error(e);
          }
        })).subscribe()
    );
  }

  private saveExpandedNodes() {
    this.expandedNodes = [];
    this.getAllNodes().forEach(node => {
      if (this.nestedTreeControl.isExpanded(node)) {
        this.expandedNodes.push(node);
      }
    });
  }

  private getAllNodes(): GetContentTreeListQuery_Result[] {
    const nodes = [];
    for (let i = 0; i < this.nestedDataSource.data.length; i++) {
      const root = this.nestedDataSource.data[i];
      nodes.push(root);

      const descendants = this.nestedTreeControl.getDescendants(root);
      nodes.push(...descendants);
    }
    return nodes;
  }

  private restoreExpandedNodes() {
    const nodes = this.getAllNodes();
    this.expandedNodes.forEach(node => {
      const toExpand = nodes.find(n => n.entityId === node.entityId);
      if (toExpand) {
        this.nestedTreeControl.expand(toExpand);
      }
    });
  }
}
