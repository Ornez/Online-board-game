import {Component, OnInit} from '@angular/core';
import {ChatStoreService} from "../../store/chat-store.service";
import {FormControl, FormGroup} from "@angular/forms";
import {ObjectUtils} from "@shared/utils/object-utils";

@Component({
  selector: 'chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss'],
})
export class ChatComponent implements OnInit {

  form: FormGroup;

  constructor(public chatStore: ChatStoreService) {}

  public ngOnInit(): void {
    this.createForm();
  }

  public onSendMessage(): void {
    if(!ObjectUtils.isNil(this.form.value.message) && this.form.value.message !== '') {
      this.chatStore.sendMessage(this.form.value);
      this.form.reset();
    }
  }

  public createForm(): void {
    this.form = new FormGroup({
      message: new FormControl(null)
    });
  }
}
