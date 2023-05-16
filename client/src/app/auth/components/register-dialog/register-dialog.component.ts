import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import {AuthStoreService} from "@auth/store/auth-store.service";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { LoginDialogComponent } from "@auth/components/login-dialog/login-dialog.component";
import { filter, takeUntil } from "rxjs";
import { BaseComponent } from "@shared/common/base-component.abstract";

@Component({
  selector: 'register-dialog',
  templateUrl: './register-dialog.component.html',
  styleUrls: ['./register-dialog.component.scss'],
})
export class RegisterDialogComponent extends BaseComponent implements OnInit{

  form: FormGroup;
  isPasswordVisible: boolean = false;

  constructor(public authStore: AuthStoreService,
              private dialogRef: MatDialogRef<RegisterDialogComponent>,
              private dialog: MatDialog) {
    super();
  }

  public ngOnInit(): void {
    this.createForm();
    this.initDataHandle();
  }

  public togglePasswordVisibility(): void {
    this.isPasswordVisible = !this.isPasswordVisible;
  }

  public onRegister(): void {
    this.authStore.register(this.form.value)
  }

  public onLogin(): void {
    this.dialogRef.close(false);
    this.dialog.open(LoginDialogComponent);
  }

  private createForm(): void {
    this.form = new FormGroup({
      username: new FormControl(null, [Validators.required]),
      password: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required]),
    })
  }

  private initDataHandle(): void {
    this.authStore.registerSuccess$
      .pipe(
        takeUntil(this.destroyed$),
        filter(Boolean))
      .subscribe( () => this.dialogRef.close());
  }
}
