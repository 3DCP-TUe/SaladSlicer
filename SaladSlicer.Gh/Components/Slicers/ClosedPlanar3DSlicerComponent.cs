// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
//Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Slicers;
using SaladSlicer.Gh.Parameters.Slicers;

namespace SaladSlicer.Gh.Components.Slicers
{
    /// <summary>
    /// Represent a component that creates a 3D slicer object.
    /// </summary>
    public class ClosedPlanar3DSlicerComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public ClosedPlanar3DSlicerComponent()
          : base("Closed Planar 3D Slicer", // Component name
              "CP3D", // Component nickname
              "Defines a slicer object for a closed planar 3D object.", // Description
              "Salad Slicer", // Category
              "Slicers") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Base geometry as a Mesh.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Seam Location", "F", "Seam location defined as a normalized length factor of the contour.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Seam Length", "L", "Seam length as a Number.", GH_ParamAccess.item, 100.0);
            pManager.AddNumberParameter("Distance", "D", "Distance between frames as a Number", GH_ParamAccess.item, 20.0);
            pManager.AddBooleanParameter("Reverse", "R", "Reverse path direction", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Heights", "H", "Layer heights a list with Numbers.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_ClosedPlanar3DSlicer(), "Slicer Object", "SO", "3D Slicer object.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            Mesh mesh = new Mesh() ;
            double seamLocation = 0.0;
            double seamLength = 100.0;
            double distance = 20.0;
            bool reverse = false;
            List<double> heights = new List<double>();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetData(1, ref seamLocation)) return;
            if (!DA.GetData(2, ref seamLength)) return;
            if (!DA.GetData(3, ref distance)) return;
            if (!DA.GetData(4, ref reverse)) return;
            if (!DA.GetDataList(5, heights)) return;

            // Check input values
            if (seamLocation < 0.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Parameter value is not in the range of 0 to 1."); }
            if (seamLocation > 1.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Parameter value is not in the range of 0 to 1."); }
            if (distance <= 0.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The distance between two frames cannot be smaller than or equal to zero."); }

            // Create the slicer object
            ClosedPlanar3DSlicer slicer = new ClosedPlanar3DSlicer(mesh, seamLocation, seamLength, distance, reverse, heights);
            slicer.Slice();

            // Assign the output parameters
            DA.SetData(0, slicer);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
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
            get { return Properties.Resources.ClosedPlanar3DSlicer_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("09714934-F519-444F-B402-24DF3EF8DB22"); }
        }
    }
}