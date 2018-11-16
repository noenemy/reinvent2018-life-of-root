import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  stage: string;
  game_id: number;
  message1: string;
  message2: string;  
  constructor() { }

  ngOnInit() {
    this.stage = 'splash';
    this.game_id = 0;
  }

  goStage(stage: string) {
    this.stage = stage;
  }

  onGameCreated(game_id: number) {
    this.game_id = game_id;
    this.message1 = 'We will start the audition right away. Lights, Camera..';
    this.message2 = 'Let\'s begin with your profile picture.';   
    this.goStage('stage');

  }

  onStageCompleted(stage_id: number) {
    let isGameCompleted = false;

    switch (stage_id) {
      case 1:
        this.message1 = 'Out of all the feelings we get, my famous one would be happy face.';
        this.message2 = 'Let me see your happy face!';
        break;
      case 2:
        this.message1 = 'You have already notice this audition is for The Revengers.';
        this.message2 = 'Revenge comes from the anger. Show us the angry face!';
        break;
      case 3:
        this.message1 = 'When half of the population was gone, we all felt sad.';
        this.message2 = 'It is time to get your memories back. Show us the sad face.';
        break;
      case 4:
        this.message1 = 'Imagine all the people are back from gone! How much would you be surprised!?';
        this.message2 = 'Please show us the face expression if everyone really returns!';
        break;
      case 5:
        this.message1 = 'Ok. Well done.';
        this.message2 = 'Now time to see your result.';
        break;
      case 0:
        isGameCompleted = true;
        break;
    }

    if (isGameCompleted) {
      this.goStage('result');
    } else {
      this.goStage('stage');
    }
  }
}
