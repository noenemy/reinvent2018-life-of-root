import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GameService } from '../_services/game.service';
import { Game } from '../_models/game';
import { GameResultService } from '../_services/gameresult.service';
import { GameResult } from '../_models/gameresult';
import { StageobjectService } from '../_services/stageobject.service';
import { Stageobject } from '../_models/stageobject';

@Component({
  selector: 'app-debug',
  templateUrl: './debug.component.html',
  styleUrls: ['./debug.component.css']
})
export class DebugComponent implements OnInit {
  gameResultColumns: string[];
  gameColumns: string[];
  castColumns: string[];
  stageObjectColumns: string[];
  gameResults: GameResult[];
  games: Game[];
  stageObjects: Stageobject[];

  constructor(private http: HttpClient, 
    private gameService: GameService,
    private stageObjectService: StageobjectService,
    private gameResultService: GameResultService) { }

  ngOnInit() {
    this.gameResultColumns = this.getGameResultColumns();
    this.gameColumns = this.getGameColumns();
    this.castColumns = this.getCastColumns();
    this.stageObjectColumns = this.getStageObjectColumns();
    this.getGameResults();
    this.getGames();
    this.getStageObjects();    
  }
  getGameResults() {
    this.gameResultService.getGameResults().subscribe((gameResults: GameResult[]) => {
      this.gameResults = gameResults;
    }, error => {
      console.log(error);
    });
  }

  getGames() {
    this.gameService.getGames().subscribe((games: Game[]) => {
      this.games = games;
    }, error => {
      console.log(error);
    });
  }

  getStageObjects() {
    this.stageObjectService.getStageObjects().subscribe((stageObjects: Stageobject[]) => {
      this.stageObjects = stageObjects;
    }, error => {
      console.log(error);
    });
  }

  getGameResultColumns(): string[] {
    return ['game_id', 'result_page_url', 'total_score', 'total_rank', 'cast_result', 'grade_result', 'gender_result', 'age_result'];
  }

  getGameColumns(): string[] {
    return ['game_id', 'name', 'share_yn', 'start_date', 'end_date'];
  }

  getCastColumns(): string[] {
    return ['cast_id', 'title', 'actor', 'gender', 'grade', 'file_loc', 'action_type'];
  }

  getStageObjectColumns(): string[] {
    return ['game_id', 'stage_id', 'object_name', 'object_score', 'found_yn', 'file_loc', 'log_date'];
  }
}