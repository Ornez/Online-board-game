import {Component, Input} from '@angular/core';
import {ChatMessage} from "@chat/models/chat-message";

@Component({
  selector: 'chat-message',
  templateUrl: './chat-message.component.html',
  styleUrls: ['./chat-message.component.scss'],
})
export class ChatMessageComponent {

  @Input() message: ChatMessage;
}
