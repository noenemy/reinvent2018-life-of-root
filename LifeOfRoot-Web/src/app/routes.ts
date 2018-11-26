import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RankingComponent } from './ranking/ranking.component';
import { AboutusComponent } from './aboutus/aboutus.component';
import { GameComponent } from './game/game.component';
import { DebugComponent } from './debug/debug.component';
import { PlaydemoComponent } from './playdemo/playdemo.component';
import { TrailerComponent } from './trailer/trailer.component';

export const appRoutes: Routes = [
    { path: 'home', component: HomeComponent },
    { path: 'leaderboard', component: RankingComponent },
    { path: 'game', component: GameComponent },
    { path: 'playdemo', component: PlaydemoComponent },
    { path: 'debug', component: DebugComponent },
    { path: 'trailer', component: TrailerComponent },
    { path: '**', redirectTo: 'home', pathMatch: 'full' }
];
