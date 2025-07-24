# Timescaler-WebAPI
- **Платформа:** .NET 9
- **Веб-фреймворк:** ASP.NET Core 9
- **База данных:** PostgreSQL 17
- **ORM:** Entity Framework Core 9
- **Документация API:** Swagger
- **Валидация:** FluentValidation
- **Логирование:** Serilog
- **Тестирование:** xunit
## Руководство пользователя
### Копирование репозитория
Склонируйте репозиторий на ваш компьютер, выполнив следующую команду:
```
git clone https://github.com/blugsam/Timescaler-WebAPI
```
### Создание Postgres контейнера
1. **Настройка данных окружения.**
Для запуска docker контейнера с базой данных PostgreSQL необходимо создать файл с данными окружения.
В корневой папке проекта создайте файл с именем .env и добавьте в него следующие значения:
```
DB_USER=timescaler_user
DB_PASSWORD=timescaler_pass
DB_NAME=timescaler_db
```
2. **Сборка и запуск контейнера.**
Выполните команду в корневой папке проекта, где находится файл docker-compose.yml:
```
docker-compose up
```
### Настройка и запуск приложения.
1. **Настройка строки подключения.** Откройте файл src/Timescaler.API/appsettings.Development.json и убедитесь, что строка подключения настроена для работы с локально запущенным контейнером...
```
{
  // Другие настройки
  },
  "ConnectionStrings": {
    "TimescalerDbContext": "Username=username;Password=password;Host=localhost;Database=database;"
  }
}
```
...где:
* Username, Password, Database должны совпадать с данными из вашего .env файла.
* Host должен быть localhost, так как ваше приложение будет обращаться к порту, проброшенному Docker на вашу локальную машину.
2. **Применение миграций EntityFramework Core.**
Чтобы создать таблицы в бд, необходимо применить миграции.
Откройте терминал в корневой папке проекта и выполните команду:
```
dotnet ef database update --project src/Timescaler.Infrastructure --startup-project src/Timescaler.API
```
Прим.: Если у вас не установлен dotnet-ef, установите его глобально: dotnet tool install --global dotnet-ef
### Тестирование.
Документация Swagger будет доступна по адресу:
```
http://localhost:5212/swagger/
```
## Возможности API
### Основные эндпоинты
* `POST /api/import/upload` Принимает на вход .csv, после чего происходит обработка и сохранение данных файла
в БД. Файл парсится, значения записываются в базу в таблицу Values.
* `GET /api/results` Возвращает пагинированный список записей из таблицы Results, подходящих под фильтры.
* `GET /api/results/{fileName}/last-values` Возвращает последние 10 записей по названию файла.
### Валидация
Принимая на вход .csv приложение проводит его валидацию:
* дата не может быть позже текущей и раньше 01.01.2000
* время выполнения не может быть меньше 0
* значение показателя не может быть меньше 0
* количество строк не может быть меньше 1 и больше 10 000
* значения должны соответствовать своим типам, отсутствие одного из значений в записи недопустимо.
### Вычисления 
Из значений файла подсчитываются интегральные результаты и записываются в таблицу Results:
* дельта времени Date в секундах (максимальное Date – минимальное Date)
* минимальное дата и время, как момент запуска первой операции (Date)
* среднее время выполнения (ExecutionTime)
* среднее значение по показателям (Value)
* медина по показателям (Value)
* максимальное значение показателя (Value)
* минимальное значение показателя (Value)
## Пример работы
## 1 кейс - полностью валидный файл, включающий 30 строк
1. POST /api/import/upload

    Логирование:

    <img width="707" height="77" alt="image" src="https://github.com/user-attachments/assets/cc015456-4041-43e5-af82-ac152de137b1" />

    Response body (code 200):

    `File 'valid.csv' successfully processed.`
