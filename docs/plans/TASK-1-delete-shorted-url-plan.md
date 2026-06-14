# TASK-1: Удаление сохранённого Shorted URL

> **Task ID:** TASK-1  
> **Дата:** 15.06.2026  
> **Статус:** Черновик, ожидает ревью

---

## 1. Контекст

Сейчас в приложении UrlShortener пользователь может добавлять сокращённые ссылки и переходить по ним,
но не может удалить запись. Кнопка удаления отсутствует в списке сохранённых ссылок.

Нужно добавить возможность удаления отдельной записи URL. Кнопка «Удалить» должна отображаться
рядом с каждой записью в списке ссылок и удалять запись из БД с обновлением списка без перезагрузки страницы.

**Текущий flow:**
1. Пользователь вводит URL, нажимает «Short!» — добавляется запись.
2. Список ссылок обновляется через AJAX (Unobtrusive AJAX, `data-ajax` на форме в `Index.cshtml`).
3. Удалить запись нельзя.

**Ключевые наблюдения по кодовой базе:**
- `IRepository<T>.Delete(T item)` уже реализован в `UrlRepository` — изменений в слое инфраструктуры не требуется.
- `IUrlService` не содержит метода удаления — нужно добавить.
- Список URL рендерится в `UrlsPartial.cshtml` внутри `<div id="urls">`, который находится внутри основной формы в `Index.cshtml`.
- Вложенные HTML-формы недопустимы, поэтому кнопка удаления использует `$.post()` через jQuery (jQuery уже подключён как зависимость Unobtrusive AJAX).

---

## 2. Вопросы и решения

> Нет открытых вопросов. Все решения приняты на основе анализа кодовой базы.

| # | Вопрос | Решение |
|---|--------|---------|
| 1 | Как сделать AJAX-запрос на удаление без вложенных форм? | Использовать `$.post()` через jQuery, который уже подключён как зависимость Unobtrusive AJAX. Ответ контроллера — обновлённый partial, которым заменяется `#urls`. |
| 2 | Нужна ли новая миграция? | Нет. Схема БД не меняется — удаляются только строки из существующей таблицы `Urls`. |
| 3 | Где получить сущность `Url` для удаления? | `IRepository<Url>.GetById(int id)` уже реализован в `UrlRepository`. |

---

## 3. Пошаговый план реализации

### Шаг 1. Добавить метод `DeleteUrl` в интерфейс `IUrlService`

**Файл:** `UrlShortener.Application/Interfaces/IUrlService.cs`

**Что сделать:**  
Добавить метод `void DeleteUrl(int id)` в интерфейс.

**Пример кода:**
```csharp
// Было:
public interface IUrlService
{
    IEnumerable<UrlModel> GetUrlViewModel();
    void CreateUrl(string newUserUrl);
    bool TryGetUrl(string shortedUrlPart, out Url url);
    void UpdateClickedUrl(Url url);
}

// Стало:
public interface IUrlService
{
    IEnumerable<UrlModel> GetUrlViewModel();
    void CreateUrl(string newUserUrl);
    bool TryGetUrl(string shortedUrlPart, out Url url);
    void UpdateClickedUrl(Url url);
    void DeleteUrl(int id);
}
```

---

### Шаг 2. Реализовать `DeleteUrl` в `UrlService`

**Файл:** `UrlShortener.Application/Services/UrlService.cs`

**Что сделать:**  
Добавить реализацию метода: получить сущность по `id` через `_urlsRepository.GetById`,
вызвать `_urlsRepository.Delete`, затем `_contextWorker.Commit()`.
Если сущность не найдена — выйти без действий.

**Пример кода:**
```csharp
// Добавить в класс UrlService:
public void DeleteUrl(int id)
{
    var url = _urlsRepository.GetById(id);
    if (url == null) return;

    _urlsRepository.Delete(url);
    _contextWorker.Commit();
}
```

---

### Шаг 3. Добавить action `DeleteUrl` в `UrlShortenerController`

**Файл:** `UrlShortener/Controllers/UrlShortenerController.cs`

**Что сделать:**  
Добавить `[HttpPost]` action `DeleteUrl(int id)`: вызвать `_urlService.DeleteUrl(id)`,
вернуть `PartialView("UrlsPartial", _urlService.GetUrlViewModel())` — по аналогии с action `ShortUrl`.

**Пример кода:**
```csharp
// Добавить в класс UrlShortenerController:
[HttpPost]
public ActionResult DeleteUrl(int id)
{
    _urlService.DeleteUrl(id);
    return PartialView("UrlsPartial", _urlService.GetUrlViewModel());
}
```

---

### Шаг 4. Добавить кнопку «Удалить» в `UrlsPartial.cshtml`

**Файл:** `UrlShortener/Views/UrlShortener/UrlsPartial.cshtml`

**Что сделать:**  
Внутри `<div class="url-item">` для каждого URL добавить кнопку,
которая через `$.post()` отправляет `id` на `DeleteUrl` и заменяет содержимое `#urls` ответом.

