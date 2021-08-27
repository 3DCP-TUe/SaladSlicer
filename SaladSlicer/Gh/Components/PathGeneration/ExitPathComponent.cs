// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Core.CodeGeneration;
using SaladSlicer.Core.Slicers;
using SaladSlicer.Gh.Parameters.CodeGeneration;
using SaladSlicer.Core.PathGeneration;
using SaladSlicer.Gh.Parameters.Slicers;

namespace SaladSlicer.Gh.Components.PathGeneration
{
    /// <summary>
    /// Represent a component that gets the start and end frames
    /// </summary>
    public class ExitPathComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public ExitPathComponent()
          : base("Get End Frames", // Component name
              "EF", // Component nickname
              "Defines the start and end frame of a sliced object.", // Description
              "Salad Slicer", // Category
              "Slicers") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_SlicerObject(), "Program Object", "PO", "Slicer object.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "R", "Path radius.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Direction", "D", "Exit path direction.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Exit path", "S", "Start frame as a Plane.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            ISlicer slicer = new ClosedPlanar2DSlicer();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref slicer)) return;

            // Assign the output parameters
            DA.SetData(0, slicer.FrameAtStart);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; } /// set to primary when published
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
            get { return Properties.Resources.endFrames_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8866BB69 - 89B2 - 4549 - 8D73 - FB28D67B064D"); }
        }
    }
}