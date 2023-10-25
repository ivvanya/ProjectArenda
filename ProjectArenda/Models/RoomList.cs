namespace ProjectArenda.Models
{
    public class RoomList
    {
        public Room room { get; set; }
        public Contract contract { get; set; }
        public Objective objective { get; set; }
        public int RentalPeriod { get; set; }
        public int Price { get; set; }

        public RoomList()
        {
            contract = new Contract();
            room = new Room();
            objective = new Objective();
        }
    }
}
