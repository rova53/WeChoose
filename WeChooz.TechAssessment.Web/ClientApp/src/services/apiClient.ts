import ky from 'ky';

const api = ky.create({
  prefixUrl: '/_api',
  headers: {
    'Content-Type': 'application/json',
  },
  // Ajoute ici d'autres options globales si besoin (auth, etc.)
});

export const apiClient = {
  get: async <T>(url: string) => {
    const data = await api.get(url).json<T>();
    return data;
  },
  post: async <T>(url: string, body: any) => {
    const data = await api.post(url, { json: body }).json<T>();
    return data;
  },
  put: <T>(url: string, data: unknown) => { return api.put(url, { json: data }).json<T>() },
  delete: <T>(url: string) => { return api.delete(url).json<T>() },
};