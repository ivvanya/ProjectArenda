using System;

namespace ProjectArenda.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public Tenant tenant { get; set; }
        public Individual individual { get; set; }
        public Entity entity { get; set; }
        public Employee employee { get; set; }
        public Periodicity periodicity { get; set; }
        public int Penalty { get; set; }
        public string Additionaly { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Contract()
        {
            Id = 0;
            tenant = new Tenant();
            employee = new Employee();
            periodicity = new Periodicity();
            individual = new Individual();
            entity = new Entity();
        }
    }
}
