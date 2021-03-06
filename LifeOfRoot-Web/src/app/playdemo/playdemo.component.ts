import { Component, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { WebcamImage, WebcamInitError, WebcamUtil } from 'ngx-webcam';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Headers, Response, RequestOptions } from '@angular/http';
import { AlertifyService } from '../_services/alertify.service';
import { StageService } from '../_services/stage.service';

@Component({
  selector: 'app-playdemo',
  templateUrl: './playdemo.component.html',
  styleUrls: ['./playdemo.component.css']
})
export class PlaydemoComponent implements OnInit {
  // toggle webcam on/off
  public showWebcam = true;
  public allowCameraSwitch = true;
  public multipleWebcamsAvailable = false;
  public deviceId: string;
  public picture: any;
  public labels: any;
  public videoOptions: MediaTrackConstraints = {
    // width: {ideal: 1024},
    // height: {ideal: 576}
  };
  public errors: WebcamInitError[] = [];

  constructor(private http:HttpClient,
    private stageService: StageService,
    private alertify: AlertifyService) { }

  // latest snapshot
  public webcamImage: WebcamImage = null;

  // webcam snapshot trigger
  private trigger: Subject<void> = new Subject<void>();
  // switch to next / previous / specific webcam; true/false: forward/backwards, string: deviceId
  private nextWebcam: Subject<boolean|string> = new Subject<boolean|string>();

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
  }

  public triggerSnapshot(): void {
    this.trigger.next();
    this.postImage();
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
    console.info('received webcam image', webcamImage);
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

  public postImage() {

    const body = {
      base64Image: this.webcamImage.imageAsBase64
    };

    this.alertify.message('Now working on it...');

    this.stageService.uploadTest(body).subscribe(response => {

      this.alertify.success('Recognition success.');
      this.labels = response;

    }, error => {
      this.alertify.error('Something wrong. Try again.');
    });

  }
}