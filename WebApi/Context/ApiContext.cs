using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging.Abstractions;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace WebApi.Context
{
    public class ApiContext : DbContext
    {
        private readonly NullLogger<ApiContext> _logger;

        public DbSet<MyEntity> Entities { get; set; }



        public ApiContext(DbContextOptions<ApiContext> options, IHostEnvironment env) : base(options)
        {
            _logger = NullLogger<ApiContext>.Instance;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            MyEntityMap.Map(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        
        }
    }
    public class MyEntity
    {
        public int Id { get; set; }
        public string Field { get; set; }
        public string FieldB { get; set; }
    }
    public static class MyEntityMap
    {
        public static void Map(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyEntity>(builder =>
            {
                builder.Property(p => p.Field).DefaultString(150, true, TextType.NoSpecialCharsUpper);
                builder.Property(p => p.FieldB).DefaultString(150, true, TextType.NoSpecialCharsUpper);
            });
        }

        internal static PropertyBuilder<string> DefaultString(
        this PropertyBuilder<string> builder,
        int maxLength = 256,
        bool required = false,
        TextType type = TextType.None)
        {
            return builder
                .HasMaxLength(maxLength)
                .HasConversion(
                    c => HandleTo(c, maxLength, type, required),
                    c => c)
                .IsRequired(required);
        }
        private static string HandleTo(string s, int maxLength, TextType type, bool required)
        {
            if (string.IsNullOrEmpty(s)) return required ? string.Empty : null;

            switch (type)
            {

                case TextType.NoSpecialCharsUpper:
                    s = s.RemoveSpecialCharacters().ToUpperInvariant();
                    break;
                case TextType.None:
                default:
                    break;
            }

            if (s.Length < maxLength) maxLength = s.Length;

            s = s.Substring(0, maxLength).Trim();

            return s;
        }
        public static string RemoveDiacritics(this string stIn)
        {
            if (stIn is null) return "";

            var stFormD = stIn.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var t in stFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(t);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
        public static string RemoveSpecialCharacters(this string input)
        {
            if (input is null) return null;

            input = RemoveDiacritics(input);

            input = input.Replace('-', ' ')
                         .Replace('º', ' ')
                         .Replace(',', ' ')
                         .Replace('ª', ' ')
                         .Replace('.', ' ');

            input = Regex.Replace(input, @"[^\p{L}\p{Nd}.º \u00BA]+", " ");

            return input.Trim();
        }

        internal enum TextType
        {
            None,
            NoSpecialCharsUpper,
        }
    }
}
