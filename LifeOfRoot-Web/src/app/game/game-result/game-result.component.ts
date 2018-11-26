import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { AlertifyService } from '../../_services/alertify.service';
import { GameResult } from '../../_models/gameresult';
import { HttpClient } from '@angular/common/http';
import { GameResultService } from 'src/app/_services/gameresult.service';
import { RankingService } from 'src/app/_services/ranking.service';

@Component({
  selector: 'app-game-result',
  templateUrl: './game-result.component.html',
  styleUrls: ['./game-result.component.css']
})
export class GameResultComponent implements OnInit {
  @Input() game_id: number;
  @Output() go = new EventEmitter<string>();
  game_result: GameResult;
  game_ranking: number;
  constructor(private http: HttpClient,
    private gameResultService: GameResultService,
    private rankingService: RankingService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.getGameResult();
  }

  public getGameResult() {

    this.alertify.message('Now working on it...');

    this.gameResultService.getGameResult(this.game_id).subscribe((gameResult: GameResult) => {

      this.alertify.success('Successfully uploaded!');
      this.game_result = gameResult;
    }, error => {
      this.alertify.error(error);
    });

    this.rankingService.get(this.game_id).subscribe(response => {
      this.game_ranking = response;
    }, error => {
      this.alertify.error(error);
    });
  }

  gameStart() {
    this.go.emit('start');
  }
}
