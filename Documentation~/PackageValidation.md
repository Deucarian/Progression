# Package Validation

## Required Unity Version

Phase 0 selected Unity `6000.3.5f1`.

## Validation Plan

1. Import `com.deucarian.progression` into a clean Unity test project through a local file reference.
2. Reference `com.deucarian.gameplay-foundation` through a local file reference.
3. Run package EditMode tests.
4. Run a compatibility harness that also references Persistence and proves snapshot-to-DTO composition.
5. Repeat tests after the initial import.
6. Record allocation measurements honestly.

## Local Package Reference

```json
"com.deucarian.gameplay-foundation": "file:C:/Repositories/Deucarian/Gameplay-Foundation",
"com.deucarian.progression": "file:C:/Repositories/Deucarian/Progression"
```

Remote publication and Package Registry updates are intentionally deferred.

## Recorded Results

- Clean project: `C:/Repositories/Deucarian/Progression-TestProject`
- Unity: `6000.3.5f1`
- Import: local file references resolved for Gameplay Foundation, Persistence, and Progression.
- Direct `-runTests`: returned code `0` but produced no result XML after import, so validation used the same `BatchEditModeTestRunner` approach as prior Deucarian package phases.
- First meaningful batch run: `result=Failed; passCount=17; failCount=1; skipCount=0; duration=0,274` due to a test expectation for lexical snapshot order.
- Final batch run: `result=Passed; passCount=18; failCount=0; skipCount=0; duration=0,266`
- Repeat batch run: `result=Passed; passCount=18; failCount=0; skipCount=0; duration=0,270`
- Hot-path allocation assertion: representative `GetBalance`, `GetTrackLevel`, `IsUnlocked`, and `GetResearchRank` loop allocated `0` bytes after warm-up inside the package test.
