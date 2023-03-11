using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models
{
	public class VillaCreateDTO
	{
        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = String.Empty;
        public string Details { get; set; } = String.Empty;
        public double Rate { get; set; }
        public int Occupancy { get; set; }
        public int Sqft { get; set; }
        public string ImageUrl { get; set; } = String.Empty;
        public string Amenity { get; set; } = String.Empty;

    }
}

