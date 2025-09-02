import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter, Router } from '@angular/router';
import { Location } from '@angular/common';
import { AppComponent } from './app/app.component';
import { TaskListComponent } from './app/components/task-list';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ComponentFixture } from '@angular/core/testing';

describe('Main bootstrap configuration', () => {
    let fixture: ComponentFixture<AppComponent>;
    let router: Router;
    let location: Location;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AppComponent, NoopAnimationsModule],
            providers: [
                provideHttpClient(),
                provideAnimations(),
                provideRouter([
                    { path: '', component: TaskListComponent },
                    { path: '**', redirectTo: '' }
                ])
            ]
        }).compileComponents();

        router = TestBed.inject(Router);
        location = TestBed.inject(Location);

        fixture = TestBed.createComponent(AppComponent);
        fixture.detectChanges();

        await router.initialNavigation();
    });

    it('should create the app component', () => {
        expect(fixture.componentInstance).toBeTruthy();
    });

    it('should navigate to "" and display TaskListComponent', async () => {
        await router.navigate(['']);
        fixture.detectChanges();
        expect(location.path()).toBe('');
    });

    it('should redirect unknown routes to ""', async () => {
        await router.navigate(['/unknown']);
        fixture.detectChanges();
        expect(location.path()).toBe(''); 
    });
});
