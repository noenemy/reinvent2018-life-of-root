import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Stageobject } from '../_models/stageobject';

@Injectable({
  providedIn: 'root'
})
export class StageobjectService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getStageObjects(): Observable<Stageobject[]> {
    return this.http.get<Stageobject[]>(this.baseUrl + 'stageobjects');
  }
}
