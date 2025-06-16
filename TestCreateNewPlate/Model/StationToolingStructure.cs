using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCreateNewPlate.Model
{
    public class StationToolingStructure
    {
        Dictionary<string, double> plateThicknesses;

        public StationToolingStructure()
        {
            plateThicknesses = new Dictionary<string, double>
            {
                { "LOWER_PAD", 25.0 },
                { "DIE_PLATE", 35.0 },
                { "mat_thk", 1.55 }, // Material thickness, not a plate
                { "STRIPPER_PLATE", 30.0 },
                { "BOTTOMING_PLATE", 16.0 },
                { "PUNCH_HOLDER", 30.0 },
                { "UPPER_PAD", 27.0 }
            };
        }
        public Dictionary<string, double> GetPlateThicknesses()
        {
            return plateThicknesses;
        }

        public double GetTotalThickness()
        {
            double totalThickness = 0.0;
            foreach (var plate in plateThicknesses)
            {
                if (!plate.Key.Equals("mat_thk", StringComparison.OrdinalIgnoreCase))
                {
                    totalThickness += plate.Value;
                }
            }
            return totalThickness;
        }
    }
}
