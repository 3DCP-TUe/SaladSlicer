// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that tweens curves.
    /// </summary>
    public class TweenCurvesWithMatchingComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public TweenCurvesWithMatchingComponent()
          : base("Tween Curves With Matching", // Component name
              "TCWM", // Component nickname
              "Creates curves between two open or closed input curves. Make the structure of input curves compatible if needed. Refits the input curves to have the same structure. The resulting curves are usually more complex than input unless input curves are compatible and no refit is needed.", // Description
              "Salad Slicer", // Category
              "Geometry part 1") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve 1", "C1", "The first, or starting, curve.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve 2", "C2", "The second, or ending, curve.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number of curves", "N", "Number of tween curves to create.", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Tolerance", "T", "The tolerance", GH_ParamAccess.item, 0.001);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curves as a list with Curves.", GH_ParamAccess.list);
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
            int count = 0;
            double tolerance = 0.001;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref curve1)) return;
            if (!DA.GetData(1, ref curve2)) return;
            if (!DA.GetData(2, ref count)) return;
            if (!DA.GetData(3, ref tolerance)) return;

            // Declare the output variables
            List<Curve> result = new List<Curve>();

            // Create the new curves
            try
            {
                result.Add(curve1);
                result.AddRange(Curve.CreateTweenCurvesWithMatching(curve1, curve2, count, tolerance).ToList());
                result.Add(curve2);
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
            get { return Properties.Resources.TweenCurvesWithMatching_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("23CADF17-5263-4B84-85F9-5E10649418A7"); }
        }

    }
}