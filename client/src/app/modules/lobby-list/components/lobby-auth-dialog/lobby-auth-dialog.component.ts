import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { filter, takeUntil } from "rxjs";
import { BaseComponent } from "@shared/common/base-component.abstract";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { LobbyListStoreService } from "@lobby-list/store/lobby-list-store.service";
import { Lobby } from "@lobby-list/models/lobby";
import { Router } from "@angular/router";
import {LobbyStoreService} from "@lobby/store/lobby-store.service";

@Component({
  selector: 'lobby-auth-dialog',
  templateUrl: './lobby-auth-dialog.component.html',
  styleUrls: ['./lobby-auth-dialog.component.scss'],
})
export class LobbyAuthDialogComponent extends BaseComponent implements OnInit {

  form: FormGroup;
  isPasswordVisible: boolean = false;

  constructor(public lobbyListStore: LobbyListStoreService,
              private lobbyStore: LobbyStoreService,
              private dialogRef: MatDialogRef<LobbyAuthDialogComponent>,
              private router: Router,
              @Inject(MAT_DIALOG_DATA) public lobby: Lobby) {
    super();
  }

  public ngOnInit(): void {
    this.createForm();
    this.initDataHandle();
  }

  public onJoin(): void {
    this.lobbyListStore.joinTheLobby({lobbyName: this.lobby.lobbyName, password: this.form.value.password});
  }

  public onTogglePasswordVisibility(): void {
    this.isPasswordVisible = !this.isPasswordVisible;
  }

  private createForm(): void {
    this.form = new FormGroup({
      password: new FormControl(null, [Validators.required]),
    })
  }

  private initDataHandle(): void {
    this.lobbyListStore.joinTheLobbySuccess$
      .pipe(
        takeUntil(this.destroyed$),
        filter(Boolean))
      .subscribe(() => {
        this.dialogRef.close(true);
        this.lobbyStore.setLobbyId(this.lobby.id)
        this.router.navigate(['', 'lobby']);
      });
  }
}
