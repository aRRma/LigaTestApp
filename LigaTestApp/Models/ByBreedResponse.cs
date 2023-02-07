using System.ComponentModel.DataAnnotations;

namespace LigaTestApp.Models
{
    internal class ByBreedResponse
    {
        [Required]
        public string Status { get; init; }

        [Required]
        public List<string> Message { get; init; }
    }
}
