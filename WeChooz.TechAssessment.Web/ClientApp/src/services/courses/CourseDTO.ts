import { TargetAudience } from "../../admin/components/common/TargetAudience";

export interface CourseDTO {
    id: string;
    name: string;
    shortDescription: string;
    longDescription: string;
    durationInDays: number;
    targetAudience: TargetAudience;
    maxCapacity: number;
    trainerFirstName: string;
    trainerLastName: string;
    sessionCount: number;
}