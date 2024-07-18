

namespace Cambios.Modelos
{
    public class Rates
    {
        public int RateID { get; set; }

        public string Code { get; set; }

        public double TaxRate { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
