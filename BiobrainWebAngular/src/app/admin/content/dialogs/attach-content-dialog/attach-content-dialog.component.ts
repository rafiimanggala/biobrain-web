import { NestedTreeControl } from '@angular/cdk/tree';
import { Component, Inject, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { StringsService } from 'src/app/share/strings.service';

import { GetContentTreeListQuery_Result } from '../../../../api/content/get-content-tree-list.query';
import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';

import { AttachContentDialogData } from './attach-content-dialog-data';

@Component({
  selector: 'app-content-tree-node-dialog',
  templateUrl: './attach-content-dialog.component.html',
  styleUrls: ['./attach-content-dialog.component.scss']
})
export class AttachContentDialog extends DialogComponent<AttachContentDialogData, AttachContentDialogData> implements OnInit {
  nestedTreeControl: NestedTreeControl<GetContentTreeListQuery_Result>;
  nestedDataSource: MatTreeNestedDataSource<GetContentTreeListQuery_Result>;

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: AttachContentDialogData,
  ) {
    super(data);

    this.nestedTreeControl = new NestedTreeControl<GetContentTreeListQuery_Result>(this.getChildren.bind(this));
    this.nestedDataSource = new MatTreeNestedDataSource<GetContentTreeListQuery_Result>();
  }

  ngOnInit(): void {

    this.nestedDataSource.data = this.data.entitiesToSelect;

    if (this.data.idsToAttach) {
      let toExpand = this.getAllNodes().find(x => x.entityId == this.data.idsToAttach[0]);
      if (toExpand)
        this.expandToNode(toExpand);
    }
    else {
      if (this.data.nodeSearchOpen) {
        let toExpand = this.getAllNodes().find(x => x.header == this.data.nodeSearchOpen);
        if (toExpand)
          this.expandToNode(toExpand);
      }
    }
  }

  expandable(node: GetContentTreeListQuery_Result): boolean {
    return node.children?.length > 0 ?? false;
  }

  hasNestedChild = (_: number, nodeData: GetContentTreeListQuery_Result) => nodeData.isCanAddChildren || nodeData.isMaterialsFolder || nodeData.isQuestionsFolder || this.expandable(nodeData);

  getChildren(node: GetContentTreeListQuery_Result): GetContentTreeListQuery_Result[] {
    return node.children;
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) {
      return;
    }
    if(this.data.isReplace)
      this.data.idsToAttach = [];
    this.data.idsToAttach.push(...this.getAllNodes().filter(x => x.isSelected && !this.data.idsToAttach.find(y => y == x.entityId)).map(x => x.entityId));
    this.close(DialogAction.save, this.data);
  }

  selectChildren(checked: boolean, node: GetContentTreeListQuery_Result) {
    node.children.forEach(x => x.isSelected = checked);
  }

  selectNode(checked: boolean, node: GetContentTreeListQuery_Result) {
    node.isSelected = checked;
  }

  expandToNode(node: GetContentTreeListQuery_Result) {
    var nodes = this.getAllNodes();
    this.nestedTreeControl.expand(node);
    do {
      var parent = nodes.find(x => x.entityId == node.parentId);
      if (!parent) break;
      this.nestedTreeControl.expand(parent);
      node = parent;
    }
    while (node.parentId);
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

}
