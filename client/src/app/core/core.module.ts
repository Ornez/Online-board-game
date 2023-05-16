import {NgModule} from '@angular/core';
import {HomePageComponent} from "@core/pages/home-page/home-page.component";
import {HomeNavComponent} from "@core/components/home-nav/home-nav.component";
import {SharedModule} from "@shared/shared.module";
import {SettingsDialogComponent} from "@core/components/settings-dialog/settings-dialog.component";
import {StatisticsDialogComponent} from "@core/components/statistics-dialog/statistics-dialog.component";
import {ProfileDialogComponent} from "@core/components/profile-dialog/profile-dialog.component";
import {HTTP_INTERCEPTORS} from "@angular/common/http";
import {HttpErrorInterceptor} from "@core/interceptors/http-error.interceptor";

@NgModule({
  declarations: [
    HomePageComponent,
    HomeNavComponent,
    SettingsDialogComponent,
    StatisticsDialogComponent,
    ProfileDialogComponent,
  ],
  imports: [
    SharedModule,
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: HttpErrorInterceptor,
    multi: true
  }],
})
export class CoreModule { }
