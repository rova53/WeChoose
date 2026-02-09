export interface CourseDTO {
    id: string;
    name: string;
    shortDescription: string;
    longDescription: string;
    durationInDays: number;
    targetAudience: string;
    maxCapacity: number;
    trainerFirstName: string;
    trainerLastName: string;
    sessionCount: number;
}