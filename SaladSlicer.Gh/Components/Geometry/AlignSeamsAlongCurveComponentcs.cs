// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// Using Salad Slicer Libs
using SaladSlicer.Geometry.Seams;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that set a seam along a guiding curve.
    /// </summary>
    public class AlignSeamsAlongCurveComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public AlignSeamsAlongCurveComponent()
          : base("Align Seams Along Curve", // Component name
              "ASAC", // Component nickname
              "Set the start points of set with closed curves as the closest point to a guiding curve.", // Description
              "Salad Slicer", // Category
              "Geometry") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Closed curves as a list with Curves", GH_ParamAccess.list);
            pManager.AddCurveParameter("Guide", "G", "Guiding curve as a Curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Closed curves as a list with Curves.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            List<Curve> curves = new List<Curve>();
            Curve guide = null;

            // Access the input parameters individually. 
            if (!DA.GetDataList(0, curves)) return;
            if (!DA.GetData(1, ref guide)) return;

            // Declare the output variables
            List<Curve> result = new List<Curve>();

            // Create the new curves
            try
            {
                result = Locations.AlignSeamsAlongCurve(curves, guide);
            }
            catch (WarningException w)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, w.Message);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }

            // Assign the output parameters
            DA.SetDataList(0, result);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
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
            get { return Properties.Resources.AlignSeamsCurve_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("AE351ACB-0DA4-4F48-BC9B-91BB44114B7C"); }
        }
    }
}