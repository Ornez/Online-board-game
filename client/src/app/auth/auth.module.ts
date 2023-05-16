import {NgModule} from '@angular/core';
import {SharedModule} from "@shared/shared.module";
import { LoginDialogComponent } from "@auth/components/login-dialog/login-dialog.component";
import {RegisterDialogComponent} from "@auth/components/register-dialog/register-dialog.component";

@NgModule({
  declarations: [
    LoginDialogComponent,
    RegisterDialogComponent
  ],
  imports: [
    SharedModule
  ],
  exports: [
    LoginDialogComponent,
    RegisterDialogComponent
  ],
  providers: [],
})
export class AuthModule { }
