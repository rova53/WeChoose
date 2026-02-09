import { useApiMutation } from '../useApiMutation';
import { CourseService } from '../../services/courses/CourseService';

export const useDeleteCourse = () => {
    return useApiMutation<void, string>(
        (id) => CourseService.delete(id)
    );
};