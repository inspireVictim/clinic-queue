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
            // Загружаем данные с API
            const response = await fetch('/api/doctors');
            if (response.ok) {
                this.doctors = await response.json();
            } else {
                // Fallback на локальные данные
                this.loadLocalMockData();
            }
        } catch (error) {
            console.warn('Не удалось загрузить данные с API, используем локальные данные:', error);
            this.loadLocalMockData();
        }
    }

    loadLocalMockData() {
        // Локальные mock данные врачей (fallback)
        this.doctors = [
            {
                id: "1",
                fullName: "Иванов Иван Иванович",
                specializations: ["Стоматолог-терапевт"],
                rating: 4.8,
                reviewCount: 127,
                experienceYears: 8,
                photoUrl: "https://via.placeholder.com/300x300/1976d2/ffffff?text=ИИИ",
                clinic: { name: "Стоматология 'Улыбка'", city: "Москва" },
                services: this.getDoctorServices("1")
            },
            {
                id: "2",
                fullName: "Петрова Анна Сергеевна",
                specializations: ["Стоматолог-хирург"],
                rating: 4.9,
                reviewCount: 89,
                experienceYears: 12,
                photoUrl: "https://via.placeholder.com/300x300/42a5f5/ffffff?text=ПАС",
                clinic: { name: "Медицинский центр 'Здоровье'", city: "Москва" },
                services: this.getDoctorServices("2")
            },
            {
                id: "3",
                fullName: "Сидоров Михаил Петрович",
                specializations: ["Ортодонт"],
                rating: 4.7,
                reviewCount: 156,
                experienceYears: 15,
                photoUrl: "https://via.placeholder.com/300x300/66bb6a/ffffff?text=СМП",
                clinic: { name: "Клиника 'Белые зубы'", city: "Санкт-Петербург" },
                services: this.getDoctorServices("3")
            },
            {
                id: "4",
                fullName: "Козлова Елена Владимировна",
                specializations: ["Детский стоматолог"],
                rating: 4.9,
                reviewCount: 203,
                experienceYears: 10,
                photoUrl: "https://via.placeholder.com/300x300/ff7043/ffffff?text=КЕВ",
                clinic: { name: "Детская стоматология 'Радуга'", city: "Москва" },
                services: this.getDoctorServices("4")
            }
        ];
    }

    getDoctorServices(doctorId) {
        // Получаем услуги врача из localStorage
        const doctorServices = JSON.parse(localStorage.getItem('doctorServices') || '[]');
        
        // Если это врач с ID "2" (демо-врач), возвращаем его услуги
        if (doctorId === "2") {
            return doctorServices.filter(service => service.active).map(service => ({
                id: service.id,
                title: service.title,
                price: service.price,
                duration: service.duration,
                description: service.description
            }));
        }
        
        // Для остальных врачей возвращаем дефолтные услуги
        const defaultServices = {
            "1": [{ title: "Консультация", price: 1500, duration: 30 }, { title: "Лечение кариеса", price: 2500, duration: 60 }],
            "3": [{ title: "Брекеты", price: 80000, duration: 120 }, { title: "Коронка", price: 15000, duration: 90 }],
            "4": [{ title: "Детская консультация", price: 1000, duration: 30 }, { title: "Лечение молочных зубов", price: 2000, duration: 45 }]
        };
        
        return defaultServices[doctorId] || [];
    }

    renderDoctors() {
        const doctorsGrid = document.getElementById('doctors-grid');
        if (!doctorsGrid) return;

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
                <div class="doctor-price">от ${doctor.services?.[0]?.price || 0} ₽</div>
                <button class="btn btn-primary" onclick="app.showDoctorDetails('${doctor.id}')">
                    Записаться на прием
                </button>
            </div>
        `).join('');
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
                <div class="doctor-price">от ${doctor.services?.[0]?.price || 0} ₽</div>
                <button class="btn btn-primary" onclick="app.showDoctorDetails('${doctor.id}')">
                    Записаться на прием
                </button>
            </div>
        `).join('');
    }

    showDoctorDetails(doctorId) {
        const doctor = this.doctors.find(d => d.id === doctorId);
        if (!doctor) return;

        // Проверяем авторизацию
        if (!this.currentUser) {
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
                                        ${service.title} - ${service.price} ₽ (${service.duration} мин)
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
                document.getElementById('summary-price').textContent = selectedService.dataset.price + ' ₽';
                summaryDiv.style.display = 'block';
            } else {
                summaryDiv.style.display = 'none';
            }
        }
        
        serviceSelect.addEventListener('change', updateSummary);
        dateInput.addEventListener('change', updateSummary);
        timeSelect.addEventListener('change', updateSummary);
    }

    createAppointment(doctor) {
        const serviceSelect = document.getElementById('appointment-service');
        const selectedService = serviceSelect.options[serviceSelect.selectedIndex];
        const date = document.getElementById('appointment-date').value;
        const time = document.getElementById('appointment-time').value;
        const notes = document.getElementById('appointment-notes').value;

        if (!selectedService.value) {
            this.showAlert('Пожалуйста, выберите услугу', 'error');
            return;
        }

        // Показываем загрузку
        this.showLoading();

        // Создаем запись
        const appointment = {
            id: Date.now(),
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

        // Сохраняем запись в localStorage
        const appointments = JSON.parse(localStorage.getItem('patientAppointments') || '[]');
        appointments.push(appointment);
        localStorage.setItem('patientAppointments', JSON.stringify(appointments));

        // Имитируем запрос к серверу
        setTimeout(() => {
            this.hideLoading();
            this.closeModal();
            this.showAlert('Запись успешно создана! Врач свяжется с вами для подтверждения.', 'success');
        }, 2000);
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

    login() {
        const email = document.getElementById('login-email').value;
        const password = document.getElementById('login-password').value;

        // Простая проверка для демо
        if (email === 'patient@example.com' && password === '123456') {
            this.currentUser = {
                id: 1,
                name: 'Иван Иванов',
                email: email,
                role: 'patient'
            };
            
            this.closeModal();
            this.showAlert('Добро пожаловать!', 'success');
            this.updateAuthUI();
        } else {
            this.showAlert('Неверный email или пароль', 'error');
        }
    }

    logout() {
        this.currentUser = null;
        this.updateAuthUI();
        this.showAlert('Вы вышли из системы', 'info');
    }

    checkAuth() {
        // Проверяем сохраненного пользователя в localStorage
        const savedUser = localStorage.getItem('currentUser');
        if (savedUser) {
            this.currentUser = JSON.parse(savedUser);
            this.updateAuthUI();
        }
    }

    updateAuthUI() {
        const navAuth = document.querySelector('.nav-auth');
        if (!navAuth) return;

        if (this.currentUser) {
            navAuth.innerHTML = `
                <span>Привет, ${this.currentUser.name}!</span>
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
