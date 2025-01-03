// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// Salad Slicer
using SaladSlicer.Geometry;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that tweens curves.
    /// </summary>
    public class InterpolateCurvesComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public InterpolateCurvesComponent()
          : base("Interpolate curves", // Component name
              "IC", // Component nickname
              "Creates a curve between two other curves with as start point the start point of the first curve and as end point the end point of the second curve.", // Description
              "Salad Slicer", // Category
              "Geometry part 1") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve 1", "C1", "The first curve.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve 2", "C2", "The second curve.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tolerance", "T", "The tolarance defined as the distance between interpolation points.", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "The interpolated curve.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            Curve curve1 = null;
            Curve curve2 = null;
            double tolerance = 1.0;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref curve1)) return;
            if (!DA.GetData(1, ref curve2)) return;
            if (!DA.GetData(2, ref tolerance)) return;

            // Result
            Curve result = null;

            // Create the new curve
            try
            {
                result = Curves.InterpolateCurves(curve1, curve2, tolerance);
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
            DA.SetData(0, result);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
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
            get { return Properties.Resources.InterpolateCurves_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("34A6B102-2A20-400C-9AD5-947C78AD5410"); }
        }

    }
}