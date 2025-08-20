import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TaskService } from '../../services';
import { TaskItem } from '../../models';

import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef, MatDialogContent } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule, MatCheckbox } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';

type Mode = 'create' | 'edit';

@Component({
  selector: 'app-task-form',
  templateUrl: './task-form.html',
  styleUrls: ['./task-form.scss'],
  imports: [
    MatDialogContent, 
    MatCheckbox,
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule
  ],
  standalone: true
})
export class TaskFormComponent implements OnInit {
  form!: FormGroup;
  mode: Mode;
  task?: TaskItem;
  saving = false;

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private snack: MatSnackBar,
    private dialogRef: MatDialogRef<TaskFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { mode: Mode; task?: TaskItem }
  ) {
    this.mode = data.mode;
    this.task = data.task;
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      title: [this.task?.title ?? '', [Validators.required, Validators.maxLength(200)]],
      description: [this.task?.description ?? ''],
      isCompleted: [this.task?.isCompleted ?? false]
    });
  }

  save(): void {
    if (this.form.invalid) return;
    this.saving = true;

    const { title, description, isCompleted } = this.form.value;

    if (this.mode === 'create') {
      this.taskService.create({ title, description }).subscribe({
        next: _ => {
          this.snack.open('Task created', 'Close', { duration: 2000 });
          this.dialogRef.close('saved');
        },
        error: err => {
          console.error(err);
          this.snack.open('Failed to create', 'Close', { duration: 3000 });
          this.saving = false;
        }
      });
    } else {
      if (!this.task) return;
      this.taskService.update(this.task.id, { title, description, isCompleted }).subscribe({
        next: _ => {
          this.snack.open('Task updated', 'Close', { duration: 2000 });
          this.dialogRef.close('saved');
        },
        error: err => {
          console.error(err);
          this.snack.open('Failed to update', 'Close', { duration: 3000 });
          this.saving = false;
        }
      });
    }
  }

  close(): void {
    this.dialogRef.close();
  }
}
