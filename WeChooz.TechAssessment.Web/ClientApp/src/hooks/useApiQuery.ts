import { useState, useEffect } from 'react';

export const useApiQuery = <TData>(
    queryFn: () => Promise<TData>,
    dependencies: any[] = []
) => {
    const [data, setData] = useState<TData | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<Error | null>(null);

    const refetch = async () => {
        try {
            setLoading(true);
            setError(null);
            const result = await queryFn();
            setData(result);
        } catch (err) {
            console.error('Erreur API:', err);
            setError(err as Error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        refetch();
    }, dependencies);

    return { data, loading, error, refetch };
};