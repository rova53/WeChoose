import { UserService } from "../../services/users/UserService";
import { useApiMutation } from "../useApiMutation";

export const useCreateUser = () => {
    return useApiMutation(UserService.create);
};
