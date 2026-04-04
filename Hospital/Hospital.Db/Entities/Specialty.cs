namespace Hospital.Db.Entities
{
    public class Specialty
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public List<Doctor> Doctors { get; set; } = [];
    }
}
