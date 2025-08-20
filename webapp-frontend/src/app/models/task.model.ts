export interface TaskItem {
  id: string;             // GUID from backend
  title: string;
  description?: string | null;
  isCompleted: boolean;
  createdAt: string;      // ISO string
}
