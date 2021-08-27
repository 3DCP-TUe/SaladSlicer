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
using SaladSlicer.Core.Slicers;
using SaladSlicer.Gh.Parameters.Slicers;

namespace SaladSlicer.Gh.Components.Slicers
{
    /// <summary>
    /// Represent a component that creates a 2.5D slicer object.
    /// </summary>
    public class OpenPlanar2DSlicerComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public OpenPlanar2DSlicerComponent()
          : base("Open Planar 2.5D Slicer", // Component name
              "2.5D", // Component nickname
              "Defines a slicer object for a open 2.5D object.", // Description
              "Salad Slicer", // Category
              "Slicers") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Base contour as a Curve.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "Distance between frames as a Number", GH_ParamAccess.item, 20.0);
            pManager.AddNumberParameter("Heights", "H", "Layer heights a list with Numbers.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_OpenPlanar2DSlicer(), "Slicer Object", "SO", "2.5D Slicer object.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            Curve contour = new Line().ToNurbsCurve();
            double distance = 20.0;
            List<double> heights = new List<double>();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref contour)) return;
            if (!DA.GetData(1, ref distance)) return;
            if (!DA.GetDataList(2, heights)) return;

            // Check input values
            if (distance <= 0.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The distance between two frames cannot be smaller than or equal to zero."); }
            if (contour.GetLength() < distance) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The distance between two frames exceeds the length of the base contour."); }

            // Create the slicer object
            OpenPlanar2DSlicer slicer = new OpenPlanar2DSlicer(contour, distance, heights);
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
            get { return Properties.Resources.OpenPlanar2DSlicer_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F7D61760-DFCB-41FF-B2FC-49677C25B48E"); }
        }
    }
}