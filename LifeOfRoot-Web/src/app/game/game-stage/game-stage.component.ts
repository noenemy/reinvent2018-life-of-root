import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { Observable, Subject, timer } from 'rxjs';
import { WebcamImage, WebcamInitError, WebcamUtil } from 'ngx-webcam';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AlertifyService } from '../../_services/alertify.service';
import { StageService } from 'src/app/_services/stage.service';
import { Stageinfo } from 'src/app/_models/stageinfo';
import { Stagescore } from 'src/app/_models/stagescore';
import { AngularFontAwesomeComponent } from 'angular-font-awesome';
import { StagelogService } from 'src/app/_services/stagelog.service';

@Component({
  selector: 'app-game-stage',
  templateUrl: './game-stage.component.html',
  styleUrls: ['./game-stage.component.css']
})
export class GameStageComponent implements OnInit {
  @Input() stage: string;
  @Input() game_id: number;
  @Input() stage_id: number;
  @Input() message1: string;
  @Input() message2: string;
  @Output() stageCompleted = new EventEmitter<number>();

  // stage info
  public difficulty: string = '';

  // score info
  public objects_score: number;
  public time_score: number;
  public clear_score: number;
  public stage_score: number;
  public total_score: number;
  public stage_completed: string;

  // object info
  public objects: string[];
  public total_object_count: number;
  public found_object_count: number;
  
  // game status info
  public gameStarted: boolean = false;

  // modal control flags
  public displayStageStartModal = 'none';
  public displayStageClearModal = 'none';
  public displayStageFailedModal = 'none';

  // ===============================================================================
  // Angular component constructor and destructor

  constructor(private http: HttpClient,
    private stageService: StageService,
    private stageLogService: StagelogService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    WebcamUtil.getAvailableVideoInputs()
      .then((mediaDevices: MediaDeviceInfo[]) => {
        this.multipleWebcamsAvailable = mediaDevices && mediaDevices.length > 1;
      });

    // Use enter key to get the current snapshot
    document.body.addEventListener('keypress', function(event) {
      if(event.keyCode === 13) {
          console.log('You pressed Enter key.');
          document.getElementById('buttonSnapshot').click();
      }
    });

    this.stage_id = 1;
    this.total_score = 0;
    this.getStageInfo();
  }

  ngOnDestroy() {
    this.clearTimer();
  }

  // ===============================================================================
  // Game play flow related functions
  onStageStart() {
    // start game
    this.displayStageStartModal = 'none';
    this.gameStarted = true;

    this.addStageLog();
    this.runTimer();
  }

  onStageEnd() {
    //
    
  }

  gameEnd() {
    console.log('game end!');
    this.stageCompleted.emit(this.stage_id);
  }

  // ===============================================================================
  // API call handlers
  public getStageInfo() {
    this.stageService.getStage(this.game_id, this.stage_id).subscribe((stageInfo: Stageinfo) => {
      // get stage objects
      this.objects = stageInfo.stage_objects;
      this.total_object_count = stageInfo.stage_objects.length;
      this.found_object_count = 0;

      // stage info
      this.seconds = stageInfo.stage_time;
      this.difficulty = stageInfo.stage_difficulty;
      this.total_object_count = 0;

      // show start modal dialog
      this.displayStageStartModal = 'block';
    })
  }

  public uploadPicture() {

    this.alertify.message('Now working on it...');

    const pictureInfo = {
      gameId: this.game_id,
      stageId: this.stage_id,
      base64Image: this.webcamImage.imageAsBase64
    };

    this.stageService.uploadPicture(pictureInfo).subscribe((stageScore: Stagescore) => {

      let element: HTMLElement = document.getElementById(stageScore.object_name) as HTMLElement;
      if (element) {
        element.style.backgroundColor = 'grey';
        this.total_score += stageScore.object_score;

        this.alertify.success('Great!');
      } else {
        this.alertify.warning('Not found!');
      }

    }, error => {
      this.alertify.error('Something wrong. Try again.');
    });
  }

  public addStageLog() {

    const stageLog = {
      gameId: this.game_id,
      stageId: this.stage_id
    };

    this.stageLogService.addStageLog(stageLog).subscribe(response => {
      console.log(response);
    }, error => {
      console.log('addStageLog failed.');
    });
  }
  
  public updateStageLog() {

    const stageLog = {
      gameId: this.game_id,
      stageId: this.stage_id,
      objects_score: this.objects_score,
      time_score: this.time_score,
      clear_score: this.clear_score,
      stage_score: this.objects_score + this.time_score,
      completed_yn : this.stage_completed
    };

    this.stageLogService.updateStageLog(stageLog).subscribe(response => {
      console.log(response);
    }, error => {
      console.log('updateStageLog failed.');
    });
  }

  // ===============================================================================
  // Timer handler
  private intervalId = 0;
  public seconds = 0.0;

  private clearTimer() {
    clearInterval(this.intervalId);
  }

  public runTimer() {
    this.clearTimer();
    this.intervalId = window.setInterval(() => {
      this.seconds -= 0.1;
      if (this.seconds ===0 || this.seconds < 0) {
        this.clearTimer();
        this.alertify.warning('Game Over!');
      }
    }, 100);
  }

  // ===============================================================================
  // Webcam handler

  // latest snapshot
  public webcamImage: WebcamImage = null;

  // webcam snapshot trigger
  private trigger: Subject<void> = new Subject<void>();

  // switch to next / previous / specific webcam; true/false: forward/backwards, string: deviceId
  private nextWebcam: Subject<boolean|string> = new Subject<boolean|string>();
  
  // toggle webcam on/off
  public showWebcam = true;
  public allowCameraSwitch = true;
  public multipleWebcamsAvailable = false;
  public deviceId: string;
  public videoOptions: MediaTrackConstraints = {
    // width: {ideal: 1024},
    // height: {ideal: 576}
  };

  public errors: WebcamInitError[] = [];

  public triggerSnapshot(): void {
    this.trigger.next();
    this.uploadPicture();
  }

  public toggleWebcam(): void {
    this.showWebcam = !this.showWebcam;
  }

  public handleInitError(error: WebcamInitError): void {
    this.errors.push(error);
  }

  public showNextWebcam(directionOrDeviceId: boolean|string): void {
    // true => move forward through devices
    // false => move backwards through devices
    // string => move to device with given deviceId
    this.nextWebcam.next(directionOrDeviceId);
  }

  public handleImage(webcamImage: WebcamImage): void {
    console.log('received webcam image', webcamImage);
    this.webcamImage = webcamImage;
  }

  public cameraWasSwitched(deviceId: string): void {
    console.log('active device: ' + deviceId);
    this.deviceId = deviceId;
  }

  public get triggerObservable(): Observable<void> {
    return this.trigger.asObservable();
  }

  public get nextWebcamObservable(): Observable<boolean|string> {
    return this.nextWebcam.asObservable();
  }
}

