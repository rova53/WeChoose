import { SessionEnroll } from "../sessions/SessionEnrollDTO";

export interface UserDTO {
    id: string;
    lastName: string;
    firstName: string;
    email: string;
    companyName: string;
    enrollments: SessionEnroll[];
    password?: string;
}