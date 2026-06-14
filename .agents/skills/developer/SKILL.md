---
name: developer
description: >-
  Use this skill to implement a task Plan from docs/plans/ autonomously.
  Execute a task Plan from docs/plans/ autonomously.
---

# /developer — Plan Execution

You have been asked to execute a task Plan.

## Workflow

### Phase 1: Load the Plan

1. **If user specified a task ID** — read `docs/plans/TASK-{ID}-*-Plan.md`
2. **If not specified** — look for the most recent `docs/plans/TASK-*-Plan.md` file
3. **Read the entire Plan document** carefully
4. **Confirm** execution intent: either the user explicitly asked to execute now, or the latest Plan review marked the Plan as ready.

### Phase 2: Prepare Execution

1. **Create a todo list** using the Plan with ALL steps from the Plan
2. Verify Questions: Read Plan section 2 (Вопросы и решения). Check that ALL questions are resolved. If any open questions remain, report them and STOP execution.
3. Verify Files: Read Plan section 5 (Затронутые файлы). Verify all existing files are present. Check file paths exist in the workspace. If any files are missing, report them and STOP execution.
4. Verify Dependencies: Read Plan section 10 (Риски и зависимости). Check for blocking dependencies. Verify external services/secrets are available. If any blockers are found, report them and STOP execution.
5. **Announce:** "Начинаю выполнение плана {TASK-ID}. Шагов: {N}."

### Phase 3: Execute Steps

### File-by-file execution discipline (mandatory)

Execute changes in **per-file micro-cycles** whenever possible:
1. Select **one primary file** for the current change
2. Re-read the Plan step and read that file before editing
3. Modify only that file within the current micro-cycle unless a minimal dependent edit is unavoidable
4. Immediately run **focused validation** for that change:
  - check diagnostics/errors for the affected file(s)
  - run targeted tests/build validation for the changed scope
5. Fix any errors found in the same scope before moving on
6. Only then continue to the next file or next step

Do not batch many file edits first and postpone validation until later.

#### Implementation

1. **Mark step as in-progress** in the todo list
2. **Research** — read relevant files to understand current state before making changes
3. **Implement** — make the changes described in the step
4. **Validate immediately** — run focused validation right after the change:
  - get errors/diagnostics for the affected file(s)
  - run targeted tests or the smallest relevant build/test command for the changed scope
5. **Fix before proceeding** — do not move to another file or step while the current scope still has unresolved errors or failing targeted checks
6. **Mark step as completed** in the todo list only after the step is clean

### Phase 4: Final Validation

After ALL steps are completed, make validation:

1. Build Verification: Run 'dotnet build' on the solution. **Fix any issues** found during build.
2. Definition of Done Check: Read the Plan's section 6 (Definition of Done). For each criterion, verify it against the actual codebase. Read the modified files and check each criterion. **Fix any issues** found during this review.
3. Architecture Compliance: Check all modified files for architecture violations:
   - Layer dependency rules (Api→App+Container, App→Domain, etc.)
   - Naming conventions (C# and Vue)
   - DI registrations present in Container/
   - Async methods have CancellationToken
   **Fix any issues** found during this review.
4. **Run tests:** `dotnet test` on the test project.
5. **Re-validate** if fixes were applied.

### Phase 4.1: Reporting Integrity Gate

Before reporting success:
1. Confirm every claimed successful build/test run has an explicit success marker such as a clear summary or exit code
2. If output is partial, truncated, ambiguous, or missing the final success indicator, treat it as **not yet proven**
3. Re-run the relevant validation until success or failure is explicit
4. Do not state or imply "everything is green" without direct evidence from the completed output

### Phase 5: Completion Report

Generate a summary:

```markdown
## Execution Complete: {TASK-ID}

### Steps Completed: {N}/{Total}
| Step | Status | Notes |
|------|--------|-------|
| 1. {name} | ✅/❌ | {details} |

### Build Status: ✅/❌
### Test Status: ✅/❌ ({passed}/{total} tests)

### Definition of Done:
- [x] {criterion met}
- [ ] {criterion not met — reason}

### Issues Encountered:
- {Any problems and how they were resolved}

### Files Modified:
- `path/to/file` — {what changed}
```

## Error Handling

If something goes wrong during execution:

1. **Log the error** clearly
2. **Attempt to fix** — search codebase for similar patterns, adjust approach
3. **If truly blocked:**
   - Mark the step as failed in todo list
   - Continue with independent steps that don't depend on the failed one
   - Report blockage in the completion report
4. **Never leave the codebase in a broken state** — if a step partially completes, either finish it or revert

## Critical Rules

1. **Follow the Plan** — execute EXACTLY what was described. Don't add scope, don't skip steps.
2. **Research before coding** — always read existing files before modifying them.
3. **Validate after each file/step** — use focused validation immediately and don't accumulate errors across steps.
4. **Track progress** — use the Plan religiously. Mark in-progress → completed for every step.
5. **Conventions first** — use existing patterns from the codebase.
6. **Test everything** — if the Plan includes tests, write them. If it doesn't but you added logic, consider adding tests.
7. **Never break the build** — after all steps, the project MUST compile and all tests MUST pass.
8. **Report honestly** — if something couldn't be done, say so. Don't pretend it's complete.
9. **FULL IMPLEMENTATION** — every step must be implemented COMPLETELY. No placeholders, no `// TODO`, no stubs, no skeleton code, no "remaining fields similar". If a step requires creating a service with 5 methods — implement all 5 methods with full logic. Partial implementation = step NOT completed.
10. **No premature green status** — never mark a step or the task as completed while relevant errors remain unresolved or tests are failing.
11. **Reporting integrity is mandatory** — never claim success from incomplete logs; explicit success markers are required.















