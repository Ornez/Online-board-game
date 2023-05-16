import { Injectable } from "@angular/core";
import { BaseState, StateService } from "@shared/common/state.service";
import { SettingsData } from "@core/models/settings-data";
import { finalize, Observable } from "rxjs";
import { HttpSettingsService } from "@core/services/http-settings.service";
import { Language } from "@core/models/language";
import { TranslateService } from "@ngx-translate/core";
import { AVAILABLE_LANGUAGES } from "@core/const/available-languages.const";

interface SettingsState {
  settings: BaseState<SettingsData>
}

const initialState: SettingsState = {
  settings: {data: {soundVolume: 100, musicVolume: 100, languageCode: 'pl'}}
}

@Injectable({
  providedIn: "root"
})
export class SettingsStoreService extends StateService<SettingsState> {

  readonly settingsMusicVolume$: Observable<number> = this.select(state => state.settings.data.musicVolume);
  readonly settingsSoundVolume$: Observable<number> = this.select(state => state.settings.data.soundVolume);
  readonly settingsLoading$: Observable<boolean> = this.select(state => state.settings.loading);

  constructor(private http: HttpSettingsService,
              private translate: TranslateService) {
    super(initialState);
    this.initTranslations();
  }

  public getSettings(): void {
    this.setState({settings: {loading: true}} as Partial<SettingsState>)
    this.http.getSettings()
      .pipe(finalize(() => this.setState({settings: {loading: false}} as Partial<SettingsState>)))
      .subscribe( (settings: SettingsData) => {
        this.setState({settings: {data: settings}} as Partial<SettingsState>);
        this.setLanguage(settings.languageCode);
      });
  }

  public saveSettings(): void {
    this.setState({settings: {loading: true}} as Partial<SettingsState>)
    this.http.saveSettings(this.state.settings.data)
      .pipe(finalize(() => this.setState({settings: {loading: false}} as Partial<SettingsState>)))
      .subscribe();
  }

  public setMusicVolume(musicVolume: number): void {
    this.setState({settings: {data: {...this.state.settings.data, musicVolume}}} as Partial<SettingsState>);
  }

  public setSoundVolume(soundVolume: number): void {
    this.setState({settings: {data: {...this.state.settings.data, soundVolume}}} as Partial<SettingsState>);
  }

  public setLanguage(languageCode: string): void {
    this.translate.use(languageCode);
    this.setState({settings: {data: {...this.state.settings.data, languageCode}}} as Partial<SettingsState>);
  }

  public getAvailableLanguages(): Language[] {
    return AVAILABLE_LANGUAGES;
  }

  public getCurrentLanguage(): string {
    return this.translate.currentLang;
  }

  private initTranslations(): void {
    const availableLanguages: string[] = AVAILABLE_LANGUAGES.map( language => language.code);
    const browserLanguage: string = this.translate.getBrowserLang();

    this.setLanguage(availableLanguages.includes(browserLanguage) ? browserLanguage : 'en')

    this.translate.addLangs(availableLanguages);
    this.translate.use(this.state.settings.data.languageCode);
  }
}
