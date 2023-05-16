import {Component} from '@angular/core';
import {Language} from "@core/models/language";
import {SettingsStoreService} from "@core/store/settings-store.service";

@Component({
  selector: 'settings-dialog',
  templateUrl: './settings-dialog.component.html',
  styleUrls: ['./settings-dialog.component.scss'],
})
export class SettingsDialogComponent {

  constructor(public settingsStore: SettingsStoreService) {}

  public getLanguages(): Language[] {
    return this.settingsStore.getAvailableLanguages();
  }

  public getCurrentLanguageCode(): string {
    return this.settingsStore.getCurrentLanguage();
  }

  public setLanguage(languageCode: string): void {
    this.settingsStore.setLanguage(languageCode);
  }

  public onChangeMusicVolume(musicVolume: number): void {
    this.settingsStore.setMusicVolume(musicVolume);
  }

  public onChangeSoundVolume(soundVolume: number): void {
    this.settingsStore.setSoundVolume(soundVolume);
  }
}
