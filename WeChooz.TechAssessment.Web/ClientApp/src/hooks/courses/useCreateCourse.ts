import { CourseService } from "../../services/courses/CourseService";
import { useApiMutation } from "../useApiMutation";

export const useCreateCourse = () => {
    return useApiMutation(CourseService.create);
};