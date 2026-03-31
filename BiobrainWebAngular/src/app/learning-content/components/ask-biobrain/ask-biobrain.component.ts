import { Component, ElementRef, Input, ViewChild } from '@angular/core';

import { Api } from '../../../api/api.service';
import {
  AskBiobrainCommand,
  AskBiobrainCommand_Result,
  AskBiobrainMessage,
} from '../../../api/ai/ask-biobrain.command';
import { firstValueFrom } from '../../../share/helpers/first-value-from';

interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

@Component({
  selector: 'app-ask-biobrain',
  templateUrl: './ask-biobrain.component.html',
  styleUrls: ['./ask-biobrain.component.scss'],
})
export class AskBiobrainComponent {
  @Input() courseId = '';
  @Input() contentTreeNodeId = '';

  @ViewChild('messageList') messageListEl!: ElementRef;

  isOpen = false;
  isLoading = false;
  questionText = '';
  messages: ChatMessage[] = [];
  errorMessage = '';

  constructor(private readonly _api: Api) {}

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
    this._scrollToBottom();

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
      };
      this.messages = [...this.messages, assistantMessage];
    } catch (err) {
      this.errorMessage = 'Failed to get a response. Please try again.';
    } finally {
      this.isLoading = false;
      this._scrollToBottom();
    }
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendQuestion();
    }
  }

  private _scrollToBottom(): void {
    setTimeout(() => {
      if (this.messageListEl) {
        const el = this.messageListEl.nativeElement;
        el.scrollTop = el.scrollHeight;
      }
    }, 50);
  }
}
