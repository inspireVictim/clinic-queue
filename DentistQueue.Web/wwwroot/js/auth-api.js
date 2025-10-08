// API сервис для работы с аутентификацией
class AuthAPI {
    constructor() {
        this.baseUrl = '/api';
        this.token = localStorage.getItem('authToken');
    }

    // Устанавливает токен авторизации
    setToken(token) {
        this.token = token;
        if (token) {
            localStorage.setItem('authToken', token);
        } else {
            localStorage.removeItem('authToken');
        }
    }

    // Получает заголовки для запросов
    getHeaders() {
        const headers = {
            'Content-Type': 'application/json'
        };

        if (this.token) {
            headers['Authorization'] = `Bearer ${this.token}`;
        }

        return headers;
    }

    // Выполняет HTTP запрос
    async request(endpoint, options = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const config = {
            headers: this.getHeaders(),
            ...options
        };

        try {
            const response = await fetch(url, config);
            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || `HTTP error! status: ${response.status}`);
            }

            return data;
        } catch (error) {
            console.error('API request failed:', error);
            throw error;
        }
    }

    // Регистрация пользователя
    async register(userData) {
        return await this.request('/auth/register', {
            method: 'POST',
            body: JSON.stringify(userData)
        });
    }

    // Вход в систему
    async login(email, password, rememberMe = false) {
        const result = await this.request('/auth/login', {
            method: 'POST',
            body: JSON.stringify({ email, password, rememberMe })
        });

        if (result.success && result.token) {
            this.setToken(result.token);
        }

        return result;
    }

    // Выход из системы
    logout() {
        this.setToken(null);
        localStorage.removeItem('currentUser');
    }

    // Получение информации о текущем пользователе
    async getCurrentUser() {
        try {
            return await this.request('/auth/me');
        } catch (error) {
            // Если токен недействителен, удаляем его
            if (error.message.includes('401') || error.message.includes('Unauthorized')) {
                this.logout();
            }
            throw error;
        }
    }

    // Проверка существования email
    async checkEmail(email) {
        return await this.request('/auth/check-email', {
            method: 'POST',
            body: JSON.stringify({ email })
        });
    }

    // Проверка валидности токена
    async validateToken() {
        try {
            await this.getCurrentUser();
            return true;
        } catch {
            return false;
        }
    }
}

// Глобальный экземпляр API
window.authAPI = new AuthAPI();
