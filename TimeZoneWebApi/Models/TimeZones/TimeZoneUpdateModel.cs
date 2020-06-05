namespace TimeZoneWebApi.Models.TimeZones
{
  public class TimeZoneUpdateModel
    {
        public string Name { get; set; }
        public string City { get; set; }
        public double DifferenceToGMT { get; set; }
        public int UserId { get; set; }
    }
}