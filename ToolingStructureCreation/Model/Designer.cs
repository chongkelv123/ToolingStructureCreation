using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class Designer
    {
        private List<string> designers;
        public Designer()
        {
            designers = new List<string>() {
                "Kelvin",
                "Ong TK",
                "Ian",
                "Liew SF",
                "Lim KC"};
        }

        public List<string> GetDesigners() => designers;        
    }
}

