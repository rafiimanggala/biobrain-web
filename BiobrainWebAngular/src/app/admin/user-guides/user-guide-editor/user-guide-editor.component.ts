import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { SummernoteService } from '../../services/summernote/summernote.service';
import { UserGuideNodeViewModel } from '../view-models/user-guide-node.view-model';
import { EditUserGuidesContentOperation } from '../../operations/user-guides/edit-user-guides-content.operation';
import { GetUserGuideContentOperation } from '../../operations/user-guides/get-user-guide-content.operation';
import { UserGuideContentViewModel } from '../view-models/user-guide-content.view-model';
import { StringsService } from 'src/app/share/strings.service';
import { EmbedVideoService } from 'ngx-embed-video';
import { BaseComponent } from 'src/app/core/app/base.component';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';

@Component({
  selector: 'app-user-guide-editor',
  templateUrl: './user-guide-editor.component.html',
  styleUrls: ['./user-guide-editor.component.scss'],
  providers: [SummernoteService, EditUserGuidesContentOperation, GetUserGuideContentOperation],
  
})
export class UserGuideEditorComponent extends BaseComponent implements OnChanges{

  public content!: UserGuideContentViewModel;
  public iframe_html: any;

  @Input()
  selectedItem!: UserGuideNodeViewModel;

  constructor(
    public summernoteService: SummernoteService,
    public strings: StringsService,
    private readonly getUserGuideContentOperation: GetUserGuideContentOperation,
    private readonly editUserGuidesContentOperation: EditUserGuidesContentOperation,
    private readonly embedService: EmbedVideoService,
    appEvents: AppEventProvider
  ){
    super(appEvents);
  }

  async ngOnChanges(changes: SimpleChanges) {
    if (changes.selectedItem){
      await this.getDataInternal();
    }
  }

  initPlayer() {
    try {
      if (this.content && this.content.videoUrl && this.content.videoUrl.length > 0) {
        this.iframe_html = this.embedService.embed(this.content.videoUrl);
      }
      else{
        this.iframe_html = null;
      }
    }
    catch (e) {
      this.error(this.strings.videoUrlError);
    }
  }

  private async getDataInternal(){
    if(!this.selectedItem) return;

    var result = await this.getUserGuideContentOperation.perform(this.selectedItem.nodeId);
    if(!result.isSuccess()) return;

    this.content = result.data;
    this.initPlayer();
  }

  async saveContent(){
    await this.editUserGuidesContentOperation.perform(this.content);
  }
 }

