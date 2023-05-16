import { Component } from '@angular/core';

@Component({
  selector: 'navigation-button',
  templateUrl: './navigation-button.component.html',
  styleUrls: ['./navigation-button.component.scss']
})
export class NavigationButtonComponent {

  public hoverSound(): void {
    const audio = new Audio();
    audio.src = "/assets/sounds/nav-hover.mp3";
    audio.load();
    audio.muted = false;
    audio.volume = 0.2;
    audio.play();
  }
}
