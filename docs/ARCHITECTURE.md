# DentalBooking — Документація проєкту

## Зміст
- [Огляд системи](#огляд-системи)
- [Актори](#актори)
- [Мікросервіси](#мікросервіси)
- [Технологічний стек](#технологічний-стек)
- [Архітектурна діаграма](#архітектурна-діаграма)
- [Workflows](#workflows)
- [Статусні моделі](#статусні-моделі)
- [FSM Telegram бота](#fsm-telegram-бота)
- [API ендпоінти](#api-ендпоінти)
- [Бізнес-правила](#бізнес-правила)
- [Події RabbitMQ](#події-rabbitmq)
- [Інфраструктура](#інфраструктура)
- [Структура проєкту](#структура-проєкту)

---

## Огляд системи

Система онлайн-запису для мережі стоматологічних клінік. Пацієнти взаємодіють через Telegram Bot (long polling). Адміністратори управляють клініками через REST API.

---

## Актори

| Актор | Канал | Роль |
|---|---|---|
| **Пацієнт** | Telegram Bot | Запис, перегляд, скасування, відгуки |
| **Адмін клініки** | REST API | Управління клінікою, лікарями, розкладом |
| **Супер-адмін** | REST API | Управління всією мережею клінік |
| **Лікар** | Telegram Bot / API | Перегляд свого розкладу |

---

## Мікросервіси

| Сервіс | Відповідальність | БД | Порт |
|---|---|---|---|
| **ApiGateway** | Маршрутизація, rate limiting, auth | — | 8080 |
| **ClinicService** | Клініки, лікарі, послуги, розклад, слоти | `clinic_db` | 8081 |
| **BookingService** | Записи, резервація слотів | `booking_db` | 8082 |
| **PatientService** | Реєстрація, профіль пацієнта | `patients_db` | 8083 |
| **NotificationService** | Нагадування, Hangfire | `notification_db` | 8084 |
| **TelegramBotService** | Long polling, FSM діалогів | Redis (FSM стан) | — |

---

## Технологічний стек

| Категорія | Технологія |
|---|---|
| Runtime | .NET 8 |
| Web API | ASP.NET Core 8 |
| Архітектура | Clean Architecture + CQRS (MediatR) |
| ORM | Entity Framework Core 8 |
| БД | PostgreSQL 16 |
| Кеш / FSM стан | Redis 7 |
| Message Bus | MassTransit + RabbitMQ 3 |
| API Gateway | YARP |
| Фонові задачі | Hangfire |
| Telegram | Telegram.Bot (long polling) |
| Контейнеризація | Docker + Docker Compose |
| Тести | xUnit + Moq + Testcontainers |
| Авторизація | JWT Bearer |

---

## Архітектурна діаграма

```
┌─────────────────────────────────────────────────────────────┐
│                    Telegram Bot Service                      │
│         (Worker Service, Long Polling, Redis FSM)           │
└──────────────────────────┬──────────────────────────────────┘
                           │ HTTP
┌──────────────────────────▼──────────────────────────────────┐
│                      API Gateway (YARP)                     │
│                         порт 8080                           │
└────┬──────────────┬──────────────┬──────────────┬───────────┘
     │              │              │              │
     ▼              ▼              ▼              ▼
┌─────────┐  ┌──────────┐  ┌──────────┐  ┌──────────────┐
│ Clinic  │  │ Booking  │  │ Patient  │  │Notification  │
│ Service │  │ Service  │  │ Service  │  │  Service     │
│  :8081  │  │  :8082   │  │  :8083   │  │   :8084      │
└────┬────┘  └────┬─────┘  └────┬─────┘  └──────┬───────┘
     │            │             │               │
     ▼            ▼             ▼               │
  clinic_db   booking_db   patients_db          │
                   │                            │
                   └──────────┬─────────────────┘
                              ▼
                          RabbitMQ
```

---

## Workflows

### WF-01 Реєстрація пацієнта

```
/start
  └─▶ Новий користувач?
        ├─ Так ─▶ Запит номера телефону (кнопка "Поділитись контактом")
        │           └─▶ Запит імені та прізвища
        │                 └─▶ Профіль створено → Головне меню
        └─ Ні  ─▶ Головне меню
```

**Правила:**
- Прив'язка через `telegram_chat_id` + номер телефону
- Якщо номер вже існує — прив'язати наявний профіль

---

### WF-02 Запис на прийом

```
"Записатись"
  └─▶ Вибір міста
        └─▶ Вибір клініки (назва + адреса)
              └─▶ Вибір категорії послуги
                    └─▶ Вибір послуги (назва + ціна + тривалість)
                          └─▶ Вибір лікаря або "Будь-який"
                                └─▶ Вибір дати (тільки доступні дні)
                                      └─▶ Вибір часу (вільні слоти)
                                            └─▶ Підтвердження
                                                  └─▶ Запис підтверджено ✅
```

**Правила:**
- Мінімум **2 години** до прийому для запису
- Слот блокується на **5 хвилин** під час підтвердження (hold)
- Якщо пацієнт не підтвердив за 5 хв — слот звільняється (статус Expired)
- Пацієнт не може мати 2 записи в один і той самий час

---

### WF-03 Управління записами пацієнта

```
"Мої записи"
  └─▶ Список майбутніх записів
        ├─▶ [Скасувати]
        │     └─▶ Підтвердження
        │           ├─ За > 2г  ─▶ Скасовано, слот звільнено
        │           └─ За < 2г  ─▶ "Занадто пізно для скасування"
        └─▶ [Перенести]
              └─▶ Вибір нової дати/часу (той самий лікар + послуга)
                    └─▶ Підтвердження → старий слот звільнено, новий зайнято
```

---

### WF-04 Нагадування та сповіщення

| Тригер | Повідомлення | Час |
|---|---|---|
| `AppointmentCreated` | Підтвердження запису з деталями | Негайно |
| `AppointmentCreated` | Нагадування | За 24г до прийому |
| `AppointmentCreated` | Нагадування | За 1г до прийому |
| `AppointmentCancelledByPatient` | Підтвердження скасування | Негайно |
| `AppointmentCancelledByClinic` | Повідомлення про скасування + причина | Негайно |
| `AppointmentCompleted` | Запит на відгук | Через 2г після прийому |

---

### WF-05 Відгук після прийому

```
Бот надсилає запит через 2г після прийому
  └─▶ Оцінка ⭐ 1-5 (inline кнопки)
        └─▶ Текстовий коментар (або "Пропустити")
              └─▶ Відгук збережено
                    └─▶ Оновлюється рейтинг лікаря та клініки
```

---

### WF-06 Адмін клініки

```
Управління клінікою:
  ├─▶ Редагування: адреса, телефон, опис, фото
  ├─▶ Розклад роботи клініки (дні, години)
  └─▶ Блокування клініки на дату (свято, ремонт)

Управління лікарями:
  ├─▶ Додати / архівувати лікаря
  ├─▶ Призначити послуги лікарю + тривалість прийому
  └─▶ Розклад лікаря:
        ├─▶ Шаблон тижня (Пн: 09:00-18:00, обід 13:00-14:00)
        ├─▶ Виключення (відпустка, лікарняний)
        └─▶ Генерація слотів на N тижнів вперед

Управління записами:
  ├─▶ Список записів (фільтр: дата, лікар, статус)
  ├─▶ Скасування запису з причиною (→ сповіщення пацієнту)
  └─▶ Позначити прийом як завершений
```

---

### WF-07 Генерація слотів

```
Тригери:
  ├─▶ Адмін змінив розклад лікаря
  ├─▶ Адмін додав нову послугу лікарю
  └─▶ Фоновий job (щоночі, горизонт 30 днів)

Алгоритм:
  Для кожного робочого дня лікаря:
    Крок = мінімальна тривалість послуги (15 хв)
    Від start_time до end_time з кроком:
      Якщо не перетинається з обідом і блокуваннями:
        Створити TimeSlot { status: Available }
```

---

### WF-08 Супер-адмін

```
├─▶ CRUD мережі клінік
├─▶ Призначення адмінів клінік
├─▶ Глобальний каталог послуг
└─▶ Аналітика по мережі:
      ├─▶ Завантаженість клінік
      ├─▶ Популярні послуги
      ├─▶ Кількість записів / скасувань
      └─▶ Рейтинги лікарів
```

---

## Статусні моделі

### Appointment (Запис)

```
Pending (hold 5 хв)
  ├─▶ Confirmed    — пацієнт підтвердив
  │     ├─▶ Completed          — прийом відбувся
  │     ├─▶ CancelledByPatient — пацієнт скасував
  │     └─▶ CancelledByClinic  — клініка скасувала
  └─▶ Expired      — не підтверджено за 5 хв
```

### TimeSlot (Слот)

```
Available
  ├─▶ Reserved  (hold 5 хв під час підтвердження)
  │     ├─▶ Booked     — підтверджений запис
  │     └─▶ Available  — hold закінчився / скасовано
  └─▶ Blocked   — заблоковано адміном
```

---

## FSM Telegram бота

```
Idle
├─▶ Registration
│     AwaitingPhone → AwaitingName → Idle
│
├─▶ Booking
│     SelectingCity → SelectingClinic → SelectingCategory →
│     SelectingService → SelectingDoctor → SelectingDate →
│     SelectingTime → ConfirmingBooking → Idle
│
├─▶ MyAppointments
│     ViewingList → ViewingAppointment
│       ├─▶ Cancelling → ConfirmCancel → Idle
│       └─▶ Rescheduling → SelectingDate → SelectingTime →
│                          ConfirmReschedule → Idle
│
└─▶ LeavingReview
      AwaitingRating → AwaitingComment → Idle
```

**Зберігання стану:** Redis, ключ `bot:state:{chatId}`, TTL 30 хвилин.

---

## API ендпоінти

### ClinicService

| Метод | Шлях | Доступ | Опис |
|---|---|---|---|
| GET | `/clinics` | Public | Список клінік (фільтр: місто) |
| GET | `/clinics/{id}` | Public | Деталі клініки |
| POST | `/clinics` | SuperAdmin | Створити клініку |
| PUT | `/clinics/{id}` | ClinicAdmin | Редагувати клініку |
| GET | `/clinics/{id}/services` | Public | Послуги клініки |
| GET | `/clinics/{id}/doctors` | Public | Лікарі клініки |
| GET | `/doctors/{id}` | Public | Профіль лікаря |
| GET | `/doctors/{id}/slots` | Public | Вільні слоти (`?date=&serviceId=`) |
| PUT | `/doctors/{id}/schedule` | ClinicAdmin | Оновити розклад |
| POST | `/doctors/{id}/blocks` | ClinicAdmin | Заблокувати дату |
| GET | `/services` | Public | Каталог послуг |
| POST | `/services` | SuperAdmin | Додати послугу до каталогу |

### BookingService

| Метод | Шлях | Доступ | Опис |
|---|---|---|---|
| POST | `/appointments` | Patient | Створити запис (hold 5 хв) |
| POST | `/appointments/{id}/confirm` | Patient | Підтвердити hold |
| GET | `/appointments/{id}` | Patient/Admin | Деталі запису |
| GET | `/appointments/my` | Patient | Записи поточного пацієнта |
| PUT | `/appointments/{id}/cancel` | Patient/Admin | Скасувати запис |
| PUT | `/appointments/{id}/reschedule` | Patient | Перенести запис |
| PUT | `/appointments/{id}/complete` | ClinicAdmin | Завершити прийом |
| GET | `/clinics/{id}/appointments` | ClinicAdmin | Записи клініки |

### PatientService

| Метод | Шлях | Доступ | Опис |
|---|---|---|---|
| POST | `/patients/register` | Public | Реєстрація пацієнта |
| GET | `/patients/me` | Patient | Свій профіль |
| PUT | `/patients/me` | Patient | Редагувати профіль |
| GET | `/patients/by-telegram/{chatId}` | Internal | Знайти за chatId |

### NotificationService

Тільки внутрішній сервіс. Споживає події з RabbitMQ. Публічних ендпоінтів немає.

---

## Бізнес-правила

| # | Правило |
|---|---|
| BR-01 | Записатись можна не пізніше ніж за **2 години** до прийому |
| BR-02 | Скасувати можна не пізніше ніж за **2 години** до прийому |
| BR-03 | Hold на слот діє **5 хвилин**, після чого Expired |
| BR-04 | Пацієнт не може мати 2 записи в один часовий проміжок |
| BR-05 | Мінімальний крок слоту — **15 хвилин** |
| BR-06 | Горизонт генерації слотів — **30 днів** вперед |
| BR-07 | Запис на відгук надсилається через **2 години** після завершення прийому |
| BR-08 | Нагадування: за **24 години** і за **1 годину** до прийому |
| BR-09 | Timezone: зберігається UTC, відображається у timezone клініки |
| BR-10 | При скасуванні клінікою — пацієнт отримує причину скасування |

---

## Події RabbitMQ

| Подія | Publisher | Consumer | Payload |
|---|---|---|---|
| `AppointmentCreated` | BookingService | NotificationService | appointmentId, patientId, dateTime, doctorId, clinicId |
| `AppointmentConfirmed` | BookingService | NotificationService | appointmentId |
| `AppointmentCancelledByPatient` | BookingService | NotificationService | appointmentId |
| `AppointmentCancelledByClinic` | BookingService | NotificationService | appointmentId, reason |
| `AppointmentCompleted` | BookingService | NotificationService | appointmentId, patientId |
| `AppointmentExpired` | BookingService | ClinicService | slotId (звільнити слот) |
| `PatientRegistered` | PatientService | NotificationService | patientId, chatId |
| `ScheduleUpdated` | ClinicService | ClinicService (internal) | doctorId, clinicId |

---

## Інфраструктура

### Docker Compose сервіси

| Контейнер | Image | Порти |
|---|---|---|
| `dental_postgres` | postgres:16-alpine | 5432 |
| `dental_redis` | redis:7-alpine | 6379 |
| `dental_rabbitmq` | rabbitmq:3-management-alpine | 5672, 15672 |
| `dental_api_gateway` | build | 8080 |
| `dental_clinic_service` | build | — |
| `dental_booking_service` | build | — |
| `dental_patient_service` | build | — |
| `dental_notification_service` | build | — |
| `dental_telegram_bot` | build | — |

### Бази даних

| БД | Сервіс | Таблиці (орієнтовно) |
|---|---|---|
| `clinic_db` | ClinicService | clinics, doctors, services, schedules, time_slots, doctor_services |
| `booking_db` | BookingService | appointments |
| `patients_db` | PatientService | patients |
| `notifications_db` | NotificationService | notifications, scheduled_jobs |

---

## Структура проєкту

```
DentalBooking.sln
├── docs/
│   └── ARCHITECTURE.md           ← цей файл
├── infra/
│   └── postgres/
│       └── init.sql              ← ініціалізація БД
├── src/
│   ├── Shared/
│   │   ├── Shared.Contracts/     ← DTO та події (спільні між сервісами)
│   │   └── Shared.BuildingBlocks/← BaseEntity, Result<T>, пагінація
│   ├── ApiGateway/               ← YARP
│   ├── ClinicService/
│   │   ├── ClinicService.Domain
│   │   ├── ClinicService.Application
│   │   ├── ClinicService.Infrastructure
│   │   └── ClinicService.API
│   ├── BookingService/           ← аналогічна структура
│   ├── PatientService/           ← аналогічна структура
│   ├── NotificationService/      ← аналогічна структура
│   └── TelegramBotService/       ← Worker Service
│       ├── StateMachine/         ← FSM кроки
│       ├── Handlers/             ← обробники команд
│       └── Services/             ← HTTP клієнти до API
├── tests/
│   ├── ClinicService.Tests/
│   ├── BookingService.Tests/
│   └── PatientService.Tests/
├── docker-compose.yml
├── .env                          ← секрети (в .gitignore)
└── .gitignore
```

---

## Статус реалізації

| Компонент | Статус |
|---|---|
| Solution scaffold | ✅ Готово |
| Docker Compose | ✅ Готово |
| Документація | ✅ Готово |
| Shared.BuildingBlocks | ✅ Готово |
| Shared.Contracts (події) | ✅ Готово |
| ClinicService.Domain | ✅ Готово |
| ClinicService.Application | ✅ Готово |
| ClinicService.Infrastructure | ✅ Готово |
| ClinicService.API | ✅ Готово |
| BookingService | ✅ Готово |
| PatientService | ✅ Готово |
| NotificationService | ✅ Готово |
| TelegramBotService | ✅ Готово |
| ApiGateway (YARP config) | ✅ Готово |
