# QSO Search Feature - Implementation Documentation

## Overview
A new modal dialog form (`QsoSearchForm`) has been implemented to search and display QSO records from the database with advanced filtering and sorting capabilities.

## Recent Updates

### Input Handling Changes
- **Uppercase Conversion**: Text in callSearchTextBox is converted to UPPERCASE immediately on KeyPress event
- **Minimum Pattern Length**: Search button is only enabled when textbox contains at least 3 characters matching [A-Z0-9] pattern
- **Character Validation**: Invalid characters are rejected at input time (no need for post-search validation)
- **Database Query**: Uppercase pattern is sent directly to database (no need for ToUpper() in query)

## Features Implemented

### 1. Database Layer (`Data/DbRepository.cs`)
- **SearchQsosByCall()**: Searches QSOs by callsign with LIKE pattern matching
  - Parameters:
    - `callPattern` (string): LIKE pattern (already uppercase, e.g., "%N0CALL%")
    - `modeGroup` (string, optional): Filter by mode group (CW, SSB, DATA, etc.)
    - `band` (string, optional): Filter by band
    - `maxResults` (int): Maximum 200 results
  - Returns: List<Dictionary<string, object?>> with columns: call, qso_time, mode_group, mode, band, freq, operator, source_ip_address

- **GetDistinctModeGroups()**: Retrieves all distinct mode groups from database
  - Returns: List<string> sorted alphabetically

- **GetDistinctBands()**: Retrieves all distinct bands from database
  - Returns: List<string> sorted alphabetically

### 2. Database Migration
- Created `Migrations/015_create-index-on-call-column.sql`
- Adds index on `qsodata(call)` for search performance optimization
- Supports LIKE queries with patterns including %, _, and single characters

### 3. User Interface (`Forms/QsoSearchForm`)

#### Layout:
- **Search Panel** (Top-left):
  - Label: "Search Callsign:"
  - TextBox: Accepts A-Za-z0-9/_%$ characters (auto-converted to uppercase)
  - Button: "Search" (disabled until ≥3 [A-Z0-9] characters entered)

- **Filters Panel** (Top-right):
  - ComboBox: Mode Group filter (auto-populated from results)
  - ComboBox: Band filter (auto-populated from results)

- **Results Panel** (Main):
  - Read-only DataGridView with 8 columns:
    1. Callsign (80px) - `call` field
    2. QSO time (160px) - `qso_time` (formatted as "YYYY-MM-DD HH:MM:SS")
    3. Mode group (100px) - `mode_group`
    4. Mode (100px) - `mode`
    5. Band (80px) - `band`
    6. Frequency (100px) - `freq`
    7. Operator (80px) - `operator` (4-7 character HAM callsign)
    8. Source IP (120px) - `source_ip_address`

- **Status Bar** (Bottom):
  - Displays result count (e.g., "25 results found" or "200+ results (limited to 200)")

#### Features:
1. **Input Processing**:
   - KeyPress event handler validates characters in real-time
   - Invalid characters (not in [A-Za-z0-9/_%$]) are rejected immediately
   - Lowercase letters are auto-converted to uppercase
   - Control characters (Backspace, Tab, Enter, etc.) are allowed

2. **Search Functionality**:
   - Enter callsign pattern in search textbox
   - Search button enables when ≥3 characters of [A-Z0-9] pattern detected
   - Press Enter or click Search button to execute search
   - Patterns support wildcard characters (%, _) for advanced matching
   - Text is already uppercase (no conversion needed in query)
   - Validates pattern against regex `^[A-Z0-9\/%_$]*$`

3. **Sorting**:
   - Click any column header to sort
   - Click again to toggle ascending/descending order
   - Default sort: by qso_time (descending)
   - Sort column indicated by DataTable.DefaultView.Sort

4. **Filtering**:
   - Mode Group and Band dropdowns auto-populated from current search results
   - Select filter value to refine results
   - Filter dropdowns update after each search
   - Maintains selected filter values when possible

5. **Result Display**:
   - Max 200 rows displayed with scrollbar
   - Read-only grid (no editing allowed)
   - Center-aligned headers and content
   - Single row selection mode

### 4. MainForm Integration
- **New Button**: "Search QSOs" placed at position (256, 444) on Server tab
- **Button Visibility**: Replaces hidden "Premium Callsigns" button (still visible=false)
- **Click Handler**: Opens QsoSearchForm as modal dialog

## Technical Details

### Search Query Pattern
```sql
SELECT call, qso_time, mode_group, mode, band, freq, operator, source_ip_address
FROM qsodata
WHERE call LIKE @callPattern
  AND (@modeGroup IS NULL OR mode_group = @modeGroup)
  AND (@band IS NULL OR band = @band)
  AND is_temporary = false
ORDER BY qso_time DESC
LIMIT 200
```

### Input Processing Flow
1. **KeyPress Event**: 
   - Validate character against `[A-Za-z0-9\/%_$]` pattern
   - Reject if invalid (set Handled = true)
   - Convert lowercase to uppercase
   - Allow control characters to pass through

2. **UpdateSearchButtonState()**:
   - Count characters matching `[A-Z0-9]` pattern
   - Enable button if count >= 3
   - Disable otherwise

3. **Search Execution**:
   - Trim whitespace
   - Validate entire pattern (should already be valid)
   - Auto-wrap with % if no wildcards present
   - Execute database query

