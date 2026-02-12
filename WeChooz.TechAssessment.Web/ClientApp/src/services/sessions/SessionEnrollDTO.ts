import { UserDTO } from "../users/UserDTO";
import { SessionDTO } from "./SessionDTO";

export interface SessionEnroll {
    sessionId: string;
    enrollmentDate: string;
    session: SessionDTO;
    user: UserDTO;
}
