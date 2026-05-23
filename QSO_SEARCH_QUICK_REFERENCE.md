# QSO Search Feature - Quick Reference Guide

## User Guide

### How to Use QSO Search

#### 1. Open Search Form
- Click "Search QSOs" button on Server tab
- Modal dialog opens

#### 2. Enter Search Callsign
- Type callsign in "Search Callsign:" textbox
- Text auto-converts to UPPERCASE as you type
- **Minimum 3 [A-Z0-9] characters required**
- Invalid characters are rejected automatically

#### 3. (Optional) Apply Filters
- **Mode Group**: Select from dropdown (CW, SSB, DATA, etc.)
- **Band**: Select from dropdown (80m, 40m, 20m, etc.)
- Dropdowns auto-populate from search results
- Filters apply to current search results

#### 4. Search
- Press **Enter** key or click **Search** button
- Search is disabled until ≥3 valid characters entered
- Results appear in grid below (max 200 rows)

#### 5. Sort Results
- Click any column header to sort
- Click again to toggle ascending/descending
- All 8 columns are sortable

#### 6. View Details
- Grid is read-only (no editing)
- Scroll horizontally to see all columns
- Use scrollbar for results

### Search Patterns

| Pattern | Example | Result |
|---------|---------|--------|
| Simple callsign | `N0CALL` | `%N0CALL%` (contains) |
| Start with | `N0%` | `N0%` (starts with N0) |
| Exact pattern | `%N0CALL%` | Same (already wildcarded) |
| Contains | `_0C%` | `_0C%` (wildcard match) |

