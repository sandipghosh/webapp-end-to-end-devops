import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TaskListComponent } from './task-list';
import { TaskService } from '../../services';
import { TaskItem } from '../../models';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { of, throwError, Subject } from 'rxjs';

describe('TaskListComponent', () => {
    let component: TaskListComponent;
    let fixture: ComponentFixture<TaskListComponent>;
    let taskServiceSpy: jasmine.SpyObj<TaskService>;
    let snackSpy: jasmine.SpyObj<MatSnackBar>;
    let dialogSpy: jasmine.SpyObj<MatDialog>;

    const mockGuid = '550e8400-e29b-41d4-a716-446655440000';

    const mockTasks: TaskItem[] = [
        {
            id: mockGuid,
            title: 'Task 1',
            description: 'Desc 1',
            isCompleted: false,
            createdAt: new Date().toISOString()
        },
        {
            id: '11111111-2222-3333-4444-555555555555',
            title: 'Task 2',
            description: 'Desc 2',
            isCompleted: true,
            createdAt: new Date().toISOString()
        }
    ];

    beforeEach(async () => {
        taskServiceSpy = jasmine.createSpyObj('TaskService', ['list', 'delete', 'update']);
        snackSpy = jasmine.createSpyObj('MatSnackBar', ['open']);
        dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);

        taskServiceSpy.list.and.returnValue(of([]));

        await TestBed.configureTestingModule({
            imports: [TaskListComponent],
            providers: [
                { provide: TaskService, useValue: taskServiceSpy },
                { provide: MatSnackBar, useValue: snackSpy }
                // don’t provide MatDialog here
            ]
        }).overrideComponent(TaskListComponent, {
            remove: { imports: [MatDialogModule] }, // remove real module
            add: { providers: [{ provide: MatDialog, useValue: dialogSpy }] }
        }).compileComponents();

        fixture = TestBed.createComponent(TaskListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create component', () => {
        expect(component).toBeTruthy();
    });

    describe('load()', () => {
        it('should load tasks successfully', () => {
            taskServiceSpy.list.and.returnValue(of(mockTasks));

            component.load();

            expect(taskServiceSpy.list).toHaveBeenCalled();
            expect(component.tasks.length).toBe(2);
            expect(component.loading).toBeFalse();
        });

        it('should handle error on load', () => {
            taskServiceSpy.list.and.returnValue(throwError(() => new Error('fail')));

            component.load();

            expect(snackSpy.open).toHaveBeenCalledWith('Error loading tasks', 'Close', { duration: 3000 });
            expect(component.loading).toBeFalse();
        });
    });

    describe('openCreate()', () => {
        it('should reload when dialog returns saved', () => {
            const afterClosed$ = of('saved');
            dialogSpy.open.and.returnValue({ afterClosed: () => afterClosed$ } as any);

            spyOn(component, 'load');

            component.openCreate();

            expect(dialogSpy.open).toHaveBeenCalled();
            expect(component.load).toHaveBeenCalled();
        });

        it('should not reload when dialog returns something else', () => {
            const afterClosed$ = of('cancelled');
            dialogSpy.open.and.returnValue({ afterClosed: () => afterClosed$ } as any);

            spyOn(component, 'load');

            component.openCreate();

            expect(component.load).not.toHaveBeenCalled();
        });
    });

    describe('openEdit()', () => {
        it('should reload when dialog returns saved', () => {
            const afterClosed$ = of('saved');
            dialogSpy.open.and.returnValue({ afterClosed: () => afterClosed$ } as any);

            spyOn(component, 'load');

            component.openEdit(mockTasks[0]);

            expect(dialogSpy.open).toHaveBeenCalled();
            expect(component.load).toHaveBeenCalled();
        });
    });

    describe('onDelete()', () => {
        it('should delete and reload on success', () => {
            spyOn(window, 'confirm').and.returnValue(true);
            taskServiceSpy.delete.and.returnValue(of(true));
            spyOn(component, 'load');

            component.onDelete(mockTasks[0]);

            expect(taskServiceSpy.delete).toHaveBeenCalledWith(mockTasks[0].id);
            expect(snackSpy.open).toHaveBeenCalledWith('Task deleted', 'Close', { duration: 2000 });
            expect(component.load).toHaveBeenCalled();
        });

        it('should not call delete if confirm is false', () => {
            spyOn(window, 'confirm').and.returnValue(false);

            component.onDelete(mockTasks[0]);

            expect(taskServiceSpy.delete).not.toHaveBeenCalled();
        });

        it('should show error when delete fails', () => {
            spyOn(window, 'confirm').and.returnValue(true);
            taskServiceSpy.delete.and.returnValue(throwError(() => new Error('fail')));

            component.onDelete(mockTasks[0]);

            expect(snackSpy.open).toHaveBeenCalledWith('Failed to delete', 'Close', { duration: 3000 });
        });
    });

    describe('toggleComplete()', () => {
        it('should update and reload on success', () => {
            taskServiceSpy.update.and.returnValue(of(true));
            spyOn(component, 'load');

            component.toggleComplete(mockTasks[0]);

            expect(taskServiceSpy.update).toHaveBeenCalledWith(mockTasks[0].id, {
                title: mockTasks[0].title,
                description: mockTasks[0].description,
                isCompleted: !mockTasks[0].isCompleted
            });
            expect(snackSpy.open).toHaveBeenCalledWith('Updated', 'Close', { duration: 1500 });
            expect(component.load).toHaveBeenCalled();
        });

        it('should show error when update fails', () => {
            taskServiceSpy.update.and.returnValue(throwError(() => new Error('fail')));

            component.toggleComplete(mockTasks[0]);

            expect(snackSpy.open).toHaveBeenCalledWith('Failed to update', 'Close', { duration: 3000 });
        });
    });
});
