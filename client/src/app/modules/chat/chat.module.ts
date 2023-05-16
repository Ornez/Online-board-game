import {NgModule} from '@angular/core';
import {SharedModule} from "@shared/shared.module";
import {ChatComponent} from "./components/chat/chat.component";
import {ChatDialogComponent} from "@chat/components/chat-dialog/chat-dialog.component";
import {ChatMessageComponent} from "@chat/components/chat-message/chat-message.component";

@NgModule({
  declarations: [
    ChatComponent,
    ChatDialogComponent,
    ChatMessageComponent
  ],
  imports: [
    SharedModule
  ],
  exports: [
    ChatComponent,
    ChatDialogComponent,
    ChatMessageComponent
  ],
  providers: [],
})
export class ChatModule { }
