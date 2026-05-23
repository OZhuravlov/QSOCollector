# QSO Search Feature - Testing Guide

## Overview
Comprehensive test suite for the QSO Search feature, including database functionality tests and input validation tests.

## Test Projects

### 1. DbRepositoryQsoSearchTests.cs
**Location**: `..\QSOCollector.Tests\Data\DbRepositoryQsoSearchTests.cs`

#### Purpose
Tests the database layer search functionality with an in-memory SQLite database.

#### Setup
- Creates in-memory test database
- Initializes qsodata table schema
- Creates index on call column
- Inserts 5 test QSO records before each test

#### Test Data
```
N0CALL     | 2026-02-13 20:28:30 | CW   | CW       | 80m | 3.573     | W5XYZ   | 192.168.1.1
W5XYZ      | 2026-02-13 20:29:00 | SSB  | SSB      | 40m | 7.079133  | N0CALL  | 192.168.1.2
K0ABC      | 2026-02-13 20:30:15 | DATA | AFSK45   | 20m | 14.070    | VE3TEST | 192.168.1.3
N0CALL/0   | 2026-02-13 20:31:45 | CW   | CW       | 40m | 7.035     | W5XYZ   | 192.168.1.1
VE3TEST    | 2026-02-13 20:32:00 | SSB  | USB      | 80m | 3.860     | N0CALL  | 192.168.1.4
```

#### Test Methods

1. **SearchQsosByCall_WithExactMatch_ReturnsMatchingResults**
   - Tests exact pattern matching
   - Pattern: "%N0CALL%"
   - Expected: 2 results

2. **SearchQsosByCall_WithWildcard_ReturnsAllMatches**
   - Tests wildcard pattern with %
   - Pattern: "%N0%"
   - Expected: 2 results (N0CALL, N0CALL/0)

3. **SearchQsosByCall_WithModeGroupFilter_ReturnsFilteredResults**
   - Tests mode group filtering
   - Pattern: "%", Mode Group: "CW"
   - Expected: 2 CW results

4. **SearchQsosByCall_WithBandFilter_ReturnsFilteredResults**
   - Tests band filtering
   - Pattern: "%", Band: "40m"
   - Expected: 2 results on 40m

5. **SearchQsosByCall_WithCombinedFilters_ReturnsFilteredResults**
   - Tests multiple filters together
   - Pattern: "%", Mode Group: "CW", Band: "40m"
   - Expected: 1 result (N0CALL/0)

6. **SearchQsosByCall_WithNoMatches_ReturnsEmptyList**
   - Tests non-matching pattern
   - Pattern: "%NONEXISTENT%"
   - Expected: 0 results (empty list)

7. **SearchQsosByCall_ReturnsCorrectColumns**
   - Tests that all 8 required columns are present
   - Validates column names in result dictionary

8. **GetDistinctModeGroups_ReturnsAllUniqueGroups**
   - Tests distinct mode group retrieval
   - Expected: ["CW", "DATA", "SSB"] (sorted)

9. **GetDistinctBands_ReturnsAllUniqueBands**
   - Tests distinct band retrieval
   - Expected: ["20m", "40m", "80m"] (sorted)

10. **SearchQsosByCall_ExcludesTemporaryQsos**
    - Tests that temporary QSOs are excluded
    - Adds temporary QSO and verifies it's not returned
    - Expected: 2 results (temporary excluded)

11. **SearchQsosByCall_RespectMaxResults**
    - Tests max results limit
    - Inserts 250 QSOs
    - Expected: <= 200 results

### 2. QsoSearchInputValidationTests.cs
**Location**: `..\QSOCollector.Tests\Forms\QsoSearchInputValidationTests.cs`

#### Purpose
Tests the input validation logic used in the QsoSearchForm for character validation and button state management.

#### Test Methods

1. **ValidateCallsignPattern_AllowsValidCharacters**
   - Tests that valid characters are accepted
   - Valid chars: A-Z, 0-9, /, %, _, $
   - Test inputs: N0CALL, W5XYZ, VE3TEST, K0ABC/0, JA1YPA%, TEST_CALL, CALL$123

2. **ValidateCallsignPattern_RejectsInvalidCharacters**
   - Tests that invalid characters are rejected
   - Invalid chars: lowercase, !, @, #, space, -, &, ^
   - Test inputs: call, N0CALL!, TEST@CALL, CALL&SIGN, CALL SIGN, CALL-SIGN

