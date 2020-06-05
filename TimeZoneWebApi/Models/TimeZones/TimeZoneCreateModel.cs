using System.ComponentModel.DataAnnotations;

namespace TimeZoneWebApi.Models.TimeZones
{
    public class TimeZoneCreateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public double DifferenceToGMT { get; set; }

        public int UserId { get; set; }
    }
}