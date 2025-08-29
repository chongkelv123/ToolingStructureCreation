using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Services
{
    public class UserAction
    {
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; }
        public string Details { get; set; }
    }
}
