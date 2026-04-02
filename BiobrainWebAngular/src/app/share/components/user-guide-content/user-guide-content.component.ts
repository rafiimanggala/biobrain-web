import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { UserGuideNodeViewModel } from 'src/app/admin/user-guides/view-models/user-guide-node.view-model';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { StringsService } from '../../strings.service';
import { GetUserGuideContentOperation } from 'src/app/admin/operations/user-guides/get-user-guide-content.operation';
import { UserGuideContentViewModel } from 'src/app/admin/user-guides/view-models/user-guide-content.view-model';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { EmbedVideoService } from 'ngx-embed-video';
import { SendQuestionOperation } from 'src/app/admin/operations/user-guides/send-question.operation';
import { rewriteImageUrls } from '../../helpers/rewrite-image-urls';

@Component({
  selector: 'app-user-guide-content',
  templateUrl: './user-guide-content.component.html',
  styleUrls: ['./user-guide-content.component.scss'],
})
export class UserGuideContentComponent extends BaseComponent implements OnChanges {
  public content!: UserGuideContentViewModel;
  public html!: SafeHtml;
  public iframe_html: any;
  private style: string = "<style>img{max-width: 100%}</style>";
  question: string = "";

  @Input()
  selectedItem!: UserGuideNodeViewModel;

  constructor(
    appEvents: AppEventProvider,
    private readonly getUserGuideContentOperation: GetUserGuideContentOperation,
    private readonly _sendQuestionOperation: SendQuestionOperation,
    private readonly sanitizer: DomSanitizer,
    private readonly embedService: EmbedVideoService,
    public strings: StringsService,
  ) {
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
        this.iframe_html = this.embedService.embed(this.content.videoUrl, { attr: { height: 360 } });
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
    this.html = this.sanitizer.bypassSecurityTrustHtml(rewriteImageUrls(this.style + this.content.htmlText));
    this.initPlayer();
  }

  async onSendQuestion(){
    if (!this.question) return;
    await this._sendQuestionOperation.perform(this.question);
    this.question = "";
  }
}

