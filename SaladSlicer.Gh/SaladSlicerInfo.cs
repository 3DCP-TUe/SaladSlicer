// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Drawing;
// Grasshopper Libs
using Grasshopper.Kernel;

namespace SaladSlicer.Gh
{
    /// <summary>
    /// Contains metadata for the Salad Slicer Grasshopper plugin, 
    /// including name, description, icon, version, and author information.
    /// Used by Grasshopper to display plugin details.
    /// </summary>
    public class SaladSlicerInfo : GH_AssemblyInfo
    {
        /// <summary>
        /// The display name of the plugin.
        /// </summary>
        public override string Name
        {
            get { return "Salad Slicer"; }
        }

        /// <summary>
        /// The icon shown in the Grasshopper component ribbon.
        /// </summary>
        public override Bitmap Icon
        {
            get { return Properties.Resources.SaladLogo_Icon2; }
        }

        /// <summary>
        /// A short description of the plugin's purpose.
        /// </summary>
        public override string Description
        {
            get { return ""; }
        }

        /// <summary>
        /// The unique identifier of the plugin assembly.
        /// </summary>
        public override Guid Id
        {
            get { return new Guid("D2501031-EEEB-471D-B266-63B0C4565E52"); }
        }

        /// <summary>
        /// The name of the plugin author or organization.
        /// </summary>
        public override string AuthorName
        {
            get { return "3DCP Research Group at Eindhoven University of Technology"; }
        }

        /// <summary>
        /// Contact information for support or inquiries.
        /// </summary>
        public override string AuthorContact
        {
            get { return ""; }
        }
    }
}