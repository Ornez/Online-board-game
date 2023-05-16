import {Injectable} from "@angular/core";
import { MatIconRegistry } from "@angular/material/icon";
import { DomSanitizer } from "@angular/platform-browser";

@Injectable({
  providedIn: 'root'
})
export class IconsService {

  icons: string[] = [
    'user-solid',
    'lock-solid',
    'lock-open-solid',
    'arrow-alt-left-solid',
    'times-square-solid',
    'shield-solid',
    'shield-check-solid',
    'crown-solid',
    'cog-solid',
    'chart-line-solid',
    'comments-alt-solid',
    'heart-solid',
    'heart-light',
    'swords-solid',
    'dice-solid',
    'undo-solid',
    'redo-solid',
    'location-solid',
    'arrow-to-bottom-solid',
    'info-square-solid',
    'walking-solid',
    'treasure-chest-solid',
    'hand-holding-medical-solid'
  ]

  constructor(private matIconRegistry: MatIconRegistry,
              private domSanitizer: DomSanitizer) {
    this.registerIcons();
  }

  private registerIcons(): void {
    this.icons.forEach((icon: string) => {
      this.matIconRegistry.addSvgIcon(
        icon,
        this.domSanitizer.bypassSecurityTrustResourceUrl(`/assets/icons/${icon}.svg`)
      );
    })
  }
}
