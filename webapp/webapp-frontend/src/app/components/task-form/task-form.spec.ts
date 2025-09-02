import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TaskFormComponent } from './task-form';
import { TaskService } from '../../services';
import { TaskItem } from '../../models';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';

describe('TaskFormComponent', () => {
    let component: TaskFormComponent;
    let fixture: ComponentFixture<TaskFormComponent>;
    let taskServiceSpy: jasmine.SpyObj<TaskService>;
    let snackSpy: jasmine.SpyObj<MatSnackBar>;
    let dialogRefSpy: jasmine.SpyObj<MatDialogRef<TaskFormComponent>>;

    const mockGuid = '550e8400-e29b-41d4-a716-446655440000';

    beforeEach(async () => {
        taskServiceSpy = jasmine.createSpyObj('TaskService', ['create', 'update']);
        snackSpy = jasmine.createSpyObj('MatSnackBar', ['open']);
        dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);

        await TestBed.configureTestingModule({
            imports: [TaskFormComponent, ReactiveFormsModule],
            providers: [
                FormBuilder,
                { provide: TaskService, useValue: taskServiceSpy },
                { provide: MatSnackBar, useValue: snackSpy },
                { provide: MatDialogRef, useValue: dialogRefSpy },
                {
                    provide: MAT_DIALOG_DATA,
                    useValue: { mode: 'create' } // default test mode
                }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(TaskFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('form initialization', () => {
        it('should create form with empty values in create mode', () => {
            expect(component.form.value).toEqual({
                title: '',
                description: '',
                isCompleted: false
            });
        });

        it('should prefill form in edit mode', async () => {
            const data = {
                mode: 'edit',
                task: {
                    id: mockGuid,
                    title: 'Test',
                    description: 'Desc',
                    isCompleted: true,
                    createdAt: new Date().toISOString()
                } as TaskItem
            };
            //TestBed.overrideProvider(MAT_DIALOG_DATA, { useValue: data });
            TestBed.resetTestingModule();
            await TestBed.configureTestingModule({
                imports: [TaskFormComponent, ReactiveFormsModule],
                providers: [
                    FormBuilder,
                    { provide: TaskService, useValue: taskServiceSpy },
                    { provide: MatSnackBar, useValue: snackSpy },
                    { provide: MatDialogRef, useValue: dialogRefSpy },
                    { provide: MAT_DIALOG_DATA, useValue: data }
                ]
            }).compileComponents();

            fixture = TestBed.createComponent(TaskFormComponent);
            component = fixture.componentInstance;
            fixture.detectChanges();

            expect(component.form.value).toEqual({
                title: 'Test',
                description: 'Desc',
                isCompleted: true
            });
        });
    });

    describe('save()', () => {
        it('should not call service if form invalid', () => {
            component.form.controls['title'].setValue('');
            component.save();
            expect(taskServiceSpy.create).not.toHaveBeenCalled();
            expect(taskServiceSpy.update).not.toHaveBeenCalled();
        });

        it('should call create() and close dialog on success (create mode)', () => {
            component.form.setValue({ title: 'New Task', description: 'Desc', isCompleted: false });
            taskServiceSpy.create.and.returnValue(of({
                id: mockGuid,
                title: 'New Task',
                description: 'Desc',
                isCompleted: false,
                createdAt: new Date().toISOString()
            } as TaskItem));

            component.save();

            expect(taskServiceSpy.create).toHaveBeenCalledWith({ title: 'New Task', description: 'Desc' });
            expect(snackSpy.open).toHaveBeenCalledWith('Task created', 'Close', { duration: 2000 });
            expect(dialogRefSpy.close).toHaveBeenCalledWith('saved');
        });

        it('should show error when create() fails', () => {
            component.form.setValue({ title: 'Fail', description: 'Desc', isCompleted: false });
            taskServiceSpy.create.and.returnValue(throwError(() => new Error('fail')));

            component.save();

            expect(snackSpy.open).toHaveBeenCalledWith('Failed to create', 'Close', { duration: 3000 });
            expect(component.saving).toBeFalse();
        });

        it('should call update() and close dialog on success (edit mode)', () => {
            component.mode = 'edit';
            component.task = {
                id: mockGuid,
                title: 'Old',
                description: 'OldDesc',
                isCompleted: false,
                createdAt: new Date().toISOString()
            };
            component.form.setValue({ title: 'Updated', description: 'NewDesc', isCompleted: true });

            // ✅ FIX: Return Observable<boolean>
            taskServiceSpy.update.and.returnValue(of(true));

            component.save();

            expect(taskServiceSpy.update).toHaveBeenCalledWith(mockGuid, {
                title: 'Updated',
                description: 'NewDesc',
                isCompleted: true
            });
            expect(snackSpy.open).toHaveBeenCalledWith('Task updated', 'Close', { duration: 2000 });
            expect(dialogRefSpy.close).toHaveBeenCalledWith('saved');
        });


        it('should show error when update() fails', () => {
            component.mode = 'edit';
            component.task = {
                id: mockGuid,
                title: 'Old',
                description: 'OldDesc',
                isCompleted: false,
                createdAt: new Date().toISOString()
            };
            component.form.setValue({ title: 'Updated', description: 'NewDesc', isCompleted: true });
            taskServiceSpy.update.and.returnValue(throwError(() => new Error('fail')));

            component.save();

            expect(snackSpy.open).toHaveBeenCalledWith('Failed to update', 'Close', { duration: 3000 });
            expect(component.saving).toBeFalse();
        });
    });

    describe('close()', () => {
        it('should close dialog', () => {
            component.close();
            expect(dialogRefSpy.close).toHaveBeenCalled();
        });
    });
});
