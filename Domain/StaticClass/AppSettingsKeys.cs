using System.ComponentModel.DataAnnotations;

namespace Domain.StaticClass
{
    public sealed class AppSettingsKeys
    {
        [Required] public static string WWWRootPath { get; set; } = string.Empty;
        [Required] public static bool IsWindows { get; set; }
    }
}
