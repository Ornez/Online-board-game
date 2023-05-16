import {Component, Inject, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {AuthStoreService} from "@auth/store/auth-store.service";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {filter, takeUntil} from "rxjs";
import {BaseComponent} from "@shared/common/base-component.abstract";
import {RegisterDialogComponent} from "@auth/components/register-dialog/register-dialog.component";

@Component({
  selector: 'login-dialog',
  templateUrl: './login-dialog.component.html',
  styleUrls: ['./login-dialog.component.scss'],
})
export class LoginDialogComponent extends BaseComponent implements OnInit{

  form: FormGroup;
  isPasswordVisible: boolean = false;

  constructor(public authStore: AuthStoreService,
              private dialogRef: MatDialogRef<LoginDialogComponent>,
              private dialog: MatDialog,
              @Inject(MAT_DIALOG_DATA) private afterClose: () => void) {
    super();
  }

  public ngOnInit(): void {
    this.createForm();
    this.initDataHandle();
  }

  public onTogglePasswordVisibility(): void {
    this.isPasswordVisible = !this.isPasswordVisible;
  }

  public onLogin(): void {
    this.authStore.login(this.form.value);
  }

  public onRegister(): void {
    this.dialogRef.close(false);
    this.dialog.open(RegisterDialogComponent);
  }

  private createForm(): void {
    this.form = new FormGroup({
      username: new FormControl(null, [Validators.required]),
      password: new FormControl(null, [Validators.required]),
    })
  }

  private initDataHandle(): void {
    this.authStore.loginSuccess$
      .pipe(
        takeUntil(this.destroyed$),
        filter(Boolean))
      .subscribe( () => {
        this.dialogRef.close();
        this.afterClose();
      });
  }
}
