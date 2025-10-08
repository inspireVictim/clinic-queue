// Основной JavaScript файл для приложения
class DentistQueueApp {
    constructor() {
        this.doctors = [];
        this.currentUser = null;
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.loadMockData();
        this.renderDoctors();
        this.checkAuth();
    }

    setupEventListeners() {
        // Мобильное меню
        const navToggle = document.getElementById('nav-toggle');
        const navMenu = document.getElementById('nav-menu');
        
        if (navToggle && navMenu) {
            navToggle.addEventListener('click', () => {
                navMenu.classList.toggle('active');
            });
        }

        // Поиск врачей
        const searchForm = document.querySelector('.search-form');
        if (searchForm) {
            searchForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.searchDoctors();
            });
        }

        // Модальные окна
        document.addEventListener('click', (e) => {
            if (e.target.classList.contains('modal-close') || e.target.classList.contains('modal')) {
                this.closeModal();
            }
        });
    }

    async loadMockData() {
        try {
            console.log('Loading doctors from API...');
            // Загружаем данные с API
            const response = await fetch('/api/doctors');
            console.log('API response status:', response.status);
            if (response.ok) {
                const data = await response.json();
                // API возвращает данные в формате {value: [...]}
                this.doctors = data.value || data || [];
                console.log('Doctors loaded from API:', this.doctors.length, 'doctors');
            } else {
                console.error('API failed, no doctors available');
                this.doctors = [];
                this.showAlert('Не удалось загрузить список врачей. Попробуйте обновить страницу.', 'error');
            }
        } catch (error) {
            console.error('Не удалось загрузить данные с API:', error);
            this.doctors = [];
            this.showAlert('Ошибка загрузки данных. Проверьте подключение к интернету.', 'error');
        }
    }


    getDoctorServices(doctorId) {
        // Получаем услуги врача из API данных
        const doctor = this.doctors.find(d => d.id === doctorId);
        if (doctor && doctor.services) {
            return doctor.services.map(service => ({
                id: service.id,
                title: service.title,
                price: service.price,
                duration: service.duration,
                description: service.description
            }));
        }
        
        return [];
    }

    renderDoctors() {
        console.log('Rendering doctors:', this.doctors.length);
        const doctorsGrid = document.getElementById('doctors-grid');
        if (!doctorsGrid) {
            console.error('doctors-grid element not found');
            return;
        }

        if (this.doctors.length === 0) {
            doctorsGrid.innerHTML = `
                <div class="text-center" style="grid-column: 1 / -1; padding: 2rem;">
                    <h3>Врачи не найдены</h3>
                    <p>В данный момент нет доступных врачей. Попробуйте обновить страницу.</p>
                </div>
            `;
            return;
        }

        doctorsGrid.innerHTML = this.doctors.map(doctor => `
            <div class="doctor-card">
                <div class="doctor-photo">
                    <img src="${doctor.photoUrl || 'https://via.placeholder.com/300x300'}" alt="${doctor.fullName}" style="width: 100%; height: 200px; object-fit: cover; border-radius: 5px;">
                </div>
                <h3>${doctor.fullName}</h3>
                <p class="doctor-specialization">${doctor.specializations?.join(', ') || 'Стоматолог'}</p>
                <div class="doctor-rating">
                    <div class="rating">
                        ${this.renderStars(doctor.rating)}
                    </div>
                    <span>${doctor.rating} (${doctor.reviewCount} отзывов)</span>
                </div>
                <p class="doctor-clinic">${doctor.clinic?.name || 'Клиника'}, ${doctor.clinic?.city || 'Город'}</p>
                <p class="doctor-experience">Опыт работы: ${doctor.experienceYears} лет</p>
                <div class="doctor-price">от ${doctor.services?.[0]?.price || 0} сом</div>
                <button class="btn btn-primary" onclick="app.showDoctorDetails('${doctor.id}')">
                    Записаться на прием
                </button>
            </div>
        `).join('');
        console.log('Doctors rendered successfully');
    }

    renderStars(rating) {
        const fullStars = Math.floor(rating);
        const hasHalfStar = rating % 1 >= 0.5;
        let stars = '';

        for (let i = 0; i < fullStars; i++) {
            stars += '<span class="rating-star active">★</span>';
        }

        if (hasHalfStar) {
            stars += '<span class="rating-star active">☆</span>';
        }

        const emptyStars = 5 - Math.ceil(rating);
        for (let i = 0; i < emptyStars; i++) {
            stars += '<span class="rating-star">☆</span>';
        }

        return stars;
    }

    searchDoctors() {
        const cityInput = document.querySelector('.search-form input[placeholder="Город"]');
        const specializationInput = document.querySelector('.search-form input[placeholder="Специализация"]');
        
        const city = cityInput?.value.toLowerCase() || '';
        const specialization = specializationInput?.value.toLowerCase() || '';

        const filteredDoctors = this.doctors.filter(doctor => {
            const matchesCity = !city || (doctor.clinic?.city && doctor.clinic.city.toLowerCase().includes(city));
            const matchesSpecialization = !specialization || 
                doctor.specializations?.some(spec => spec.toLowerCase().includes(specialization));
            
            return matchesCity && matchesSpecialization;
        });

        this.renderFilteredDoctors(filteredDoctors);
    }

    renderFilteredDoctors(doctors) {
        const doctorsGrid = document.getElementById('doctors-grid');
        if (!doctorsGrid) return;

        if (doctors.length === 0) {
            doctorsGrid.innerHTML = `
                <div class="text-center" style="grid-column: 1 / -1; padding: 2rem;">
                    <h3>Врачи не найдены</h3>
                    <p>Попробуйте изменить критерии поиска</p>
                </div>
            `;
            return;
        }

        doctorsGrid.innerHTML = doctors.map(doctor => `
            <div class="doctor-card">
                <div class="doctor-photo">
                    <img src="${doctor.photoUrl || 'https://via.placeholder.com/300x300'}" alt="${doctor.fullName}" style="width: 100%; height: 200px; object-fit: cover; border-radius: 5px;">
                </div>
                <h3>${doctor.fullName}</h3>
                <p class="doctor-specialization">${doctor.specializations?.join(', ') || 'Стоматолог'}</p>
                <div class="doctor-rating">
                    <div class="rating">
                        ${this.renderStars(doctor.rating)}
                    </div>
                    <span>${doctor.rating} (${doctor.reviewCount} отзывов)</span>
                </div>
                <p class="doctor-clinic">${doctor.clinic?.name || 'Клиника'}, ${doctor.clinic?.city || 'Город'}</p>
                <p class="doctor-experience">Опыт работы: ${doctor.experienceYears} лет</p>
                <div class="doctor-price">от ${doctor.services?.[0]?.price || 0} сом</div>
                <button class="btn btn-primary" onclick="app.showDoctorDetails('${doctor.id}')">
                    Записаться на прием
                </button>
            </div>
        `).join('');
    }

    showDoctorDetails(doctorId) {
        console.log('showDoctorDetails called with doctorId:', doctorId);
        const doctor = this.doctors.find(d => d.id === doctorId);
        if (!doctor) {
            console.error('Doctor not found with id:', doctorId);
            return;
        }

        console.log('Current user:', this.currentUser);
        // Проверяем авторизацию
        if (!this.currentUser) {
            console.log('No current user, showing login modal');
            this.showLoginModal();
            return;
        }

        // Показываем модальное окно записи
        this.showAppointmentModal(doctor);
    }

    showAppointmentModal(doctor) {
        const modal = document.createElement('div');
        modal.className = 'modal show';
        modal.innerHTML = `
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title">Запись к врачу</h3>
                    <button class="modal-close">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="doctor-info mb-3">
                        <h4>${doctor.fullName}</h4>
                        <p>${doctor.specializations?.join(', ') || 'Стоматолог'}</p>
                        <p>${doctor.clinic?.name || 'Клиника'}, ${doctor.clinic?.city || 'Город'}</p>
                    </div>
                    <form id="appointment-form">
                        <div class="form-group">
                            <label class="form-label">Выберите услугу:</label>
                            <select class="form-control" id="appointment-service" required>
                                <option value="">Выберите услугу</option>
                                ${doctor.services?.map(service => 
                                    `<option value="${service.id}" data-price="${service.price}" data-duration="${service.duration}">
                                        ${service.title} - ${service.price} сом (${service.duration} мин)
                                    </option>`
                                ).join('') || '<option value="">Нет доступных услуг</option>'}
                            </select>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Выберите дату:</label>
                            <input type="date" class="form-control" id="appointment-date" required>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Выберите время:</label>
                            <select class="form-control" id="appointment-time" required>
                                <option value="">Выберите время</option>
                                <option value="09:00">09:00</option>
                                <option value="10:00">10:00</option>
                                <option value="11:00">11:00</option>
                                <option value="12:00">12:00</option>
                                <option value="14:00">14:00</option>
                                <option value="15:00">15:00</option>
                                <option value="16:00">16:00</option>
                                <option value="17:00">17:00</option>
                            </select>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Комментарий (необязательно):</label>
                            <textarea class="form-control" id="appointment-notes" rows="3" placeholder="Опишите причину обращения или особые пожелания"></textarea>
                        </div>
                        <div class="appointment-summary" id="appointment-summary" style="display: none;">
                            <h4>Сводка записи:</h4>
                            <p><strong>Услуга:</strong> <span id="summary-service"></span></p>
                            <p><strong>Дата:</strong> <span id="summary-date"></span></p>
                            <p><strong>Время:</strong> <span id="summary-time"></span></p>
                            <p><strong>Стоимость:</strong> <span id="summary-price"></span></p>
                        </div>
                        <div class="text-center">
                            <button type="submit" class="btn btn-primary">Записаться на прием</button>
                        </div>
                    </form>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        // Обработка формы
        const form = document.getElementById('appointment-form');
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            this.createAppointment(doctor);
        });

        // Устанавливаем минимальную дату (завтра)
        const dateInput = document.getElementById('appointment-date');
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        dateInput.min = tomorrow.toISOString().split('T')[0];
        
        // Обработчики для обновления сводки
        const serviceSelect = document.getElementById('appointment-service');
        const timeSelect = document.getElementById('appointment-time');
        const summaryDiv = document.getElementById('appointment-summary');
        
        function updateSummary() {
            const selectedService = serviceSelect.options[serviceSelect.selectedIndex];
            const selectedDate = dateInput.value;
            const selectedTime = timeSelect.value;
            
            if (selectedService.value && selectedDate && selectedTime) {
                document.getElementById('summary-service').textContent = selectedService.textContent.split(' - ')[0];
                document.getElementById('summary-date').textContent = new Date(selectedDate).toLocaleDateString('ru-RU');
                document.getElementById('summary-time').textContent = selectedTime;
                document.getElementById('summary-price').textContent = selectedService.dataset.price + ' сом';
                summaryDiv.style.display = 'block';
            } else {
                summaryDiv.style.display = 'none';
            }
        }
        
        serviceSelect.addEventListener('change', updateSummary);
        dateInput.addEventListener('change', updateSummary);
        timeSelect.addEventListener('change', updateSummary);
    }

    async createAppointment(doctor) {
        const serviceSelect = document.getElementById('appointment-service');
        const selectedService = serviceSelect.options[serviceSelect.selectedIndex];
        const date = document.getElementById('appointment-date').value;
        const time = document.getElementById('appointment-time').value;
        const notes = document.getElementById('appointment-notes').value;

        if (!selectedService.value) {
            this.showAlert('Пожалуйста, выберите услугу', 'error');
            return;
        }

        if (!date || !time) {
            this.showAlert('Пожалуйста, выберите дату и время', 'error');
            return;
        }

        try {
            // Показываем загрузку
            this.showLoading();

            // Создаем запись через API
            const appointmentData = {
                doctorId: doctor.id,
                serviceId: selectedService.value,
                appointmentDate: new Date(date + 'T' + time + ':00'),
                appointmentTime: time,
                notes: notes || null
            };

            const result = await window.authAPI.request('/appointments', {
                method: 'POST',
                body: JSON.stringify(appointmentData)
            });

            this.hideLoading();
            this.closeModal();
            this.showAlert('Запись успешно создана! Врач свяжется с вами для подтверждения.', 'success');

            // Обновляем локальные записи для совместимости
            const appointment = {
                id: result.id,
                doctorId: doctor.id,
                doctorName: doctor.fullName,
                serviceId: selectedService.value,
                serviceTitle: selectedService.textContent.split(' - ')[0],
                servicePrice: parseInt(selectedService.dataset.price),
                serviceDuration: parseInt(selectedService.dataset.duration),
                date: date,
                time: time,
                notes: notes,
                status: 'pending',
                createdAt: new Date().toISOString()
            };

            const appointments = JSON.parse(localStorage.getItem('patientAppointments') || '[]');
            appointments.push(appointment);
            localStorage.setItem('patientAppointments', JSON.stringify(appointments));

        } catch (error) {
            console.error('Appointment creation error:', error);
            this.hideLoading();
            
            if (error.message.includes('401') || error.message.includes('Unauthorized')) {
                this.showAlert('Необходимо войти в систему для записи к врачу', 'error');
                this.showLoginModal();
            } else {
                this.showAlert('Ошибка при создании записи. Попробуйте еще раз.', 'error');
            }
        }
    }

    showLoginModal() {
        const modal = document.createElement('div');
        modal.className = 'modal show';
        modal.innerHTML = `
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title">Вход в систему</h3>
                    <button class="modal-close">&times;</button>
                </div>
                <div class="modal-body">
                    <form id="login-form">
                        <div class="form-group">
                            <label class="form-label">Email:</label>
                            <input type="email" class="form-control" id="login-email" required>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Пароль:</label>
                            <input type="password" class="form-control" id="login-password" required>
                        </div>
                        <div class="text-center">
                            <button type="submit" class="btn btn-primary">Войти</button>
                            <a href="pages/register.html" class="btn btn-outline">Регистрация</a>
                        </div>
                    </form>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        const form = document.getElementById('login-form');
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            this.login();
        });
    }

    async login() {
        const email = document.getElementById('login-email').value;
        const password = document.getElementById('login-password').value;

        if (!email || !password) {
            this.showAlert('Пожалуйста, заполните все поля', 'error');
            return;
        }

        try {
            this.showLoading();
            const result = await window.authAPI.login(email, password);

            if (result.success) {
                this.currentUser = result.user;
                this.closeModal();
                this.showAlert('Добро пожаловать!', 'success');
                this.updateAuthUI();
            } else {
                this.showAlert(result.errors?.[0] || 'Ошибка входа', 'error');
            }
        } catch (error) {
            console.error('Login error:', error);
            this.showAlert('Ошибка при входе в систему', 'error');
        } finally {
            this.hideLoading();
        }
    }

    logout() {
        window.authAPI.logout();
        this.currentUser = null;
        this.updateAuthUI();
        this.showAlert('Вы вышли из системы', 'info');
    }

    async checkAuth() {
        try {
            console.log('Checking auth...');
            // Проверяем токен
            const isValid = await window.authAPI.validateToken();
            console.log('Token validation result:', isValid);
            if (isValid) {
                const user = await window.authAPI.getCurrentUser();
                console.log('Current user from API:', user);
                this.currentUser = user;
                this.updateAuthUI();
            } else {
                // Проверяем сохраненного пользователя в localStorage (fallback)
                const savedUser = localStorage.getItem('currentUser');
                console.log('Saved user from localStorage:', savedUser);
                if (savedUser) {
                    this.currentUser = JSON.parse(savedUser);
                    this.updateAuthUI();
                }
            }
        } catch (error) {
            console.warn('Auth check failed:', error);
            // Fallback на localStorage
            const savedUser = localStorage.getItem('currentUser');
            if (savedUser) {
                this.currentUser = JSON.parse(savedUser);
                this.updateAuthUI();
            }
        }
    }

    updateAuthUI() {
        const navAuth = document.querySelector('.nav-auth');
        if (!navAuth) return;

        if (this.currentUser) {
            const userName = this.currentUser.fullName || this.currentUser.name || 'Пользователь';
            navAuth.innerHTML = `
                <span>Привет, ${userName}!</span>
                <button class="btn btn-outline" onclick="app.logout()">Выйти</button>
            `;
            localStorage.setItem('currentUser', JSON.stringify(this.currentUser));
        } else {
            navAuth.innerHTML = `
                <a href="pages/login.html" class="btn btn-outline">Войти</a>
                <a href="pages/register.html" class="btn btn-primary">Регистрация</a>
            `;
            localStorage.removeItem('currentUser');
        }
    }

    closeModal() {
        const modal = document.querySelector('.modal');
        if (modal) {
            modal.remove();
        }
    }

    showLoading() {
        const loading = document.createElement('div');
        loading.className = 'loading-overlay';
        loading.innerHTML = '<div class="loading"></div>';
        loading.id = 'loading-overlay';
        document.body.appendChild(loading);
    }

    hideLoading() {
        const loading = document.getElementById('loading-overlay');
        if (loading) {
            loading.remove();
        }
    }

    showAlert(message, type = 'info') {
        const alert = document.createElement('div');
        alert.className = `alert alert-${type}`;
        alert.textContent = message;
        
        // Добавляем в начало страницы
        const container = document.querySelector('.container');
        if (container) {
            container.insertBefore(alert, container.firstChild);
            
            // Удаляем через 5 секунд
            setTimeout(() => {
                alert.remove();
            }, 5000);
        }
    }
}

// Инициализация приложения
const app = new DentistQueueApp();

// Service Worker регистрация
if ('serviceWorker' in navigator) {
    window.addEventListener('load', () => {
        navigator.serviceWorker.register('/sw.js')
            .then(registration => {
                console.log('SW registered: ', registration);
            })
            .catch(registrationError => {
                console.log('SW registration failed: ', registrationError);
            });
    });
}
