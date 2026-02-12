import { UserDTO } from "../../services/users/UserDTO";
import { UserService } from "../../services/users/UserService";
import { useApiMutation } from "../useApiMutation";

export const useUpdateUser = () => {
    return useApiMutation(
        ({ id, user }: { id: string; user: Partial<UserDTO> }) =>
            UserService.update(id, user)
    );
};
