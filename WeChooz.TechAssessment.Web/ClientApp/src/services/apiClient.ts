import ky from 'ky';

const api = ky.create({
  prefixUrl: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
  // Ajoute ici d'autres options globales si besoin (auth, etc.)
});

export const apiClient = {
  get: <T>(url: string) => api.get(url).json<T>(),
  post: <T>(url: string, data: unknown) => api.post(url, { json: data }).json<T>(),
  put: <T>(url: string, data: unknown) => api.put(url, { json: data }).json<T>(),
  delete: <T>(url: string) => api.delete(url).json<T>(),
};