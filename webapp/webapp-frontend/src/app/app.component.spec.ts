import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { By } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideLocationMocks } from '@angular/common/testing';

describe('AppComponent', () => {
    let fixture: ComponentFixture<AppComponent>;
    let component: AppComponent;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AppComponent],
            providers: [
                provideRouter([]),        // ✅ replaces RouterTestingModule
                provideLocationMocks()    // ✅ provides mock Location/LocationStrategy
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(AppComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create the app', () => {
        expect(component).toBeTruthy();
    });

    it('should render title inside toolbar', () => {
        const toolbarEl: HTMLElement = fixture.debugElement.query(By.css('mat-toolbar')).nativeElement;
        expect(toolbarEl.textContent).toContain('Todo App');
    });

    it('should render toolbar', () => {
        const toolbar = fixture.debugElement.query(By.css('mat-toolbar'));
        expect(toolbar).toBeTruthy();
    });
});
