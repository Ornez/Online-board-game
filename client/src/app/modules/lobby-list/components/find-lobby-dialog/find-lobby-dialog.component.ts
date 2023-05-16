import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { BaseComponent } from "@shared/common/base-component.abstract";

@Component({
  selector: 'find-lobby-dialog',
  templateUrl: './find-lobby-dialog.component.html',
  styleUrls: ['./find-lobby-dialog.component.scss'],
})
export class FindLobbyDialogComponent extends BaseComponent implements OnInit {

  form: FormGroup;

  constructor() {
    super();
  }

  public ngOnInit(): void {
    this.createForm();
  }

  private createForm(): void {
    this.form = new FormGroup({
      lobbyCode: new FormControl(null, [Validators.required]),
    })
  }

}
