import { apiClient } from '../apiClient';
import { CourseDTO } from './CourseDTO';

export const CourseService = {
    getAll: () => apiClient.get<CourseDTO[]>('courses'),
    getById: (id: string) => apiClient.get<CourseDTO>(`courses/${id}`),
    create: (course: Partial<CourseDTO>) => apiClient.post<CourseDTO>('courses', course),
    update: (id: string, course: Partial<CourseDTO>) => apiClient.put<CourseDTO>(`courses/${id}`, course),
    delete: (id: string) => apiClient.delete<void>(`courses/${id}`),
};