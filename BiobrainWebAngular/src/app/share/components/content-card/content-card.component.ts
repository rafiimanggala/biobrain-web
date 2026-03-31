import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { getScoreColor } from 'src/app/teachers/helpers/get-score-color';
import { StringsService } from '../../strings.service';
import { Colors } from '../../values/colors';
import { ContentCardAction } from './content-card-action';

@Component({
  selector: 'app-content-card',
  templateUrl: './content-card.component.html',
  styleUrls: ['./content-card.component.scss'],
})
export class ContentCardComponent implements OnInit {
  @Input() nameLines: string[] = [];
  @Input() score: number = -1;
  @Input() date: string = '';
  @Input() dueDate: string = '';
  @Input() dueDateColor: string = 'green';
  @Input() actions: ContentCardAction[] = [];
  @Input() endActions: boolean = false;
  @Output() action: EventEmitter<ContentCardAction> = new EventEmitter<ContentCardAction>();

  cardActions: ContentCardAction[] = [];
  
  constructor(
    public readonly strings: StringsService
  ){}

  ngOnInit(): void {
    this.cardActions = this.actions;
  }

  public onAction(action: ContentCardAction){
    if(!action) return;
    this.action.emit(action);
  }

  getScoreStyle(progress: number): { 'color': string } {
    return {
      color: getScoreColor(progress)
    };
  }
}
