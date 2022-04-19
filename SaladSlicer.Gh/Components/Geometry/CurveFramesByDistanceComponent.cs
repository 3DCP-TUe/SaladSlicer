// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
using SaladSlicer.Geometry;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that gets the curve frames by distance. 
    /// </summary>
    public class CurveFramesByDistanceComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public CurveFramesByDistanceComponent()
          : base("Curve Frames by Distance", // Component name
              "CFD", // Component nickname
              "Gets the frames of a curve by distance.", // Description
              "Salad Slicer", // Category
              "Geometry") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to divide.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "Distance as a number.", GH_ParamAccess.item, 20);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Frames", "F", "Frames as list with Planes", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            Curve curve = null;
            double distance = 20.0;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetData(1, ref distance)) return;

            // Create the frames
            List<Plane> frames = Frames.GetFramesByDistanceAndSegment(curve, distance, true, true);

            // Assign the output parameters
            DA.SetDataList(0, frames);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
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
            get { return Properties.Resources.CurveFramesByDistance_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2F41ED5E-033B-43EB-9463-25411505ADFD"); }
        }

    }
}