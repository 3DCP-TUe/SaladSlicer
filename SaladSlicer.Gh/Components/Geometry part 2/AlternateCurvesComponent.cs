// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
using SaladSlicer.Geometry;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that joins curves using linear interpolation. 
    /// </summary>
    public class AlternateCurvesComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public AlternateCurvesComponent()
          : base("Alternate Curves", // Component name
              "AC", // Component nickname
              "Reverses the direction of every other curve in a list, starting with the second curve.", // Description
              "Salad Slicer", // Category
              "Geometry part 2") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curves to alternate", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Alternated curves", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            List<Curve> curves = new List<Curve>();

            // Access the input parameters individually. 
            if (!DA.GetDataList(0, curves)) return;

            // Declare the output variables
            List<Curve> curvesCopy = new List<Curve>();

            // Create the curves
            try
            {
                curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
                curvesCopy = Curves.AlternateCurves(curvesCopy);
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
            DA.SetDataList(0, curvesCopy);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
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
            get { return Properties.Resources.AlternateCurves_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1DCEE820-E981-49F7-8E99-2D9BA7B695A9"); }
        }

    }
}