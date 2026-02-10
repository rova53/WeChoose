import { useState } from 'react';

interface MutationOptions<TData> {
    onSuccess?: (data: TData) => void;
    onError?: (error: Error) => void;
}

export const useApiMutation = <TData, TVariables>(
    mutationFn: (variables: TVariables) => Promise<TData>
) => {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<Error | null>(null);
    const [data, setData] = useState<TData | null>(null);

    const mutate = async (
        variables: TVariables,
        options?: MutationOptions<TData>
    ): Promise<TData | null> => {
        try {
            setLoading(true);
            setError(null);
            const result = await mutationFn(variables);
            setData(result);
            options?.onSuccess?.(result);
            return result;
        } catch (err) {
            if (err?.response?.status === 401) {
                window.location.href = '/login';
                return null;
            }
            // Récupération du message d'erreur personnalisé
            let errorMessage = "Une erreur est survenue.";
            if (err?.response) {
                try {
                    const data = await err.response.json();
                    errorMessage = data?.message || data || errorMessage;
                } catch {
                    // ignore
                }
            } else if (err?.message) {
                errorMessage = err.message;
            }

            const error = new Error(errorMessage);
            setError(error);
            options?.onError?.(error);
            return null;
        } finally {
            setLoading(false);
        }
    };

    return { mutate, loading, error, data };
};