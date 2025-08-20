import { Injectable } from '@angular/core';

declare global {
  interface Window {
    __env: any;
  }
}

@Injectable({
  providedIn: 'root'
})
export class EnvService {
  private readonly env = window.__env;

  get apiUrl(): string {
    return this.env?.apiUrl || 'http://localhost:5000/api';
  }

  get debug(): boolean {
    return this.env?.debug ?? false;
  }
}
