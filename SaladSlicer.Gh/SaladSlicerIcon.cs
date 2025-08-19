// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

namespace SaladSlicer.Gh
{
    /// <summary>
    /// Derive from this class if you wish to perform additional steps before any of your components are loaded. 
    /// Any class in your project which inherits from GH_AssemblyPriority and which has an empty constructor
    /// will be instantiated prior to any GH_Component or IGH_DocumentObject classes.
    /// </summary>
    public class SaladSlicerCategoryIcon : Grasshopper.Kernel.GH_AssemblyPriority
    {
        /// <summary>
        /// This method will be called exactly once before any of the Components in your project are loaded.
        /// </summary>
        /// <returns> Loading instruction. </returns>
        public override Grasshopper.Kernel.GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("Salad Slicer", Properties.Resources.SaladLogo_Icon2);
            Grasshopper.Instances.ComponentServer.AddCategoryShortName("Salad Slicer", "Salad");
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("Salad Slicer", 'S');
            return Grasshopper.Kernel.GH_LoadingInstruction.Proceed;
        }
    }
}