import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {LobbyListStoreService} from "@lobby-list/store/lobby-list-store.service";
import {filter, takeUntil} from "rxjs";
import {BaseComponent} from "@shared/common/base-component.abstract";
import {MatDialogRef} from "@angular/material/dialog";
import {Router} from "@angular/router";
import {ObjectUtils} from "@shared/utils/object-utils";
import {LobbyStoreService} from "@lobby/store/lobby-store.service";

@Component({
  selector: 'lobby-auth-dialog',
  templateUrl: './create-lobby-dialog.component.html',
  styleUrls: ['./create-lobby-dialog.component.scss'],
})
export class CreateLobbyDialogComponent extends BaseComponent implements OnInit{

  form: FormGroup;
  isPasswordVisible: boolean = false;

  constructor(public lobbyListStore: LobbyListStoreService,
              private lobbyStore: LobbyStoreService,
              private router: Router,
              private createLobbyDialogRef: MatDialogRef<CreateLobbyDialogComponent>) {
    super();
  }

  public ngOnInit(): void {
    this.createForm();
    this.initDataHandle();
  }

  public onCreate(): void {
    this.lobbyListStore.createLobby(this.form.value);
  }

  public onTogglePasswordVisibility(): void {
    this.isPasswordVisible = !this.isPasswordVisible;
  }

  private createForm(): void {
    this.form = new FormGroup({
      lobbyName: new FormControl(null, [Validators.required]),
      isPrivate: new FormControl(false, [Validators.required]),
      maxPlayersNumber: new FormControl(null, [Validators.required, Validators.max(5), Validators.min(1)]),
    })

    this.form.controls['isPrivate'].valueChanges
      .pipe(takeUntil(this.destroyed$))
      .subscribe((isPrivate: boolean) => isPrivate
        ? this.form.addControl('password', new FormControl(null, [Validators.required]))
        : this.form.removeControl('password'))
  }

  private initDataHandle(): void {
    this.lobbyListStore.createLobby$
      .pipe(
        filter(lobbyId => !ObjectUtils.isNil(lobbyId)),
        takeUntil(this.destroyed$))
      .subscribe( (lobbyId: number) => {
        this.createLobbyDialogRef.close(true);
        this.lobbyStore.setLobbyId(lobbyId);
        this.router.navigate(['', 'lobby']);
      });
  }
}
