// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer libs
using SaladSlicer.Utils;

namespace SaladSlicer.Gh.Components.Utilities
{
    /// <summary>
    /// Represents the component gives info about the pluging (license, authors etc.)
    /// </summary>
    public class InfoComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public InfoComponent()
          : base("Info", // Component name
              "I", // Component nickname
              "Gives info about the plugin.", // Description
              "Salad Slicer", // Category
              "Utilities") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // This component has no input parameters
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "I", "Authors and license information.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare info
            string text = "";
            text += "Salad Slicer";
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "Salad Slicer is an open source project that is developed and initiated by the 3D Concrete Printing Research Group at Eindhoven University of Technology. The technical development is executed by the PhD and PDEng candidates.";
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "-----------------------------------";
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "Copyright(c) 2021-2025 3D Concrete Printing Research Group at Eindhoven University of Technology";
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "Salad Slicer is free software; you can redistribute it and / or modify it under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation. Salad Slicer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.";
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "You should have received a copy of the GNU General Public License along with Salad Slicer; If not, see http://www.gnu.org/licenses/.";
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "@license GPL-3.0 https://www.gnu.org/licenses/gpl-3.0.html";
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "-----------------------------------";
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "More information can be found here: https://github.com/3DCP-TUe/SaladSlicer";
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "-----------------------------------";
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "Version " + HelperMethods.GetVersionNumber();

            // Assign the output parameters
            DA.SetData(0, text);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.septenary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the 24x24 pixels icon.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.Info_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6F91A88F-DC67-4D2D-8F11-C865AA96CD6F"); }
        }
    }
}