2. GET /api/results/{fileName}/last-values

    Логирование:

    <img width="782" height="30" alt="image" src="https://github.com/user-attachments/assets/7042645d-dd03-457a-86f2-3a2b195e1b36" />

    Response body (code 200):

    ```
    [
      {
        "id": "2e96b39e-3fe1-473f-85fd-89f15e07620f",
        "date": "2023-07-22T14:15:45.6789Z",
        "executionTime": 17.89,
        "value": 40.94
      },
      {
        "id": "c147f152-2533-4527-a821-2b1643c8839c",
        "date": "2023-07-20T09:30:00.5678Z",
        "executionTime": 16.789,
        "value": 85.83
      },
      ...
      {
        "id": "8b5bafee-9cd3-49be-a3ab-c6cb6f60ae47",
        "date": "2021-01-15T05:20:35.7777Z",
        "executionTime": 15.678,
        "value": 50.06
      },
      {
        "id": "72b84500-aef3-4d59-824b-3d4fbb138033",
        "date": "2020-08-30T13:45:50.6666Z",
        "executionTime": 14.567,
        "value": 95.95
      }
    ]
    ```
## 2 кейс - некорректный файл, включающий 30 строк
1. POST /api/import/upload

    Логирование:

    <img width="1064" height="186" alt="image" src="https://github.com/user-attachments/assets/3104fca9-acc0-4b0a-9950-d95a52cb74a6" />

    Response body (code 400):

    ```
    {
      "title": "File validation errors",
      "errors": [
        {
          "field": "Line[4]",
          "message": "The date cannot be earlier than 01.01.2000."
        },
        {
          "field": "Line[5]",
          "message": "The 'Execution Time' field cannot be empty."
        },
        {
          "field": "Line[7]",
          "message": "The date cannot be in the future."
        },
        {
          "field": "Line[8]",
          "message": "The execution time cannot be negative."
        },
        {
          "field": "Line[10]",
          "message": "The indicator value cannot be negative."
        },
        {
          "field": "Line[17]",
          "message": "The execution time cannot be negative."
        },
        {
          "field": "Line[20]",
          "message": "The string must contain exactly 3 fields."
        },
        {
          "field": "Line[23]",
          "message": "Incorrect date format. Expected format: 'yyyy-MM-ddTHH-mm-ss.ffffZ'."
        },
    {
    ```
## GET /api/results
Логирование:

<img width="599" height="25" alt="image" src="https://github.com/user-attachments/assets/dac049ea-ccf1-4901-a954-664f2f4714e7" />

Response body (code 200):

```
{
  "items": [
    {
      "id": "f4a982b3-de57-492a-9c51-eaf9f73d50d0",
      "fileName": "444jet.csv",
      "firstOperationDate": "2007-02-12T09:15:22.5678Z",
      "timeDelta": 581886277.4322,
      "averageExecutionTime": 15.8455,
      "averageValue": 95.420333,
      "medianValue": 91.5645,
      "maxValue": 145.672,
      "minValue": 56.789
    },
    {
      "id": "bd52f47f-e3db-485f-9987-e09866874dff",
      "fileName": "luvxomea.csv",
      "firstOperationDate": "2005-08-14T11:22:33.4444Z",
      "timeDelta": 565841291.679,
      "averageExecutionTime": 14.33192307692308,
      "averageValue": 94.753231,
      "medianValue": 94.12,
      "maxValue": 120.45,
      "minValue": 67.333
    },
    {
      "id": "d08a77dc-f9f9-4018-89fa-ed2aed7dbf5f",
      "fileName": "valid.csv",
      "firstOperationDate": "2000-01-01T00:00:00.0001Z",
      "timeDelta": 743350545.6788,
      "averageExecutionTime": 15.120833333333334,
      "averageValue": 90.4967,
      "medianValue": 88.08,
      "maxValue": 180.72,
      "minValue": 0.001
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 3,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```
## Тестирование
Все разработанные тесты для ключевой бизнес-логики проходят успешно

<img width="774" height="551" alt="image" src="https://github.com/user-attachments/assets/1e188a92-513e-4e88-aa86-70d9bce9ca61" />
