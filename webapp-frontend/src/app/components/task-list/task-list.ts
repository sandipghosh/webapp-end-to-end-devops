import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { TaskService } from '../../services';
import { TaskItem } from '../../models';
import { TaskFormComponent } from '../task-form';

import { MatDialog } from '@angular/material/dialog';
import { MatTable, MatTableModule } from '@angular/material/table';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.html',
  styleUrls: ['./task-list.scss'],
  imports:[
    CommonModule,
    DatePipe,
    MatTableModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule
  ],
  standalone: true
})
export class TaskListComponent implements OnInit {
  displayedColumns = ['title', 'description', 'isCompleted', 'createdAt', 'actions'];
  tasks: TaskItem[] = [];
  loading = false;

  @ViewChild(MatTable) table?: MatTable<TaskItem>;

  constructor(
    private taskService: TaskService,
    private dialog: MatDialog,
    private snack: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.taskService.list().subscribe({
      next: items => {
        this.tasks = items;
        this.loading = false;
      },
      error: err => {
        console.error(err);
        this.snack.open('Error loading tasks', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  openCreate(): void {
    const ref = this.dialog.open(TaskFormComponent, {
      width: '480px',
      data: { mode: 'create' }
    });

    ref.afterClosed().subscribe(result => {
      if (result === 'saved') this.load();
    });
  }

  openEdit(task: TaskItem): void {
    const ref = this.dialog.open(TaskFormComponent, {
      width: '480px',
      data: { mode: 'edit', task }
    });

    ref.afterClosed().subscribe(result => {
      if (result === 'saved') this.load();
    });
  }

  onDelete(task: TaskItem): void {
    if (!confirm(`Delete task "${task.title}"?`)) return;

    this.taskService.delete(task.id).subscribe({
      next: _ => {
        this.snack.open('Task deleted', 'Close', { duration: 2000 });
        this.load();
      },
      error: err => {
        console.error(err);
        this.snack.open('Failed to delete', 'Close', { duration: 3000 });
      }
    });
  }

  toggleComplete(task: TaskItem): void {
    const payload = {
      title: task.title,
      description: task.description ?? null,
      isCompleted: !task.isCompleted
    };
    this.taskService.update(task.id, payload).subscribe({
      next: _ => {
        this.snack.open('Updated', 'Close', { duration: 1500 });
        this.load();
      },
      error: err => {
        console.error(err);
        this.snack.open('Failed to update', 'Close', { duration: 3000 });
      }
    });
  }
}
