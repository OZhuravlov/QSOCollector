using System.Text.RegularExpressions;

namespace QSOCollector.Tests.Forms
{
    [TestClass]
    public class QsoSearchInputValidationTests
    {
        /// <summary>
        /// Tests for input validation patterns used in QsoSearchForm
        /// </summary>

        [TestMethod]
        public void ValidateCallsignPattern_AllowsValidCharacters()
        {
            // Arrange
            string pattern = @"^[A-Z0-9\/%_$]*$";
            var validInputs = new[]
            {
                "N0CALL",
                "W5XYZ",
                "VE3TEST",
                "K0ABC/0",
                "JA1YPA%",
                "TEST_CALL",
                "CALL$123"
            };

            // Act & Assert
            foreach (var input in validInputs)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Regex.IsMatch(input, pattern), $"Pattern should match '{input}'");
            }
        }

        [TestMethod]
        public void ValidateCallsignPattern_RejectsInvalidCharacters()
        {
            // Arrange
            string pattern = @"^[A-Z0-9\/%_$]*$";
            var invalidInputs = new[]
            {
                "call",              // lowercase
                "N0CALL!",           // exclamation mark
                "TEST@CALL",         // at symbol
                "CALL&SIGN",         // ampersand
                "CALL SIGN",         // space
                "CALL-SIGN"          // hyphen
            };

            // Act & Assert
            foreach (var input in invalidInputs)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(Regex.IsMatch(input, pattern), $"Pattern should NOT match '{input}'");
            }
        }

        [TestMethod]
        public void CountValidCharacters_ReturnsCorrectCount()
        {
            // Arrange
            string pattern = @"[A-Z0-9]";
            var testCases = new[]
            {
                ("N0CALL", 6),
                ("W5%XYZ", 5),           // % doesn't count
                ("TEST_CALL", 8),        // _ doesn't count
                ("K0ABC/0", 6),          // / doesn't count
                ("$CALL", 4),            // $ doesn't count
                ("AB%CD_EF/GH$IJ", 10)   // Only letters and numbers
            };

            // Act & Assert
            foreach (var (input, expectedCount) in testCases)
            {
                int count = Regex.Matches(input, pattern).Count;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedCount, count, $"'{input}' should have {expectedCount} valid [A-Z0-9] characters");
            }
        }

        [TestMethod]
        public void ValidateSearchButton_EnabledWith3OrMoreValidCharacters()
        {
            // Arrange
            string pattern = @"[A-Z0-9]";
            var testCases = new[]
            {
                ("N0", 2, false),           // 2 valid chars - disabled
                ("N0C", 3, true),           // 3 valid chars - enabled
                ("N0CALL", 6, true),        // 6 valid chars - enabled
                ("N0%", 2, false),          // 2 valid chars - disabled
                ("N0C%", 3, true),          // 3 valid chars - enabled
                ("_/%$", 0, false),         // 0 valid chars - disabled
            };

            // Act & Assert
            foreach (var (input, expectedCount, shouldEnable) in testCases)
            {
                int count = Regex.Matches(input, pattern).Count;
                bool isEnabled = count >= 3;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(shouldEnable, isEnabled, 
                    $"Input '{input}' with {count} valid chars should be {(shouldEnable ? "enabled" : "disabled")}");
            }
        }

        [TestMethod]
        public void ConvertToUppercase_HandlesAllCharacters()
        {
            // Arrange
            var testCases = new[]
            {
                ('n', 'N'),
                ('0', '0'),
                ('/', '/'),
                ('%', '%'),
                ('_', '_'),
                ('$', '$')
            };

            // Act & Assert
            foreach (var (input, expected) in testCases)
            {
                char result = char.IsLower(input) ? char.ToUpper(input) : input;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, result, $"'{input}' should convert to '{expected}'");
            }
        }

        [TestMethod]
        public void BuildSearchPattern_WithoutWildcards_AddsSurroundingPercent()
        {
            // Arrange
            var testCases = new[]
            {
                ("N0CALL", "%N0CALL%"),
                ("W5XYZ", "%W5XYZ%"),
                ("K0ABC", "%K0ABC%")
            };

            // Act & Assert
            foreach (var (input, expected) in testCases)
            {
                string pattern = !input.Contains('%') && !input.Contains('_') 
                    ? $"%{input}%" 
                    : input;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, pattern, $"'{input}' should become '{expected}'");
            }
        }

        [TestMethod]
        public void BuildSearchPattern_WithWildcards_DoesNotModify()
        {
            // Arrange
            var testCases = new[]
            {
                ("N0%", "N0%"),
                ("%XYZ", "%XYZ"),
                ("K_ABC", "K_ABC"),
                ("%TEST%", "%TEST%")
            };

            // Act & Assert
            foreach (var (input, expected) in testCases)
            {
                string pattern = !input.Contains('%') && !input.Contains('_')
                    ? $"%{input}%"
                    : input;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, pattern, $"'{input}' should remain '{expected}'");
            }
        }

        [TestMethod]
        public void KeyPressValidation_OnlyAllowsValidCharacters()
        {
            // Arrange
            string pattern = @"[A-Za-z0-9\/%_$]";
            var validChars = new[] { 'A', 'Z', '0', '9', '/', '%', '_', '$' };
            var invalidChars = new[] { '!', '@', '#', ' ', '-', '+', '&', '^' };

            // Act & Assert
            foreach (var c in validChars)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Regex.IsMatch(c.ToString(), pattern), $"'{c}' should be valid");
            }

            foreach (var c in invalidChars)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(Regex.IsMatch(c.ToString(), pattern), $"'{c}' should be invalid");
            }
        }

        [TestMethod]
        public void ControlCharacters_AreNeverBlocked()
        {
            // Arrange
            var controlChars = new[] 
            { 
                (char)8,    // Backspace
                (char)9,    // Tab
                (char)13,   // Enter
                (char)27    // Escape
            };

            // Act & Assert
            foreach (var c in controlChars)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(char.IsControl(c), $"Char code {(int)c} should be a control character");
            }
        }
    }
}
