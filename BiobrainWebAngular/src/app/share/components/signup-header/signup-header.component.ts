import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-signup-header',
  templateUrl: './signup-header.component.html',
  styleUrls: ['./signup-header.component.scss'],
})
export class SignupHeaderComponent {
  @Input() text: string = '';  
  @Input() subText: string | null = null;  
}
