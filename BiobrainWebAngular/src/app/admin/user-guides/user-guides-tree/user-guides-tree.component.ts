import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { UserGuideNodeViewModel } from '../view-models/user-guide-node.view-model';
import { StringsService } from 'src/app/share/strings.service';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { BaseComponent } from 'src/app/core/app/base.component';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { NestedTreeService } from 'src/app/core/services/nested-tree/nested-tree.service';

@Component({
  selector: 'app-user-guides-tree',
  templateUrl: './user-guides-tree.component.html',
  styleUrls: ['./user-guides-tree.component.scss'],
  providers: [NestedTreeService],
})
export class UserGuidesTreeComponent extends BaseComponent implements OnChanges {

  @Output() add = new EventEmitter<UserGuideNodeViewModel | null>();
  @Output() edit = new EventEmitter<UserGuideNodeViewModel>();
  @Output() delete = new EventEmitter<UserGuideNodeViewModel>();
  @Output() reorder = new EventEmitter<UserGuideNodeViewModel>();
  @Output() select = new EventEmitter<UserGuideNodeViewModel>();

  @Input()
  contentTree: UserGuideNodeViewModel[] = [];
  @Input()
  selectedItem!: UserGuideNodeViewModel;
  @Input()
  isEditMode: boolean = true;

  public get nestedTreeControl(): NestedTreeControl<UserGuideNodeViewModel> { return this.treeService.nestedTreeControl; }
  public get nestedDataSource(): MatTreeNestedDataSource<UserGuideNodeViewModel> { return this.treeService.nestedDataSource; }

  constructor(
    public strings: StringsService,
    private readonly treeService: NestedTreeService<UserGuideNodeViewModel>,
    appEvents: AppEventProvider
  ) {
    super(appEvents);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.contentTree) {
      this.treeService.initTree(this.contentTree)
      if (this.selectedItem) {
        var newSelectedItem = this.treeService.getNode(this.selectedItem.nodeId);
        if (newSelectedItem)
          this.onItemSelected(newSelectedItem);
      }
    };
  }


  hasNestedChild = (_: number, nodeData: UserGuideNodeViewModel): boolean =>
    !nodeData.parentId;

  onAdd(node: UserGuideNodeViewModel | null) {
    this.add.emit(node);
  }

  onEdit(node: UserGuideNodeViewModel) {
    this.edit.emit(node);
  }

  onDelete(node: UserGuideNodeViewModel) {
    this.delete.emit(node);
  }

  onContentMoveUp(node: UserGuideNodeViewModel) {
    if (node.order <= 0) return;

    node.order--;
    this.reorder.emit(node);
  }

  onContentMoveDown(node: UserGuideNodeViewModel) {
    node.order++;
    this.reorder.emit(node);
  }

  onItemSelected(node: UserGuideNodeViewModel) {
    if (this.selectedItem)
      this.selectedItem.isSelected = false;
    node.isSelected = true;
    this.select.emit(node);
  }
}

