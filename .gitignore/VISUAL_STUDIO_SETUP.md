# 🚀 Настройка Visual Studio для DentistQueue

## ✅ Проблемы исправлены!

Все ошибки с библиотеками и настройками проекта были исправлены. Теперь проект должен запускаться в Visual Studio без проблем.

## 🔧 Что было исправлено:

### 1. **Настройки проекта**
- Добавлены `RootNamespace` и `AssemblyName` в `.csproj`
- Обновлены настройки запуска в `launchSettings.json`
- Добавлен дополнительный профиль для Production

### 2. **Solution файл**
- Добавлен `SolutionGuid` для корректной работы с Visual Studio
- Настроены правильные конфигурации сборки

### 3. **Program.cs**
- Добавлена поддержка CORS для разработки
- Улучшена конфигурация middleware

### 4. **Зависимости**
- Все NuGet пакеты восстановлены
- Проект собирается без ошибок

## 🎯 Как запустить в Visual Studio:

### Способ 1: Через Solution
1. Откройте файл `DentistQueue.sln` в Visual Studio
2. Убедитесь, что `DentistQueue.Web` установлен как стартовый проект
3. Нажмите **F5** или кнопку **Запуск**

### Способ 2: Через проект
1. Откройте папку `DentistQueue.Web` в Visual Studio
2. Нажмите **F5** или кнопку **Запуск**

### Способ 3: Через командную строку
```bash
cd DentistQueue.Web
dotnet run
```

## 🌐 Доступ к приложению:

После запуска приложение будет доступно по адресам:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:7001

## 🔍 Проверка работы:

1. Откройте браузер и перейдите на http://localhost:5000
2. Вы должны увидеть главную страницу DentistQueue
3. Проверьте работу поиска врачей
4. Проверьте модальные окна записи

## 🛠 Дополнительные настройки:

### Если возникают проблемы с портами:
1. Откройте `Properties/launchSettings.json`
2. Измените порты в `applicationUrl`
3. Перезапустите приложение

### Если нужны дополнительные профили:
Добавьте новые профили в `launchSettings.json`:

```json
{
  "commandName": "Project",
  "dotnetRunMessages": true,
  "launchBrowser": true,
  "applicationUrl": "https://localhost:7001;http://localhost:5000",
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```

## 📱 PWA функции:

Приложение поддерживает:
- ✅ Service Worker
- ✅ Манифест для установки
- ✅ Офлайн режим
- ✅ Push уведомления

## 🎉 Готово!

Проект полностью настроен и готов к работе в Visual Studio!
