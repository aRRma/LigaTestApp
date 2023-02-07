using System.ComponentModel.DataAnnotations;

namespace LigaTestApp.Models
{
    internal class RandomImageResponse
    {
        [Required]
        public string Status { get; init; }

        [Required]
        public string Message { get; init; }
    }
}
