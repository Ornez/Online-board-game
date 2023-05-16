import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {CoreModule} from "@core/core.module";
import {AuthModule} from "@auth/auth.module";
import {SharedModule} from "@shared/shared.module";
import {TranslateModule} from "@ngx-translate/core";
import {TRANSLATION_CONFIG} from "@core/config/translation.config";
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {IconsService} from "@core/services/icons.service";
import {LobbyListModule} from "@lobby-list/lobby-list.module";
import {SettingsStoreService} from "@core/store/settings-store.service";
import {TokenInterceptor} from "@auth/interceptors/token.interceptor";
import {AuthService} from "@auth/services/auth.service";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    SharedModule,
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    CoreModule,
    AuthModule,
    LobbyListModule,
    HttpClientModule,
    TranslateModule.forRoot(TRANSLATION_CONFIG)
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: TokenInterceptor,
    multi: true
  }],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private icons: IconsService,
              private settings: SettingsStoreService,
              private auth: AuthService) {
    if (this.auth.isLoggedIn())
      this.settings.getSettings();
  }
}
