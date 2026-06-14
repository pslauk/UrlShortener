---
name: planning
description: >-
  Use this skill to generate a comprehensive implementation plan for a given task.
  One-shot planning: /planning <task description> → generates a comprehensive plan document in docs/plans/.
---

# One-Shot Task Planning (PLANNING ONLY)

> **⛔ CRITICAL CONSTRAINT: This prompt is for PLANNING ONLY.**
> **You MUST NOT write code, create source files, run builds, execute migrations, or implement ANY part of the plan.**
> **Your ONLY deliverable is a plan document in `docs/plans/`. Nothing else.**
> **After presenting the plan — STOP. Do not proceed to implementation under any circumstances.**
> **For execution, the user will use the `/developer` command separately.**

You received a task description directly from the user in a single message.
**DO NOT enter questionnaire/interview mode.** Take the ENTIRE user message (after `/planning`) as the task input and produce a complete plan immediately.

## Workflow

### Phase 1: Deep Research (mandatory, do NOT skip)

Before writing anything, thoroughly research the codebase.

#### Step 1a Backend Research

1. Read and analyze backend code related to {task}.
2. Search Domain level for entities and interfaces.
3. Search Infrastructure level for repositories, DbContext.
4. Search Application level for services, mappers, validators, helpers.
5. Search Presentation level for controllers, endpoints.
6. Return: list of ALL affected files, interfaces, classes, methods with brief descriptions.

#### Step 1b Frontend Research
1. Read and analyze frontend code related to {task}.
2. Search `UrlShortener/Views/` for Razor Views.
3. Search `UrlShortener/wwroot/` for static assets, css, js.
4. Return: list of ALL affected frontend files with descriptions.

#### Step 1c Test & Migration Research
1. Search for existing tests covering {affected areas}.
2. Search for database migrations related to {entities}.
3. Return: list of test files, test methods, recent migrations.

#### Step 1d Aggregate results 
   Aggregate results from all research steps into a comprehensive list of affected areas, components, and existing coverage.

1. **Identify ALL affected files** — combine results from all research steps, be exhaustive (both Server/ and Client/)
2. **Understand the dependency graph** — which projects depend on what. For example:
   - Api → Application + Container
   - Container → Domain + Application
   - Application → Domain
   - Tests → Application + Domain
3. **Identify gaps** — if any area is unclear, do targeted follow-up reads


### Phase 2: Generate Task ID

**CRITICAL:** Before creating the plan document, generate a unique incremental task ID:

1. **Read** the file `.docs/task-counter.txt` to get the current counter value
   - If file doesn't exist, start with counter = 0
2. **Increment** the counter by 1
3. **Create Task ID** in format `TASK-{counter}` (e.g., `TASK-1`, `TASK-346`)
4. **Write** the new counter value back to `.docs/task-counter.txt`
5. **Use this Task ID** in the filename and plan header

### Phase 3: Generate Plan Document

Create a Markdown file at `docs/plans/{TASK-ID}-{slug}-plan.md` where:
- `{TASK-ID}` is the generated task ID (e.g., `TASK-346`)
- `{slug}` is a kebab-case short name derived from the task (e.g., `cancel-task`, `add-retry-logic`)

Example filename: `docs/plans/TASK-346-cancel-task-plan.md`

**The plan document MUST be written in Russian (except code snippets which stay in English).**

Use the following template EXACTLY:

````markdown
# {TASK-ID}: {Название задачи}

> **Task ID:** {TASK-ID}  
> **Дата:** {current date, DD.MM.YYYY}  
> **Статус:** Черновик, ожидает ревью

---

## 1. Контекст

{Подробное описание задачи: что сейчас есть, чего не хватает, зачем это нужно.
Включить текущий flow если релевантно.}

---

## 2. Вопросы и решения

| # | Вопрос | Решение |
|---|--------|---------|
| 1 | {Вопрос, на который нужен ответ пользователя/команды} | ⏳ Ожидает ответа |
| 2 | {Вопрос о неочевидном архитектурном выборе} | ⏳ Ожидает ответа |

> **Примечание:** Если все вопросы удалось разрешить из кодовой базы и документации, напиши "Нет открытых вопросов" и укажи решения, которые ты принял самостоятельно с обоснованием.

---

## 3. Пошаговый план реализации

Каждый шаг должен быть:
- Атомарным (одно логическое действие)
- Проверяемым (можно убедиться что сделано)
- С указанием конкретных файлов и изменений
- С обязательным шагом обновления документации после реализации
- С примерами кода "было → стало" где это помогает пониманию

### Шаг N. {Краткое название}

**Файл(ы):** `путь/к/файлу`

**Что сделать:**
{Подробное описание изменений}

**Пример кода (если применимо):**
```csharp
// Было:
...
// Стало:
...
```

---

## 4. Юнит-тесты

Для каждого шага, затрагивающего логику, описать какие тесты нужны:

| # | Тест | Что проверяет | Файл теста |
|---|------|---------------|------------|
| 1 | `{TestMethodName}` | {Что именно проверяется} | `Server/{SolutionName}.Tests/{path}` |

### Тестовые сценарии