3. **CountValidCharacters_ReturnsCorrectCount**
   - Tests character counting logic for [A-Z0-9] pattern
   - Wildcards (%, _, /) and ($) should not be counted
   - Examples:
     - "N0CALL" → 6 valid chars
     - "W5%XYZ" → 5 valid chars
     - "TEST_CALL" → 8 valid chars

4. **ValidateSearchButton_EnabledWith3OrMoreValidCharacters**
   - Tests button enable/disable logic
   - Button enabled when count >= 3
   - Test cases: "N0" (disabled), "N0C" (enabled), "N0CALL" (enabled), etc.

5. **ConvertToUppercase_HandlesAllCharacters**
   - Tests uppercase conversion
   - Lowercase letters converted to uppercase
   - Other characters (0-9, /, %, _, $) remain unchanged

6. **BuildSearchPattern_WithoutWildcards_AddsSurroundingPercent**
   - Tests auto-wrapping with % when no wildcards present
   - "N0CALL" → "%N0CALL%"
   - "W5XYZ" → "%W5XYZ%"

7. **BuildSearchPattern_WithWildcards_DoesNotModify**
   - Tests that patterns with wildcards are not wrapped
   - "N0%" → "N0%"
   - "%XYZ" → "%XYZ"
   - "K_ABC" → "K_ABC" (underscores are wildcards in SQL)

8. **KeyPressValidation_OnlyAllowsValidCharacters**
   - Tests KeyPress event character validation
   - Valid: A-Za-z0-9/_%$
   - Invalid: !, @, #, space, -, +, &, ^

9. **ControlCharacters_AreNeverBlocked**
   - Tests that control characters are always allowed
   - Examples: Backspace (8), Tab (9), Enter (13), Escape (27)

## Running the Tests

### From Visual Studio
1. Open Test Explorer (Test > Test Explorer)
2. Build solution
3. Tests will appear in Test Explorer
4. Click "Run All" or run specific test

### From Command Line
```powershell
# Run all tests
dotnet test

# Run specific test file
dotnet test --filter "FullyQualifiedName~DbRepositoryQsoSearchTests"

# Run specific test
dotnet test --filter "FullyQualifiedName~SearchQsosByCall_WithExactMatch_ReturnsMatchingResults"

# Run with verbose output
dotnet test --verbosity detailed
```

## Test Coverage Summary

| Category | Count | Status |
|----------|-------|--------|
| Database Search Tests | 11 | ✅ Implemented |
| Input Validation Tests | 9 | ✅ Implemented |
| **Total** | **20** | **✅ Ready** |

### Coverage Areas
- ✅ Database queries (exact match, wildcards, filters, combined filters)
- ✅ Result set validation (correct columns, proper counts)
- ✅ Edge cases (no matches, temporary QSOs, max results)
- ✅ Input validation (valid/invalid characters)
- ✅ Button state management (enable/disable logic)
- ✅ Character conversion (uppercase)
- ✅ Pattern building (wildcard handling)
- ✅ Control character handling

## Test Naming Convention

Tests follow AAA pattern (Arrange-Act-Assert) and naming convention:
```
MethodName_Scenario_ExpectedOutcome
```

Example: `SearchQsosByCall_WithExactMatch_ReturnsMatchingResults`
- Method: SearchQsosByCall
- Scenario: WithExactMatch
- Outcome: ReturnsMatchingResults

## Debugging Tests

### Enable Verbose Logging
In DbRepositoryQsoSearchTests.cs Setup method:
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();
```

### Inspect Test Data
Use breakpoints in Setup() or InsertTestData() to verify:
- Table creation
- Data insertion
- Index creation

### View Result Sets
Add breakpoints in test methods to inspect:
- Returned dictionaries
- Column values
- Count and filtering

## Common Issues

### Issue: Tests fail with "table already exists"
**Solution**: Clear in-memory database or ensure each test uses TestInitialize

### Issue: Search returns no results
**Solution**: Verify test data is inserted and pattern matches. Check:
- Call values are uppercase
- Pattern is correct (e.g., "%TEST%")
- Temporary flag is 0

### Issue: Column validation fails
**Solution**: Ensure result dictionary contains all 8 required columns:
- call, qso_time, mode_group, mode, band, freq, operator, source_ip_address

## Future Test Enhancements

- [ ] Integration tests with real database
- [ ] Performance tests for large datasets (100K+ records)
- [ ] UI tests for form interactions
- [ ] Stress tests for concurrent searches
- [ ] Sort functionality tests
- [ ] Filter dropdown population tests

## Test Maintenance

- Update InsertTestData() when new test scenarios are needed
- Keep test data representative of real-world usage
- Review tests quarterly for relevance
- Add tests when new features are implemented