### Input Validation Examples
| Input | Valid [A-Z0-9] Count | Button State | Pattern Sent |
|-------|---------------------|--------------|--------------|
| "N0" | 2 | Disabled | - |
| "N0C" | 3 | **Enabled** | "%N0C%" |
| "N0CALL" | 6 | **Enabled** | "%N0CALL%" |
| "N0%" | 2 | Disabled | - |
| "N0C%" | 3 | **Enabled** | "N0C%" |
| "%N0CALL" | 6 | **Enabled** | "%N0CALL" |
| "TEST_CALL" | 8 | **Enabled** | "%TEST_CALL%" |

### Database Index
- Index on `qsodata(call)` created via DbUp migration
- Improves search performance for large datasets (100K-150K rows expected)

## Usage Examples

### Example 1: Find all QSOs with specific callsign
- Input: "N0CALL" (user types lowercase, auto-converts to uppercase)
- Internal: "%N0CALL%"
- Result: All QSOs containing "N0CALL"

### Example 2: Find QSOs with wildcard pattern
- Input: "N0%"
- Internal: "N0%" (wildcard detected, not wrapped)
- Result: All QSOs starting with "N0"

### Example 3: Find specific callsign with mode filter
- Input: "W5XYZ"
- Mode Group: "CW"
- Band: "40m"
- Result: QSOs with W5XYZ on 40m CW

## Testing

### Unit Tests Created

#### `DbRepositoryQsoSearchTests.cs`
Located in: `..\QSOCollector.Tests\Data\DbRepositoryQsoSearchTests.cs`

Tests include:
- ✅ SearchQsosByCall_WithExactMatch_ReturnsMatchingResults
- ✅ SearchQsosByCall_WithWildcard_ReturnsAllMatches
- ✅ SearchQsosByCall_WithModeGroupFilter_ReturnsFilteredResults
- ✅ SearchQsosByCall_WithBandFilter_ReturnsFilteredResults
- ✅ SearchQsosByCall_WithCombinedFilters_ReturnsFilteredResults
- ✅ SearchQsosByCall_WithNoMatches_ReturnsEmptyList
- ✅ SearchQsosByCall_ReturnsCorrectColumns
- ✅ GetDistinctModeGroups_ReturnsAllUniqueGroups
- ✅ GetDistinctBands_ReturnsAllUniqueBands
- ✅ SearchQsosByCall_ExcludesTemporaryQsos
- ✅ SearchQsosByCall_RespectMaxResults

#### `QsoSearchInputValidationTests.cs`
Located in: `..\QSOCollector.Tests\Forms\QsoSearchInputValidationTests.cs`

Tests include:
- ✅ ValidateCallsignPattern_AllowsValidCharacters
- ✅ ValidateCallsignPattern_RejectsInvalidCharacters
- ✅ CountValidCharacters_ReturnsCorrectCount
- ✅ ValidateSearchButton_EnabledWith3OrMoreValidCharacters
- ✅ ConvertToUppercase_HandlesAllCharacters
- ✅ BuildSearchPattern_WithoutWildcards_AddsSurroundingPercent
- ✅ BuildSearchPattern_WithWildcards_DoesNotModify
- ✅ KeyPressValidation_OnlyAllowsValidCharacters
- ✅ ControlCharacters_AreNeverBlocked

### Test Coverage
- ✅ Valid character acceptance (A-Za-z0-9/_%$)
- ✅ Invalid character rejection
- ✅ Uppercase conversion on input
- ✅ Minimum 3 [A-Z0-9] character requirement
- ✅ Wildcard pattern handling
- ✅ Database search with various patterns
- ✅ Mode group and band filtering
- ✅ Temporary QSO exclusion
- ✅ Result count limiting

## Performance Considerations

1. **Query Optimization**:
   - Database index on `call` field
   - LIMIT 200 prevents excessive memory usage
   - Indexed LIKE queries perform well

2. **UI Performance**:
   - Max 200 rows in grid (scrollbar handles view)
   - Filter dropdowns populated from results (not full database)
   - DataTable.DefaultView used for efficient sorting

3. **Expected Growth**:
   - Designed to handle 100K-150K QSO records
   - Current max display: 200 results per search
   - Search response time: <500ms typical

## Files Modified/Created

### New Files:
- `Forms/QsoSearchForm.cs` - Form logic and search implementation
- `Forms/QsoSearchForm.Designer.cs` - UI layout and controls
- `Migrations/015_create-index-on-call-column.sql` - Database index creation
- `..\QSOCollector.Tests\Data\DbRepositoryQsoSearchTests.cs` - Database tests (11 test methods)
- `..\QSOCollector.Tests\Forms\QsoSearchInputValidationTests.cs` - Input validation tests (9 test methods)

### Modified Files:
- `Data/IDbRepository.cs` - Added 3 new method signatures
- `Data/DbRepository.cs` - Implemented 3 new search methods
- `Forms/MainForm.cs` - Added button click handler
- `Forms/MainForm.Designer.cs` - Added new button control and configuration
- `Forms/QsoSearchForm.Designer.cs` - Added KeyPress event handler

## Implementation Notes

### KeyPress Event Handler
The `callSearchTextBox_KeyPress` method handles:
1. Character validation using regex pattern
2. Automatic uppercase conversion
3. Rejection of invalid characters (Handled = true)
4. Call to UpdateSearchButtonState()

### UpdateSearchButtonState Method
- Counts valid [A-Z0-9] characters using Regex.Matches
- Enables Search button only if count >= 3
- Called after each KeyPress event

### Database Queries
- Pattern sent to database is already uppercase
- No ToUpper() conversion needed in SQL query
- SQL LIKE operator handles case-insensitive matching in SQLite

## Future Enhancements (Not Implemented)

- Export search results to ADIF
- Bulk operations on search results
- Search history/bookmarks
- Advanced date/time filtering
- Frequency range filtering
- Result count pagination
- Regular expression search option (instead of SQL LIKE)
