namespace ProjectArenda.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int buildingId { get; set; }
        public Building building { get; set; }
        public int Number { get; set; }
        public int Floor { get; set; }
        public int Square { get; set; }
        public int decorationId { get; set; }
        public Decoration decoration { get; set; }
        public bool Phone { get; set; }

        public Room()
        {
            building = new Building();
            decoration = new Decoration();
            Id = 0;
            buildingId = 0;
            decorationId = 0;
        }
    }
}
