using NXOpen.Layout2d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class Machine
    {
        List<String> machines;
        public const string MC304 = "MC304";
        public const string MC303 = "MC303";
        public const string MC254 = "MC254";
        public const string MC302 = "MC302";
        public const string MC602 = "MC602";
        public const string MC403 = "MC403";
        public const string MC803 = "MC803";
        public const string MC1801 = "MC1801";
        public const string MC1202 = "MC1202";

        public Machine()
        {
            machines = new List<string>() { 
                Machine.MC304, 
                Machine.MC303, 
                Machine.MC254, 
                Machine.MC302, 
                Machine.MC602, 
                Machine.MC403, 
                Machine.MC803, 
                Machine.MC1801, 
                Machine.MC1202 };
            
        }
        public List<string> GetMachines()
        {
            return machines;
        }

        public CommonPlateLegacy GetCommonPlate(string machineName)
        {
            if (string.IsNullOrEmpty(machineName) || !machines.Contains(machineName))
            {
                throw new ArgumentException("Invalid machine name provided.");
            }

            double length;
            double width;
            double thickness;

            switch (machineName)
            {
                case MC304:
                    length = 2100;
                    width = 700;
                    thickness = 40;                    
                    break;
                case MC303:
                    length = 2100;
                    width = 700;
                    thickness = 40;
                    break;
                case MC254:
                    length = 2100;
                    width = 700;
                    thickness = 40;
                    break;
                case MC302:
                    length = 2100;
                    width = 700;
                    thickness = 40;
                    break;
                case MC602:
                    length = 2300;
                    width = 960;
                    thickness = 60;
                    break;
                case MC403:
                    length = 2300;
                    width = 960;
                    thickness = 60;
                    break;
                case MC803:
                    length = 2300;
                    width = 960;
                    thickness = 60;
                    break;
                case MC1801:
                    length = 2600;
                    width = 960;
                    thickness = 60;
                    break;
                case MC1202:
                    length = 2600;
                    width = 960;
                    thickness = 60;
                    break;
                default:
                    throw new ArgumentException("Invalid machine name provided.");                    
            }

            return new CommonPlateLegacy(length, width, thickness);
        }

    }
}
