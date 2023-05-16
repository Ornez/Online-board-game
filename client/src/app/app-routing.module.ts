import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePageComponent } from "@core/pages/home-page/home-page.component";

const routes: Routes = [
  {
    path: '',
    pathMatch: "full",
    component: HomePageComponent
  },
  {
    path: 'game',
    loadChildren: () => import('@game/game.module').then(m => m.GameModule)
  },
  {
    path: 'lobby',
    loadChildren: () => import('@lobby/lobby.module').then(m => m.LobbyModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
