// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

namespace SaladSlicer.Gh
{
    public class SaladSlicerCategoryIcon : Grasshopper.Kernel.GH_AssemblyPriority
    {
        public override Grasshopper.Kernel.GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("Salad Slicer", Properties.Resources.SaladLogo_Icon2);
            Grasshopper.Instances.ComponentServer.AddCategoryShortName("Salad Slicer", "Salad");
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("Salad Slicer", 'S');
            return Grasshopper.Kernel.GH_LoadingInstruction.Proceed;
        }
    }
}