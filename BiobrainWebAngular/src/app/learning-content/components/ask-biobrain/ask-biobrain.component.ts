import { Component, ElementRef, Input, OnDestroy, ViewChild } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Subscription } from 'rxjs';

import { Api } from '../../../api/api.service';
import {
  AskBiobrainCommand,
  AskBiobrainCommand_Result,
  AskBiobrainMessage,
} from '../../../api/ai/ask-biobrain.command';
import { ThemeService } from '../../../core/app/theme.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';

interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
  renderedHtml?: SafeHtml;
}

@Component({
  selector: 'app-ask-biobrain',
  templateUrl: './ask-biobrain.component.html',
  styleUrls: ['./ask-biobrain.component.scss'],
})
export class AskBiobrainComponent implements OnDestroy {
  @Input() courseId = '';
  @Input() contentTreeNodeId = '';

  @ViewChild('messageList') messageListEl!: ElementRef;

  isOpen = false;
  isLoading = false;
  questionText = '';
  messages: ChatMessage[] = [];
  errorMessage = '';
  headerColor = '#004876';
  private _colorSub: Subscription;

  constructor(
    private readonly _api: Api,
    private readonly _sanitizer: DomSanitizer,
    private readonly _themeService: ThemeService
  ) {
    this._colorSub = this._themeService.colors$.subscribe(colors => {
      this.headerColor = colors?.primary || '#004876';
    });
  }

  ngOnDestroy(): void {
    this._colorSub?.unsubscribe();
  }

  togglePanel(): void {
    this.isOpen = !this.isOpen;
  }

  closePanel(): void {
    this.isOpen = false;
  }

  clearConversation(): void {
    this.messages = [];
    this.errorMessage = '';
  }

  async sendQuestion(): Promise<void> {
    const question = this.questionText.trim();
    if (!question || this.isLoading) {
      return;
    }

    this.errorMessage = '';
    this.questionText = '';

    const userMessage: ChatMessage = { role: 'user', content: question };
    this.messages = [...this.messages, userMessage];
    this._scrollToLatestMessage();

    this.isLoading = true;

    try {
      const history: AskBiobrainMessage[] = this.messages
        .slice(0, -1)
        .map(m => ({ role: m.role, content: m.content }));

      const command = new AskBiobrainCommand(
        this.courseId,
        this.contentTreeNodeId,
        question,
        history
      );

      const result: AskBiobrainCommand_Result = await firstValueFrom(
        this._api.send(command)
      );

      const assistantMessage: ChatMessage = {
        role: 'assistant',
        content: result.answer,
        renderedHtml: this._sanitizer.bypassSecurityTrustHtml(
          this._renderMarkdown(result.answer)
        ),
      };
      this.messages = [...this.messages, assistantMessage];
    } catch (err) {
      this.errorMessage = 'Failed to get a response. Please try again.';
    } finally {
      this.isLoading = false;
      this._scrollToLatestMessage();
    }
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendQuestion();
    }
  }

  private _renderMarkdown(text: string): string {
    if (!text) return '';
    let html = text
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;');

    // Code blocks
    html = html.replace(/```([\s\S]*?)```/g, '<pre><code>$1</code></pre>');
    html = html.replace(/`([^`\n]+)`/g, '<code>$1</code>');
    // Headers → bold styled text (no raw ## shown)
    html = html.replace(/^###\s+(.+)$/gm, '<p class="chat-heading">$1</p>');
    html = html.replace(/^##\s+(.+)$/gm, '<p class="chat-heading">$1</p>');
    html = html.replace(/^#\s+(.+)$/gm, '<p class="chat-heading">$1</p>');
    // Bold **text**
    html = html.replace(/\*\*([^\*]+)\*\*/g, '<strong>$1</strong>');
    // Italic *text* (avoid bullets)
    html = html.replace(/(^|[^\*])\*([^\*\n]+)\*/g, '$1<em>$2</em>');

    // "Memory Tip:" → bold heading
    html = html.replace(/^(Memory Tip\s*:)/gm, '<strong>$1</strong>');

    // Strip any remaining standalone # or * at line starts
    html = html.replace(/^#{1,4}\s*/gm, '');

    const lines = html.split('\n');
    const out: string[] = [];
    let inList = false;
    let inOrdered = false;
    for (const line of lines) {
      const trimmed = line.trim();
      const bulletMatch = trimmed.match(/^[-]\s+(.+)$/);
      const orderedMatch = trimmed.match(/^(\d+)\.\s+(.+)$/);
      if (bulletMatch) {
        if (inOrdered) { out.push('</ol>'); inOrdered = false; }
        if (!inList) { out.push('<ul>'); inList = true; }
        out.push('<li>' + bulletMatch[1] + '</li>');
      } else if (orderedMatch) {
        if (inList) { out.push('</ul>'); inList = false; }
        if (!inOrdered) { out.push('<ol>'); inOrdered = true; }
        out.push('<li>' + orderedMatch[2] + '</li>');
      } else {
        if (inList) { out.push('</ul>'); inList = false; }
        if (inOrdered) { out.push('</ol>'); inOrdered = false; }
        if (trimmed.length === 0) {
          out.push('');
        } else if (/^<(p class="chat-heading"|pre|ul|ol|li)/.test(trimmed)) {
          out.push(line);
        } else {
          out.push('<p>' + line + '</p>');
        }
      }
    }
    if (inList) out.push('</ul>');
    if (inOrdered) out.push('</ol>');
    return out.join('\n');
  }

  private _scrollToLatestMessage(): void {
    setTimeout(() => {
      if (this.messageListEl) {
        const el = this.messageListEl.nativeElement;
        const messageElements = el.querySelectorAll('.message');
        if (messageElements.length > 0) {
          const lastMessage = messageElements[messageElements.length - 1];
          lastMessage.scrollIntoView({ behavior: 'smooth', block: 'start' });
        } else {
          el.scrollTop = el.scrollHeight;
        }
      }
    }, 50);
  }
}
