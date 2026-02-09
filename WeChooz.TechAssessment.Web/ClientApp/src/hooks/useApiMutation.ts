import { useState } from 'react';

export const useApiMutation = <TData, TVariables>(
    mutationFn: (variables: TVariables) => Promise<TData>
) => {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<Error | null>(null);
    const [data, setData] = useState<TData | null>(null);

    const mutate = async (variables: TVariables): Promise<TData | null> => {
        try {
            setLoading(true);
            setError(null);
            const result = await mutationFn(variables);
            setData(result);
            return result;
        } catch (err) {
            setError(err as Error);
            return null;
        } finally {
            setLoading(false);
        }
    };

    return { mutate, loading, error, data };
};