**Пример кода:**
```html
<!-- Было: -->
<div class="url-item">
    <div><b>Source url:</b> @Model[i].UserUrl</div>
    <div>
        <b>Shorted link:</b>
        <a href="...">...</a>
    </div>
    <div><b>Clicks:</b> @Model[i].Clicks</div>
</div>

<!-- Стало: -->
<div class="url-item">
    <div><b>Source url:</b> @Model[i].UserUrl</div>
    <div>
        <b>Shorted link:</b>
        <a href="@Url.Action("UserShortedUrl", "Home", new { shortedUrlPart = Model[i].ShortedUrl }, scheme, host)">
            @Url.Action("UserShortedUrl", "Home", new { shortedUrlPart = Model[i].ShortedUrl }, scheme, host)
        </a>
    </div>
    <div><b>Clicks:</b> @Model[i].Clicks</div>
    <button type="button" class="delete-button"
            onclick="$.post('@Url.Action("DeleteUrl", "UrlShortener")', { id: @Model[i].Id }, function(html){ $('#urls').html(html); })">
        Удалить
    </button>
</div>
```

---

### Шаг 5. Добавить стиль кнопки удаления в `site.css`

**Файл:** `UrlShortener/wwwroot/css/site.css`

**Что сделать:**  
Добавить стиль `.delete-button` в конец файла.

**Пример кода:**
```css
.delete-button {
    background-color: #dc3545;
    color: #fff;
    border: none;
    border-radius: 4px;
    padding: 4px 12px;
    cursor: pointer;
    margin-top: 4px;
}

.delete-button:hover {
    background-color: #b02a37;
}
```

---

## 4. Юнит-тесты

| # | Тест | Что проверяет | Файл теста |
|---|------|---------------|------------|
| 1 | `DeleteUrlShouldCallRepositoryDeleteAndCommit` | При вызове `DeleteUrl(id)` вызываются `GetById(id)`, `Delete(url)` и `Commit()` | `UnitTests/Application/UrlServiceTests.cs` |
| 2 | `DeleteUrlShouldDoNothingIfUrlNotFound` | При `GetById` возвращающем `null` метод завершается без вызова `Delete` и `Commit` | `UnitTests/Application/UrlServiceTests.cs` |

### Тестовые сценарии

- **Happy path:** URL с переданным `id` существует → `Delete` и `Commit` вызваны по одному разу.
- **Edge case (не найден):** `GetById` возвращает `null` → `Delete` и `Commit` не вызываются.

---

## 5. Затронутые файлы

### Новые файлы
Нет.

### Изменяемые файлы

| Файл | Что меняется |
|------|-------------|
| `UrlShortener.Application/Interfaces/IUrlService.cs` | Добавлен метод `void DeleteUrl(int id)` |
| `UrlShortener.Application/Services/UrlService.cs` | Добавлена реализация `DeleteUrl(int id)` |
| `UrlShortener/Controllers/UrlShortenerController.cs` | Добавлен `[HttpPost] ActionResult DeleteUrl(int id)` |
| `UrlShortener/Views/UrlShortener/UrlsPartial.cshtml` | Добавлена кнопка «Удалить» с `$.post` для каждой записи |
| `UrlShortener/wwwroot/css/site.css` | Добавлен стиль `.delete-button` |
| `UnitTests/Application/UrlServiceTests.cs` | Добавлены 2 теста для `DeleteUrl` |

---

## 6. Definition of Done (Критерии готовности)

- [ ] Интерфейс `IUrlService` содержит метод `DeleteUrl(int id)`
- [ ] `UrlService.DeleteUrl` корректно удаляет запись и вызывает `Commit`
- [ ] При несуществующем `id` метод завершается без исключения
- [ ] Контроллер возвращает обновлённый partial после удаления
- [ ] Кнопка «Удалить» видна рядом с каждой сохранённой ссылкой в UI
- [ ] Нажатие кнопки удаляет запись и обновляет список без перезагрузки страницы
- [ ] Все юнит-тесты из раздела 4 написаны и проходят
- [ ] Проект собирается без ошибок (`dotnet build UrlShortener.sln`)
- [ ] Нет регрессий в существующих тестах

---

## 7. Acceptance Criteria (Критерии приёмки)

- [ ] Рядом с каждой сохранённой ссылкой отображается кнопка «Удалить»
- [ ] После нажатия «Удалить» запись исчезает из списка без перезагрузки страницы
- [ ] Остальные записи в списке не затронуты
- [ ] Функция сокращения новых URL продолжает работать как раньше

---

## 8. Non-goals / Out of Scope (За рамками задачи)

- ❌ Подтверждение удаления (диалог «Вы уверены?»)
- ❌ Soft delete (пометка записи как удалённой вместо физического удаления)
- ❌ Удаление нескольких записей разом
- ❌ Авторизация / контроль доступа к удалению
- ❌ Рефакторинг существующего механизма добавления URL

---

## 9. Rollback Plan (План отката)

### Миграции БД
Миграций нет — схема не меняется. Откат не требуется.

### Feature Flags
Не нужны — изменение минимально и изолировано.

### Откат изменений
Достаточно отменить изменения в 6 файлах из раздела 5. Данные в БД при откате кода не восстанавливаются — удалённые записи теряются безвозвратно.

---

## 10. Риски и зависимости

| # | Риск / Зависимость | Вероятность | Влияние | Митигация |
|---|---------------------|-------------|---------|-----------|
| 1 | jQuery не загружен в момент клика | Низкая | Кнопка не работает | jQuery уже подключён через `wwwroot/lib` как зависимость Unobtrusive AJAX — гарантированно присутствует |
| 2 | Одновременное удаление несуществующей записи | Низкая | Нет (`DeleteUrl` возвращается без действий при `null`) | Обработано в реализации метода |

### Зависимости от внешних ресурсов
- **Секреты/ключи:** не нужны
- **Внешние сервисы:** нет
- **Доступы:** нет

---
*Создано AI Planning Workflow*