- **Happy path:** {описание}
- **Edge cases:** {описание}
- **Error handling:** {описание}

---

## 5. Затронутые файлы

### Новые файлы
| Файл | Назначение |
|------|------------|
| `путь/к/файлу` | {зачем создаётся} |

### Изменяемые файлы
| Файл | Что меняется |
|------|-------------|
| `путь/к/файлу` | {описание изменений} |

---

## 6. Definition of Done (Критерии готовности)

Задача считается завершённой ТОЛЬКО когда ВСЕ пункты выполнены:

- [ ] {Конкретный критерий 1 — например: "API эндпоинт POST /api/... возвращает 200"}
- [ ] {Конкретный критерий 2 — например: "Статус в БД корректно переходит X → Y"}
- [ ] {Конкретный критерий 3}
- [ ] Все юнит-тесты из раздела 4 написаны и проходят
- [ ] Проект собирается без ошибок (`dotnet build`)
- [ ] Нет регрессий в существующих тестах
- [ ] Документация обновлена согласно изменениям (минимум: описание нового/изменённого поведения и затронутых интерфейсов)
- [ ] {Дополнительные критерии специфичные для задачи}

---

## 7. Acceptance Criteria (Критерии приёмки)

Критерии со стороны продукта/заказчика — что должен видеть/получить пользователь:

- [ ] {Пользовательский сценарий 1 — например: "Пользователь нажимает кнопку X и видит Y"}
- [ ] {Пользовательский сценарий 2}
- [ ] {Пользовательский сценарий 3}

---

## 8. Non-goals / Out of Scope (За рамками задачи)

Явно перечислить, что НЕ входит в эту задачу, чтобы план не расползался:

- ❌ {Что-то, что можно было бы сделать, но не нужно сейчас}
- ❌ {Смежная фича, которая будет отдельной задачей}
- ❌ {Оптимизация/рефакторинг не связанный с задачей}

---

## 9. Rollback Plan (План отката)

### Миграции БД
{Есть ли миграции? Если да — обратимы ли они? Скрипт отката.}

### Feature Flags
{Нужны ли фичефлаги для безопасного включения/отключения?}

### Откат изменений
{Как откатить изменения если что-то пойдёт не так после деплоя}

---

## 10. Риски и зависимости

| # | Риск / Зависимость | Вероятность | Влияние | Митигация |
|---|---------------------|-------------|---------|-----------|
| 1 | {Описание риска} | Высокая/Средняя/Низкая | {Что случится} | {Как предотвратить/смягчить} |
| 2 | {Внешняя зависимость} | — | {Что блокируется} | {Как обойти} |

### Зависимости от внешних ресурсов
- **Секреты/ключи:** {нужны ли новые секреты}
- **Внешние сервисы:** {зависимости от API третьих сторон}
- **Доступы:** {нужны ли новые доступы к чему-либо}

---
*Создано AI Planning Workflow*
````

### Phase 4: Present the Plan

After creating the file:

1. **Show the Task ID prominently** (e.g., "✅ Создан план **TASK-346**")
2. **Show the user** a brief summary of what was planned
3. **Highlight open questions** from section 2 that need answers
4. **State the file path** where the full plan is saved
5. **Ask:** "Есть вопросы или правки к плану? Дальше можно запустить `/my-plan-review` (ревью) или `/developer` (выполнение)."

### ⛔ STOP HERE — Do NOT Implement

**Your work is DONE after Phase 4.**

- Do NOT write or modify any source code files.
- Do NOT create entities, services, controllers, components, or any implementation files.
- Do NOT run `dotnet build`, `dotnet ef migrations add`, or any build/migration commands.
- Do NOT start executing plan steps.

**For plan execution**, the user will separately invoke the `/my-run` command.
**For plan review**, the user can invoke `/my-plan-review`.

## Critical Rules

1. **⛔ PLANNING ONLY** — produce ONLY the plan document. NEVER write implementation code, create source files, or execute any plan steps. Your sole output is the `.md` plan file and a summary message.
2. **ONE message, ONE plan** — no back-and-forth questionnaire. Generate everything at once.
3. **Research FIRST** — always read the codebase before writing the plan. Never guess about file paths, interfaces, or current behavior.
4. **Be specific** — every step must reference concrete files, methods, types. No vague "update the service" — say which service, which method, what exactly changes.
5. **Code examples** — include "before/after" code snippets for non-trivial changes.
6. **Russian language** for the plan document. English only for code snippets and technical identifiers.
7. **Agent-friendly steps** — write steps so that an AI coding agent can execute them unambiguously. Each step should be self-contained with enough context.
8. **Conservative scope** — actively use "Non-goals" section to prevent scope creep. When in doubt, mark it out of scope.
9. **Test coverage** — every logic change MUST have corresponding unit tests described.
10. **Rollback safety** — if there are DB migrations, ALWAYS describe the rollback migration. If changes are risky, suggest feature flags.
11. **Use existing patterns** — follow the project's conventions that already exists in the project. Don't invent new patterns.
12. **STOP after presenting** — after showing the plan summary and next-step options (`/my-plan-review` or `/developer`), your turn is OVER. Do not continue to execution automatically.