### Valid Characters
✅ **Allowed**: `A-Za-z0-9/_%$`
❌ **Not Allowed**: Special characters (!, @, #, space, -, +, etc.)

### Keyboard Shortcuts
- **Enter**: Execute search
- **Backspace**: Delete character
- **Tab**: Move to next field
- **Escape**: Close form (if supported by OS)

---

## Developer Guide

### Database Methods

#### SearchQsosByCall
```csharp
public List<Dictionary<string, object?>> SearchQsosByCall(
    string callPattern,      // e.g., "%N0CALL%"
    string? modeGroup = null, // e.g., "CW"
    string? band = null,      // e.g., "40m"
    int maxResults = 200
)
```

**Returns**: List of dictionaries with these keys:
- `call` - Callsign (string)
- `qso_time` - QSO time (DateTime)
- `mode_group` - Mode group (string)
- `mode` - Mode (string)
- `band` - Band (string)
- `freq` - Frequency (string)
- `operator` - Operator (string)
- `source_ip_address` - Source IP (string)

#### GetDistinctModeGroups
```csharp
public List<string> GetDistinctModeGroups()
```
**Returns**: Sorted list of unique mode groups

#### GetDistinctBands
```csharp
public List<string> GetDistinctBands()
```
**Returns**: Sorted list of unique bands

### Form Methods

#### UpdateSearchButtonState
```csharp
private void UpdateSearchButtonState()
```
- Counts [A-Z0-9] characters in textbox
- Enables button if count >= 3
- Called after each KeyPress

#### callSearchTextBox_KeyPress
```csharp
private void callSearchTextBox_KeyPress(KeyPressEventArgs e)
```
- Validates character against `[A-Za-z0-9\/%_$]`
- Converts lowercase to uppercase
- Rejects invalid characters

### Running Tests

```powershell
# All tests
dotnet test

# Database tests only
dotnet test --filter "DbRepositoryQsoSearchTests"

# Validation tests only
dotnet test --filter "QsoSearchInputValidationTests"

# Specific test
dotnet test --filter "SearchQsosByCall_WithExactMatch_ReturnsMatchingResults"

# Verbose output
dotnet test --verbosity detailed
```

### Key Files

| File | Lines | Purpose |
|------|-------|---------|
| `QsoSearchForm.cs` | 290 | Form logic |
| `QsoSearchForm.Designer.cs` | 200 | UI layout |
| `DbRepository.cs` | 120 | Search methods |
| `DbRepositoryQsoSearchTests.cs` | 280 | Database tests |
| `QsoSearchInputValidationTests.cs` | 240 | Validation tests |

### Database Query

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

### Database Index

```sql
CREATE INDEX idx_qsodata_call ON qsodata(call);
```

---

## Input Validation Rules

### Character Counting
Only `[A-Z0-9]` characters count toward the 3-character minimum.

| Input | Valid Count | Button State |
|-------|-------------|--------------|
| `N0` | 2 | ❌ Disabled |
| `N0C` | 3 | ✅ Enabled |
| `N0%` | 2 | ❌ Disabled |
| `N0C%` | 3 | ✅ Enabled |
| `%_%` | 0 | ❌ Disabled |

### Character Conversion
- Lowercase letters → Uppercase (e.g., `n` → `N`)
- Numbers → Unchanged
- Special chars → Unchanged
- Invalid chars → Rejected

### Pattern Building
- If input has no `%` or `_`: wrap with `%` (e.g., `N0CALL` → `%N0CALL%`)
- If input has `%` or `_`: use as-is (e.g., `N0%` → `N0%`)

---

## Troubleshooting

### Search Button Disabled
**Problem**: Search button won't enable
**Solution**: Ensure you have ≥3 characters matching [A-Z0-9]
- Valid: `N0C`, `ABC`, `123`
- Invalid: `N0`, `A%`, `1_2`

### No Results Found
**Problem**: Search returns 0 results
**Solution**: Try these steps
1. Check callsign spelling (auto-converted to uppercase)
2. Remove filters and search again
3. Use wildcard patterns: `N0%`, `%CALL%`
4. Search for single call that definitely exists

### Results Limited to 200
**Problem**: See "200+ results" message
**Solution**: Refine search pattern
1. Add filters (Mode Group, Band)
2. Use more specific pattern (e.g., `W5XYZ` instead of `%`)
3. Add wildcard constraints (e.g., `%/0` instead of `%`)

### Invalid Characters Rejected
**Problem**: Characters get rejected when typing
**Solution**: Only use allowed characters
- ✅ A-Z, 0-9, /, %, _, $
- ❌ Everything else

---

## Architecture Diagram

```
┌─────────────────────────────────────────┐
│         MainForm (Server Tab)           │
│  +────────────────────────────────────+  │
│  │  [Search QSOs] Button              │  │
│  └────────────────────────────────────┘  │
└────────────────┬──────────────────────────┘
                 │ Click → ShowDialog()
                 ▼
┌─────────────────────────────────────────┐
│       QsoSearchForm (Modal)              │
│  ┌───────────────────────────────────┐   │
│  │ Search Panel  │ Filters Panel     │   │
│  │ [TextBox]  [] │ [ModeGroup] [Band]│   │
│  │ [Search Button]                   │   │
│  └───────────────────────────────────┘   │
│  ┌───────────────────────────────────┐   │
│  │ Results DataGridView (8 columns)  │   │
│  │ Max 200 rows, sortable, read-only │   │
│  └───────────────────────────────────┘   │
└────────────────┬──────────────────────────┘
                 │ dbRepository.SearchQsosByCall()
                 ▼
┌─────────────────────────────────────────┐
│      DbRepository (Data Layer)           │
│  SearchQsosByCall()                     │
│  GetDistinctModeGroups()                │
│  GetDistinctBands()                     │
└────────────────┬──────────────────────────┘
                 │ SQL Queries
                 ▼
┌─────────────────────────────────────────┐
│      SQLite Database                    │
│  qsodata table with idx_qsodata_call    │
└─────────────────────────────────────────┘
```

---

## Performance Tips

1. **Use Specific Patterns**
   - `W5XYZ` is faster than `%` (matches fewer rows)
   - `N0%` is faster than `%0%`

2. **Apply Filters**
   - Reduces result set before display
   - Narrows down dropdowns

3. **Sort After Search**
   - UI handles sorting efficiently
   - Use column headers to sort

4. **Close Form When Done**
   - Frees up form resources
   - Button allows reopening

---

## API Reference

### IDbRepository Interface

```csharp
// Search for QSOs by callsign pattern
List<Dictionary<string, object?>> SearchQsosByCall(
    string callPattern,
    string? modeGroup = null,
    string? band = null,
    int maxResults = 200
);

// Get all distinct mode groups for filtering
List<string> GetDistinctModeGroups();

// Get all distinct bands for filtering
List<string> GetDistinctBands();
```

### QsoSearchForm

**Properties**:
- `searchResultsDataTable` - DataTable holding results
- `currentSortColumn` - Last sorted column
- `isAscending` - Sort direction

**Public Methods**:
- (None - form is internal)

**Events**:
- Form opens/closes as modal dialog
- Events handled internally

---

## Common Code Snippets

### Open Search Form
```csharp
new QsoSearchForm(dbRepository).ShowDialog(this);
```

### Search from Code
```csharp
var results = dbRepository.SearchQsosByCall("%N0CALL%", "CW", "80m");
```

### Get Filter Values
```csharp
var modeGroups = dbRepository.GetDistinctModeGroups();
var bands = dbRepository.GetDistinctBands();
```

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-02-13 | Initial release |

---

## Support

### Documentation
- 📄 `QSO_SEARCH_FEATURE_DOCUMENTATION.md` - Full technical docs
- 📄 `QSO_SEARCH_FEATURE_TESTS_README.md` - Testing guide
- 📄 `QSO_SEARCH_IMPLEMENTATION_SUMMARY.md` - Implementation details

### Tests
- 11 database layer tests
- 9 input validation tests
- All located in `..\QSOCollector.Tests\`

### Code
- Well-commented implementation
- Follows existing project patterns
- Consistent with project style

---

**Last Updated**: 2026-02-13
**Status**: ✅ Production Ready
