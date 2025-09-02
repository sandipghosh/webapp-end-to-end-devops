import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { TaskService } from './task.service';
import { EnvService } from './env.service';
import { TaskItem } from '../models';

// ✅ import env.js so it sets window.__env
import '../../../env.js';

describe('TaskService', () => {
    let service: TaskService;
    let httpMock: HttpTestingController;
    let mockApiUrl: string;

    const mockGuid = '550e8400-e29b-41d4-a716-446655440000';

    const mockTask: TaskItem = {
        id: mockGuid,
        title: 'Test Task',
        description: 'Some description',
        isCompleted: false,
        createdAt: new Date().toISOString()
    };

    beforeEach(() => {
        (window as any).__env = { apiUrl: 'http://localhost:8080/api' };

        TestBed.configureTestingModule({
            providers: [
                TaskService,
                EnvService,
                provideHttpClient(),
                provideHttpClientTesting()
            ]
        });

        service = TestBed.inject(TaskService);
        httpMock = TestBed.inject(HttpTestingController);

        mockApiUrl = (window as any).__env.apiUrl + '/tasks';
    });


    afterEach(() => {
        httpMock.verify();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    describe('list()', () => {
        it('should return an array of tasks', () => {
            service.list().subscribe(tasks => {
                expect(tasks).toEqual([mockTask]);
            });

            const req = httpMock.expectOne(mockApiUrl);
            expect(req.request.method).toBe('GET');
            req.flush([mockTask]);
        });
    });

    describe('get()', () => {
        it('should return a single task', () => {
            service.get(mockGuid).subscribe(task => {
                expect(task).toEqual(mockTask);
            });

            const req = httpMock.expectOne(`${mockApiUrl}/${mockGuid}`);
            expect(req.request.method).toBe('GET');
            req.flush(mockTask);
        });
    });

    describe('create()', () => {
        it('should create a task and return it', () => {
            const payload = { title: 'New Task', description: 'Desc' };

            service.create(payload).subscribe(task => {
                expect(task).toEqual(mockTask);
            });

            const req = httpMock.expectOne(mockApiUrl);
            expect(req.request.method).toBe('POST');
            expect(req.request.body).toEqual(payload);
            req.flush(mockTask);
        });
    });

    describe('update()', () => {
        it('should update a task and return boolean', () => {
            const payload = { title: 'Updated', description: 'Desc', isCompleted: true };

            service.update(mockGuid, payload).subscribe(result => {
                expect(result).toBeTrue();
            });

            const req = httpMock.expectOne(`${mockApiUrl}/${mockGuid}`);
            expect(req.request.method).toBe('PUT');
            expect(req.request.body).toEqual(payload);
            req.flush(true);
        });
    });

    describe('delete()', () => {
        it('should delete a task and return boolean', () => {
            service.delete(mockGuid).subscribe(result => {
                expect(result).toBeTrue();
            });

            const req = httpMock.expectOne(`${mockApiUrl}/${mockGuid}`);
            expect(req.request.method).toBe('DELETE');
            req.flush(true);
        });
    });
});
