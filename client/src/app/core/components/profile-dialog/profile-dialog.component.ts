import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProfileStoreService } from "@core/store/profile-store.service";
import { takeUntil } from "rxjs";
import { BaseComponent } from "@shared/common/base-component.abstract";
import { ProfileData } from "@core/models/profile-data";
import { AuthStoreService } from "@auth/store/auth-store.service";
import {LobbyListStoreService} from "@lobby-list/store/lobby-list-store.service";

@Component({
  selector: 'profile-dialog',
  templateUrl: './profile-dialog.component.html',
  styleUrls: ['./profile-dialog.component.scss'],
})
export class ProfileDialogComponent extends BaseComponent implements OnInit, OnDestroy{
  profile: ProfileData;

  constructor(public profileStore: ProfileStoreService,
              private lobbyListStore: LobbyListStoreService,
              private authStore: AuthStoreService) {
    super();
  }

  public ngOnInit(): void {
    this.initDataHandle();
    this.profileStore.getProfile();
  }

  public onLogout(): void {
    this.authStore.logout();
    this.lobbyListStore.disconnectLobbiesHub();
    this.lobbyListStore.connectLobbiesHub();
  }

  private initDataHandle(): void {
    this.profileStore.profile$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((profile: ProfileData) => this.profile = profile)
  }

  public ngOnDestroy(): void {
    super.ngOnDestroy();
    this.profileStore.clearProfile();
  }
}
