namespace TimeZoneWebApi.Entities
{
    public class TimeZone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public double DifferenceToGMT { get; set; }
        public int UserId { get; set; }
    }
}
