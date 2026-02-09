import { useApiQuery } from '../useApiQuery';
import { CourseService } from '../../services/courses/CourseService';
import { CourseDTO } from '../../services/courses/CourseDTO';

export const useGetCourseById = (id: string) => {
    const { data: course, loading, error, refetch } = useApiQuery<CourseDTO>(
        () => CourseService.getById(id),
        [id]
    );

    return {
        course,
        loading,
        error,
        refetch
    };
};