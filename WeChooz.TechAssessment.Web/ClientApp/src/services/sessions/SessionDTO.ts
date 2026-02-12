import { CourseDTO } from "../courses/CourseDTO";
import { DeliveryMode } from "./DeliveryMode";

export interface SessionDTO {
    id: string;
    courseId: string;
    courseName: string;
    startDate: string;
    deliveryMode: DeliveryMode;
    UserCount: number;
    course: CourseDTO;
}