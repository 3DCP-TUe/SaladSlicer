// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Drawing;
// Grasshopper Libs
using Grasshopper.Kernel;

namespace SaladSlicer
{
    public class SaladSlicerInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get { return "Salad Slicer"; }
        }
        public override Bitmap Icon
        {
            get { return Properties.Resources.SaladLogo_Icon2; }
        }
        public override string Description
        {
            get { return ""; }
        }
        public override Guid Id
        {
            get {return new Guid("D2501031-EEEB-471D-B266-63B0C4565E52"); }
        }

        public override string AuthorName
        {
            get { return "3DCP Research Group at Eindhoven University of Technology"; }
        }
        public override string AuthorContact
        {
            get { return ""; }
        }
    }
}