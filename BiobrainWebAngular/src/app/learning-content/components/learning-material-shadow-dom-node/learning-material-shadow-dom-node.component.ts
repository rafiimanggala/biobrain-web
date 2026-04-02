import { Component, ElementRef, Input, OnDestroy, Renderer2, ViewEncapsulation } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Observable, ReplaySubject } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { ThemeService } from '../../../core/app/theme.service';
import { assertHasValue } from '../../../share/helpers/assert-has-value';
import { assertUnreachableStatement } from '../../../share/helpers/assert-unreachable-statement';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { isNullOrWhitespace } from '../../../share/helpers/is-null-or-white-space';
import { removeHtmlTag } from '../../../share/helpers/remove-html-tag';
import { rewriteImageUrls } from '../../../share/helpers/rewrite-image-urls';
import { RequestHandlingService } from '../../../share/services/request-parse.service';
import { ContentStyles } from '../../../share/values/content-styles';

import { PluggableScriptComponent } from './pluggable-script.component';

declare function preventImagesClick(): void;

@Component({
  selector: 'app-learning-material-shadow-dom-node',
  templateUrl: './learning-material-shadow-dom-node.component.html',
  styleUrls: ['./learning-material-shadow-dom-node.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom
})
export class LearningMaterialShadowDomNodeComponent extends PluggableScriptComponent implements OnDestroy {
  content$ = new ReplaySubject<LearningMaterialContent>(1);
  formattedHtml$: Observable<SafeHtml>;

  private readonly _handler: EventListener;
  private readonly _shadowRoot: ShadowRoot;

  private readonly _linksRegEx = /(<a .*?)(href=["'](.*?)["'])(.*?>.*?\n*?.*?<\/a>)/g;
  private readonly _linksReplaceExpression = "$1onclick=\"sendRequest(event, '$3')\"$4";
  private readonly _getRequestScriptId = 'sendRequestScript';
  private readonly _preventClickScriptId = 'preventClickScript';
  private readonly _preventTapScriptId = 'preventTapScript';

  constructor(
    private readonly _element: ElementRef<HTMLDivElement>,
    private readonly _requestHandlingService: RequestHandlingService,
    private readonly _sanitizer: DomSanitizer,
    private readonly _themeService: ThemeService,
    renderer: Renderer2
  ) {
    super(renderer);

    assertHasValue(this._element.nativeElement.shadowRoot);
    this._shadowRoot = this._element.nativeElement.shadowRoot;

    this.formattedHtml$ = this.content$.pipe(
      switchMap(x => this._toHtmlContent(x)),
      map(x => this._sanitizer.bypassSecurityTrustHtml(x))
    );

    this._handler = ((e: CustomEvent<{ev: any, request: string} | null | undefined>) => {
      const value = e.detail;
      assertHasValue(value);
      this._requestHandlingService.handle(value);
    }) as EventListener;

    this._shadowRoot.addEventListener('request', this._handler, false);
    this._includeRequestSenderScript();
  }

  @Input() set learningContent(value: LearningMaterialContent | null | undefined) {
    const defaultContent: LearningMaterialContent = {
      contentType: ContentTypes.unknown,
      text: ''
    };

    this.content$.next(value ?? defaultContent);
  }

  ngOnDestroy(): void {
    this._shadowRoot.removeEventListener('request', this._handler, false);
    this._shadowRoot.removeEventListener('swichBookmark', this._handler, false);
  }

  getRootNode(): DocumentFragment {
    return this._shadowRoot;
  }

  onInnerHtmlRendered() {
    // renderMathInElement(this._element.nativeElement);
    // preventImagesClick();
  }

  private _includeRequestSenderScript(): void {
    this.includeScript(this._getRequestScriptId, "function sendRequest(ev, request){const event = new CustomEvent('request', {detail: {ev,request}, bubbles: true}); ev.target.dispatchEvent(event);}");
    this.includeScript(this._preventClickScriptId, "document.querySelector('app-learning-material-shadow-dom-node').addEventListener('contextmenu', e => { e.preventDefault(); event.stopPropagation(); return false;});");
    this.includeScript(this._preventTapScriptId, "document.body.style.webkitTouchCallout='none';");
    
  }

  private async _toHtmlContent(content: LearningMaterialContent): Promise<string> {
    const colors = await firstValueFrom(this._themeService.colors$);
    const contentText = content.text;

    switch (content.contentType) {
      case ContentTypes.unknown:
        return '';

      case ContentTypes.pages: {
        const materialText = this._bindLinksToRequestSender(contentText);
        return rewriteImageUrls(`<html>${ContentStyles.getMaterialStyles(colors)}<section><article>${materialText}</article></section></html>`);
      }

      case ContentTypes.glossaryDefinition: {
        const definitionWithTunedLinks = this._bindLinksToRequestSender(removeHtmlTag(contentText));
        return rewriteImageUrls(`${ContentStyles.getMaterialStyles(colors)}${definitionWithTunedLinks}`);
      }

      default:
        assertUnreachableStatement(content.contentType);
    }
  }

  private _bindLinksToRequestSender(html: string): string {
    if (isNullOrWhitespace(html)) {
      return '';
    }

    // Replace href to onclick sendRequest
    return html.replace(this._linksRegEx, this._linksReplaceExpression);
  }
}

export interface LearningMaterialContent {
  text: string;
  contentType: ContentTypes;
}

export enum ContentTypes {
  unknown,
  pages,
  glossaryDefinition
}

