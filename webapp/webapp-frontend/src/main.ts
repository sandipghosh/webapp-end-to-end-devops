import { bootstrapApplication } from '@angular/platform-browser';
import { importProvidersFrom } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { AppComponent } from './app/app.component';
import { RouterModule, Routes } from '@angular/router';
import { TaskListComponent } from './app/components/task-list'

const routes: Routes = [
  { path: '', component: TaskListComponent },
  { path: '**', redirectTo: '' }
];

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(),
    provideAnimations(),
    importProvidersFrom(RouterModule.forRoot(routes))
  ]
}).catch(err => console.error(err));
