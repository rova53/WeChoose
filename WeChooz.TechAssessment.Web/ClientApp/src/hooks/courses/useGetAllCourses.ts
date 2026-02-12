import { useApiQuery } from '../useApiQuery';
import { CourseService } from '../../services/courses/CourseService';
import { CourseDTO } from '../../services/courses/CourseDTO';

export const useGetAllCourses = () => {
    const { data: courses, loading, error, refetch } = useApiQuery<CourseDTO[]>(
        () => CourseService.getAll(),
        []
    );

    return {
        courses: courses || [],
        loading,
        error,
        refetch
    };
};