namespace ProjectArenda.Models
{
    public class Periodicity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Periodicity()
        {
            Id = 0;
        }
    }
}
