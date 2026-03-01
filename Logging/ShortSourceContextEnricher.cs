using Serilog.Core;
using Serilog.Events;

namespace QSOCollector.Logging
{
    public class ShortSourceContextEnricher : ILogEventEnricher
    {
        private readonly int _maxLength;
        private readonly string _propertyName;

        public ShortSourceContextEnricher(int maxLength = 30, string propertyName = "ShortSourceContext")
        {
            _maxLength = Math.Max(8, maxLength);
            _propertyName = propertyName;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (!logEvent.Properties.TryGetValue("SourceContext", out var scValue))
            {
                var prop = propertyFactory.CreateProperty(_propertyName, "");
                logEvent.AddPropertyIfAbsent(prop);
                return;
            }

            string full = scValue.ToString().Trim('\"');
            string shorted = Shorten(full, _maxLength);
            var se = propertyFactory.CreateProperty(_propertyName, shorted);
            logEvent.AddPropertyIfAbsent(se);
        }

        private static string Shorten(string fullName, int maxLength)
        {
            if (string.IsNullOrEmpty(fullName)) return fullName;
            if (fullName.Length <= maxLength) return fullName;

            var parts = fullName.Split('.');
            if (parts.Length == 1)
            {
                // single token, just trim end
                return TruncateEnd(fullName, maxLength);
            }

            string last = parts.Last(); // keep last (type name) intact preferentially
            var prefixes = parts.Take(parts.Length - 1).ToArray();

            // Strategy:
            // 1) try full prefixes + last
            // 2) if too long, abbreviate prefixes left-to-right to 2 chars
            // 3) if still too long, abbreviate prefixes left-to-right to 1 char
            // 4) if still too long, truncate from the start of the first part

            string Compose(string[] pfx)
            {
                return string.Join(".", pfx.Concat(new[] { last }));
            }

            // 1) full
            string candidate = Compose(prefixes);
            if (candidate.Length <= maxLength) return candidate;

            // make copies and transform
            string[] twoChars = prefixes.Select(p => p.Length <= 2 ? p : p.Substring(0, 2)).ToArray();
            candidate = Compose(twoChars);
            if (candidate.Length <= maxLength) return candidate;

            string[] oneChar = prefixes.Select(p => p.Length <= 1 ? p : p.Substring(0, 1)).ToArray();
            candidate = Compose(oneChar);
            if (candidate.Length <= maxLength) return candidate;

            // fallback: ensure last + abbreviated prefixes maybe truncated front of first prefix
            // keep last intact, create a base like "X.Y.Z.Last" and then truncate the leftmost side if needed
            candidate = Compose(oneChar);
            if (candidate.Length > maxLength)
            {
                // truncate from the very left to fit maxLength, but keep last segment intact
                int keep = maxLength - (last.Length + 1); // +1 for the '.' separator
                if (keep <= 0)
                {
                    // cannot keep prefixes, return truncated last
                    return TruncateEnd(last, maxLength);
                }
                // compose a single string of prefixes joined by '.' and trim to keep chars from the right
                string prefixJoin = string.Join(".", oneChar);
                if (prefixJoin.Length <= keep)
                {
                    return prefixJoin + "." + last;
                }
                // take last `keep` chars of prefixJoin, but ensure starts on a segment boundary if possible
                string trimmedPrefix = prefixJoin.Substring(prefixJoin.Length - keep);
                // ensure we don't start with '.'; if so, drop it
                if (trimmedPrefix.StartsWith(".")) trimmedPrefix = trimmedPrefix.TrimStart('.');
                return trimmedPrefix + "." + last;
            }

            return candidate;
        }

        private static string TruncateEnd(string s, int max)
        {
            if (s.Length <= max) return s;
            if (max <= 3) return s.Substring(0, max);
            return s.Substring(0, max - 3) + "...";
        }
    }
}