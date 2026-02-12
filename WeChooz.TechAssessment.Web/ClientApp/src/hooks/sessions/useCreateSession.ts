import { SessionService } from "../../services/sessions/SessionService";
import { useApiMutation } from "../useApiMutation";

export const useCreateSession = () => {
    return useApiMutation(SessionService.create);
};
