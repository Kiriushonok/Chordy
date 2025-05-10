export default async function fetchWithRefresh(url, options = {}) {
    let response = await fetch(url, { ...options, credentials: 'include' });

    if (response.status === 401 || response.status === 403) {
        // Пробуем обновить токены
        const refreshRes = await fetch('https://localhost:7007/api/users/refresh', {
            method: 'POST',
            credentials: 'include'
        });

        if (refreshRes.ok) {
            // После успешного refresh повторяем исходный запрос
            response = await fetch(url, { ...options, credentials: 'include' });
            
            // Если запрос все еще не удался, возвращаем ошибку
            if (!response.ok) {
                throw new Error('Failed to fetch after token refresh');
            }
        } else {
            throw new Error('Failed to refresh token');
        }
    }

    return response;
}
