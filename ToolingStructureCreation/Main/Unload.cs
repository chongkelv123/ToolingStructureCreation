using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ToolingStructureCreation.Main
{
    public partial class MyProgram
    {
        // Terminate piont
        public static int GetUnloadOption(string args)
        {
            int unloadOption;
            unloadOption = System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);         //After executing
            //unloadOption = System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);     //When NX session terminates
            //unloadOption = System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);        //Using File-->Unload

            return unloadOption;
        }
    }
}
