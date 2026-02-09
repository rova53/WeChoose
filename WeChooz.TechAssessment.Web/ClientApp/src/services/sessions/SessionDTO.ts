import { CourseDTO } from "../courses/CourseDTO";

export interface SessionDTO {
    id: string;
    courseId: string;
    courseName: string;
    startDate: string; // DateOnly côté C#, string (ISO) côté JS
    deliveryMode: string; // ou number si l'API renvoie la valeur numérique de l'enum
    UserCount: number;
    course: CourseDTO;
}