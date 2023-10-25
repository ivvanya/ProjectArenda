namespace ProjectArenda.Models
{
    public class Objective
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Objective()
        {
            Id = 0;
        }
    }
}
