import { apiClient } from '../apiClient';
import { CourseDTO } from './CourseDTO';

export const CourseService = {
    getAll: () => apiClient.get<CourseDTO[]>('courses'),
    getById: (id: string) => apiClient.get<CourseDTO>(`course/${id}`),
    create: (course: Partial<CourseDTO>) => apiClient.post<CourseDTO>('course', course),
    update: (id: string, course: Partial<CourseDTO>) => apiClient.put<CourseDTO>(`course/${id}`, course),
    delete: (id: string) => apiClient.delete<void>(`course/${id}`),
};