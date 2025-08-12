using NXOpen.Layout2d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Domain.ValueObjects
{
    public sealed class DrawingCode: IEquatable<DrawingCode>
    {
        private static readonly Regex DrawingCodePattern = new Regex(@"^([A-Z0-9]+)-(\d{4})-(\d{4})?", RegexOptions.Compiled);
        public string Prefix { get; }
        public string Suffix { get; }
        public string Code { get; }
        public string FullCode { get; }

        public DrawingCode(string prefix, string suffix, string code)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Prefix cannot be null or empty.", nameof(prefix));
            if (string.IsNullOrWhiteSpace(suffix))
                throw new ArgumentException("Suffix cannot be null or empty.", nameof(suffix));
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be null or empty.", nameof(code));

            Prefix = NormalizePrefix(prefix);
            Suffix = ValidateSuffix(suffix);
            Code = ValidateCode(code);
            FullCode = $"{Prefix}-{Suffix}-{Code}";
        }

        public DrawingCode(string fullCode)
        {
            if (string.IsNullOrWhiteSpace(fullCode))
                throw new ArgumentException("Full code cannot be null or empty.", nameof(fullCode));

            var match = DrawingCodePattern.Match(fullCode.Trim());
            if (!match.Success)
                throw new ArgumentException($"Invalid drawing code format: {fullCode}");

            Prefix = match.Groups[1].Value;
            Suffix = match.Groups[2].Value;
            Code = match.Groups[3].Value;
            FullCode = fullCode.Trim();
        }
        
        private static string NormalizePrefix(string prefix)
        {
            var normalized = prefix.Trim().ToUpperInvariant();

            // Business rule: Normalize prefix to 6 characters
            if (normalized.Length > 6)
            {
                return normalized.Substring(0, 6);
            }else if (normalized.Length < 6)
            {
                return normalized.PadRight(6, 'X');
            }
            return normalized;
        }
        private static string ValidateSuffix(string suffix)
        {
            var trimmed = suffix.Trim();
            if (trimmed.Length != 4 || !IsNumeric(trimmed))
                throw new ArgumentException("Suffix must be exactly 4 digits.", nameof(suffix));

            return trimmed;
        }

        private static string ValidateCode(string code)
        {
            var trimmed = code.Trim();
            if(trimmed.Length != 4 || !IsNumeric(trimmed))
                throw new ArgumentException("Code must be exactly 4 digits.", nameof(code));

            return trimmed;
        }

        private static bool IsNumeric(string value)
        {
            return int.TryParse(value, out _);
        }
        public int GetStationNumber()
        {
            // Business rule: First 2 digits of code represent station number
            var stationPart = Code.Substring(0, 2);
            return int.Parse(stationPart);
        }
        public int GetTypeCode()
        {
            // Business rule: Last 2 digits of cod represent type
            var typePart = Code.Substring(2, 2);
            return int.Parse(typePart);
        }
        public DrawingCode WithCode(string newCode)
        {
            return new DrawingCode(Prefix, Suffix, newCode);
        }
        public DrawingCode NextInSequence()
        {
            var currentCode = int.Parse(Code);
            var nextCode = currentCode + 1;
            return new DrawingCode(Prefix, Suffix, nextCode.ToString("D4"));
        }
        public bool Equals(DrawingCode other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(FullCode, other.FullCode, StringComparison.OrdinalIgnoreCase);
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as DrawingCode);
        }
        public override int GetHashCode()
        {
            return FullCode?.GetHashCode() ?? 0;
        }
        public static bool operator ==(DrawingCode left, DrawingCode right) => Equals(left, right);
        public static bool operator !=(DrawingCode left, DrawingCode right) => !Equals(left, right);
        public override string ToString() => FullCode;        
    }
}
