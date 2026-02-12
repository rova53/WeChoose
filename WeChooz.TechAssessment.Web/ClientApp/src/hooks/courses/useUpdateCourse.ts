import { CourseDTO } from "../../services/courses/CourseDTO";
import { CourseService } from "../../services/courses/CourseService";
import { useApiMutation } from "../useApiMutation";

export const useUpdateCourse = () => {
    return useApiMutation(
        ({ id, course }: { id: string; course: Partial<CourseDTO> }) =>
            CourseService.update(id, course)
    );
};