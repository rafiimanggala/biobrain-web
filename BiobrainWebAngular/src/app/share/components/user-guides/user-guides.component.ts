import { TitleCasePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { SubTitleProviderService } from 'src/app/admin/services/sub-title-provider.service';
import { UserGuideNodeViewModel } from 'src/app/admin/user-guides/view-models/user-guide-node.view-model';
import { StringsService } from '../../strings.service';
import { DisposableSubscriberComponent } from '../disposable-subscriber.component';
import { GetUserGuidesContentTreeOperation } from 'src/app/admin/operations/user-guides/get-user-guides-content-tree.operation';
import { UserGuidesTreeComponent } from 'src/app/admin/user-guides/user-guides-tree/user-guides-tree.component';
import { SendQuestionOperation } from 'src/app/admin/operations/user-guides/send-question.operation';

@Component({
  selector: 'app-user-guides',
  templateUrl: './user-guides.component.html',
  styleUrls: ['./user-guides.component.scss'],
  providers: [
    GetUserGuidesContentTreeOperation
  ],
})
export class UserGuidesComponent extends DisposableSubscriberComponent implements OnInit { 
  
  contentTree: UserGuideNodeViewModel[] = [];
  selectedItem!: UserGuideNodeViewModel;

  constructor(
    public readonly strings: StringsService,
    private readonly _getUserGuidesContentTreeOperation: GetUserGuidesContentTreeOperation,
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

  onItemSelected(node: UserGuideNodeViewModel) {
    this.selectedItem = node;
  }
}

