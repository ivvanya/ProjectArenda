namespace ProjectArenda.Models
{
    public class Building
    {
        public int Id { get; set; }
        public int DistrictId { get; set; }
        public District district { get; set; }
        public string Address { get; set; }
        public int Floors { get; set; }
        public int Rooms { get; set; }
        public string ComNumb { get; set; }
        public Building()
        {
            district = new District();
            Id = 0;
            Rooms = 0;
        }
    }
}
