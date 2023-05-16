import {Component, OnInit} from '@angular/core';
import {LobbyListStoreService} from "@lobby-list/store/lobby-list-store.service";
import {Toolbar} from "@core/enums/toolbar.enum";
import {AuthStoreService} from "@auth/store/auth-store.service";

@Component({
  selector: 'home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent implements OnInit {
  Toolbar = Toolbar;

  constructor(private lobbyListStore: LobbyListStoreService,
              private authStore: AuthStoreService) {}

  public ngOnInit(): void {
    this.authStore.generatePlayerToken();
  }

  public ngOnDestroy(): void {
    this.lobbyListStore.disconnectLobbiesHub();
  }
}
