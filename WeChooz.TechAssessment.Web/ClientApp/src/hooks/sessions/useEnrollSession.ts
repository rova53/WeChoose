import { useApiMutation } from "../useApiMutation";
import { SessionService } from "../../services/sessions/SessionService";

export const useEnrollSession = () => {
    return useApiMutation<void, string>(
        (sessionId: string) => {
            console.log(`Enrolling in session with ID: ${sessionId}`);
            return SessionService.enroll(sessionId);
        }
    );
};