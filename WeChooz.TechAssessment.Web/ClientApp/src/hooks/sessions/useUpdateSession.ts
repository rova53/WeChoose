import { SessionDTO } from "../../services/sessions/SessionDTO";
import { SessionService } from "../../services/sessions/SessionService";
import { useApiMutation } from "../useApiMutation";

export const useUpdateSession = () => {
    return useApiMutation(
        ({ id, session }: { id: string; session: Partial<SessionDTO> }) =>
            SessionService.update(id, session)
    );
};
