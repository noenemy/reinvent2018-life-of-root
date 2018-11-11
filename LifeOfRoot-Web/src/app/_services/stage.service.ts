import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Stageinfo } from '../_models/stageinfo';
import { Stagescore } from '../_models/stagescore';

@Injectable({
  providedIn: 'root'
})
export class StageService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getStage(game_id, stage_id): Observable<Stageinfo> {
    return this.http.get<Stageinfo>(this.baseUrl + 'stages?game_id=' + game_id + '&stage_id=' + stage_id);
  }

  uploadPicture(stage_object): Observable<Stagescore> {
    return this.http.post<Stagescore>(this.baseUrl + 'stages', stage_object);
  }
}
