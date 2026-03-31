import { Component, OnInit } from '@angular/core';

import { DisposableSubscriberComponent } from 'src/app/share/components/disposable-subscriber.component';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { StringsService } from 'src/app/share/strings.service';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';
import { TitleCasePipe } from '@angular/common';
import { GetUserGuidesContentTreeOperation } from '../../operations/user-guides/get-user-guides-content-tree.operation';
import { UserGuideNodeViewModel } from '../view-models/user-guide-node.view-model';
import { AddUserGuidesContentTreeNodeOperation } from '../../operations/user-guides/add-user-guides-content-tree-node.operation';
import { EditUserGuidesContentTreeNodeOperation } from '../../operations/user-guides/edit-user-guides-content-tree-node.operation';
import { DeleteUserGuidesContentTreeNodeOperation } from '../../operations/user-guides/delete-user-guides-content-tree-node.operation';
import { ReorderUserGuidesContentTreeNodeOperation } from '../../operations/user-guides/reorder-user-guides-content-tree-node.operation';

@Component({
  selector: 'app-user-guides-list',
  templateUrl: './user-guides-list.component.html',
  styleUrls: ['./user-guides-list.component.scss'],
  providers: [
    GetUserGuidesContentTreeOperation,
    AddUserGuidesContentTreeNodeOperation,
    EditUserGuidesContentTreeNodeOperation,
    DeleteUserGuidesContentTreeNodeOperation,
    ReorderUserGuidesContentTreeNodeOperation
  ],
})
export class UserGuidesListComponent extends DisposableSubscriberComponent implements OnInit {
  
  contentTree: UserGuideNodeViewModel[] = [];
  selectedItem!: UserGuideNodeViewModel;

  constructor(
    public readonly strings: StringsService,
    private readonly _getUserGuidesContentTreeOperation: GetUserGuidesContentTreeOperation,
    private readonly _addUserGuidesContentTreeNodeOperation: AddUserGuidesContentTreeNodeOperation,
    private readonly _editUserGuidesContentTreeNodeOperation: EditUserGuidesContentTreeNodeOperation,
    private readonly _reorderUserGuidesContentTreeNodeOperation: ReorderUserGuidesContentTreeNodeOperation,
    private readonly _routingService: RoutingService,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
  ) {
    super();
  }

  async ngOnInit() {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(`${this._titlecasePipe.transform(this.strings.userGuides)}`);
    }, 0);

   await this.getDataInternal();
  }

  private async getDataInternal(){
    var result = await this._getUserGuidesContentTreeOperation.perform();
    if(!result.isSuccess()) return;

    this.contentTree = result.data;
    if(!this.selectedItem){
      if(this.contentTree.some( _ => true)){
        if(this.contentTree[0].children.some(_ => true)){
          this.selectedItem = this.contentTree[0].children[0];
        }
      }
    }
  }
  
  async onAddNode(parent: UserGuideNodeViewModel|null){
    var result = await this._addUserGuidesContentTreeNodeOperation.perform(parent?.nodeId);
    if(!result.isSuccess) return;

    await this.getDataInternal();
  }
  
  async onEditNode(node: UserGuideNodeViewModel){
    var result = await this._editUserGuidesContentTreeNodeOperation.perform(node);
    if(!result.isSuccess) return;

    await this.getDataInternal();
  }
  
  async onReorderNode(node: UserGuideNodeViewModel){
    var result = await this._reorderUserGuidesContentTreeNodeOperation.perform(node);
    if(!result.isSuccess) return;

    await this.getDataInternal();
  }

  onItemSelected(node: UserGuideNodeViewModel) {
    this.selectedItem = node;
  }
 
}
