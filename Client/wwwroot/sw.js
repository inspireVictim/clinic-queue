// Service Worker для DentistQueue PWA
const CACHE_NAME = 'dentist-queue-v1';
const urlsToCache = [
  '/',
  '/css/app.css',
  '/css/bootstrap/bootstrap.min.css',
  '/js/app.js',
  '/manifest.json',
  '/icon-192.png',
  '/icon-512.png'
];

// Install event - кеширование ресурсов
self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => {
        console.log('Service Worker: Caching files');
        return cache.addAll(urlsToCache);
      })
      .catch(err => console.log('Service Worker: Error caching files', err))
  );
});

// Activate event - очистка старых кешей
self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(cacheNames => {
      return Promise.all(
        cacheNames.map(cacheName => {
          if (cacheName !== CACHE_NAME) {
            console.log('Service Worker: Deleting old cache', cacheName);
            return caches.delete(cacheName);
          }
        })
      );
    })
  );
});

// Fetch event - обслуживание запросов
self.addEventListener('fetch', event => {
  // Стратегия Cache First для статических ресурсов
  if (event.request.destination === 'image' || 
      event.request.destination === 'style' || 
      event.request.destination === 'script') {
    event.respondWith(
      caches.match(event.request)
        .then(response => {
          return response || fetch(event.request);
        })
    );
  } else {
    // Стратегия Network First для HTML и API запросов
    event.respondWith(
      fetch(event.request)
        .then(response => {
          // Клонируем ответ, так как он может быть использован только один раз
          const responseClone = response.clone();
          
          // Кешируем только успешные ответы
          if (response.status === 200) {
            caches.open(CACHE_NAME)
              .then(cache => {
                cache.put(event.request, responseClone);
              });
          }
          
          return response;
        })
        .catch(() => {
          // Если сеть недоступна, пытаемся найти в кеше
          return caches.match(event.request)
            .then(response => {
              if (response) {
                return response;
              }
              
              // Если это HTML запрос и нет в кеше, возвращаем офлайн страницу
              if (event.request.destination === 'document') {
                return caches.match('/offline.html');
              }
            });
        })
    );
  }
});

// Push notifications
self.addEventListener('push', event => {
  if (event.data) {
    const data = event.data.json();
    
    const options = {
      body: data.body || 'У вас новое уведомление',
      icon: '/icon-192.png',
      badge: '/icon-192.png',
      tag: data.tag || 'notification',
      data: data.url || '/',
      actions: [
        {
          action: 'open',
          title: 'Открыть',
          icon: '/icon-192.png'
        },
        {
          action: 'close',
          title: 'Закрыть'
        }
      ]
    };

    event.waitUntil(
      self.registration.showNotification(data.title || 'DentistQueue', options)
    );
  }
});

// Notification click
self.addEventListener('notificationclick', event => {
  event.notification.close();

  if (event.action === 'open' || !event.action) {
    event.waitUntil(
      clients.openWindow(event.notification.data || '/')
    );
  }
});

// Background sync для офлайн записей
self.addEventListener('sync', event => {
  if (event.tag === 'appointment-sync') {
    event.waitUntil(syncAppointments());
  }
});

async function syncAppointments() {
  try {
    // Здесь будет логика синхронизации офлайн записей
    console.log('Service Worker: Syncing appointments');
    
    // Получаем данные из IndexedDB
    // Отправляем на сервер
    // Очищаем локальное хранилище
    
  } catch (error) {
    console.error('Service Worker: Error syncing appointments', error);
  }
}
