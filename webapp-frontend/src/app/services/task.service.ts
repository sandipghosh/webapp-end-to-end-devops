import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { TaskItem } from '../models';
import { EnvService } from './env.service';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly apiUrl: string;

  constructor(private http: HttpClient, private env: EnvService) {
    this.apiUrl = this.env.apiUrl + '/tasks';
  }

  list(): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(this.apiUrl);
  }

  get(id: string): Observable<TaskItem> {
    return this.http.get<TaskItem>(`${this.apiUrl}/${id}`);
  }

  create(payload: { title: string; description?: string | null }): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.apiUrl, payload);
  }

  update(id: string, payload: { title: string; description?: string | null; isCompleted: boolean }): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/${id}`, payload);
  }

  delete(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`);
  }
}
