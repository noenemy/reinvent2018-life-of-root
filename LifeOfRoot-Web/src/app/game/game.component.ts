import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  stage: string;
  game_id: number; 
  constructor() { }

  ngOnInit() {
    this.stage = 'start';
    this.game_id = 0;
  }

  goStage(stage: string) {
    this.stage = stage;
  }

  onGameCreated(game_id: number) {
    this.game_id = game_id; 
    this.goStage('stage');

  }

  onStageCompleted(stage_id: number) {
    let isGameCompleted = false;

    if (stage_id == 0 || stage_id >= 2)
      isGameCompleted = true;

    if (isGameCompleted) {
      this.goStage('result');
    } else {
      this.goStage('stage');
    }
  }
}
