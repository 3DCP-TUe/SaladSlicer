// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
// Rhino Libs
using Rhino.Geometry;
using SaladSlicer.Core.Geometry;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that joins curves using linear interpolation. 
    /// </summary>
    public class JoinClosedCurvesComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public JoinClosedCurvesComponent()
          : base("Join Closed Curves", // Component name
              "JCC", // Component nickname
              "Joins a list of closed curves between end and start points. Alternation false keeps the original directions of the curves, while alternation true flips every other curve in the list.", // Description
              "Salad Slicer", // Category
              "Geometry") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "List of curves", GH_ParamAccess.list);
            pManager.AddNumberParameter("Startparameter", "S", "A parameter thet redefines the startpoint of the curves [0 to 1]", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Changelength", "L", "Sets the length over which to connect to the next layer", GH_ParamAccess.item, 100);
            pManager.AddBooleanParameter("Alternate", "A", "Reverses the direction of every other curve", GH_ParamAccess.item,false);
            pManager.AddTextParameter("Connection type", "T", "Sets the type of connection [Linear,Interpolation]", GH_ParamAccess.item, "Interpolation");

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Joined curve.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            List<Curve> curves = new List<Curve>();
            bool reverse = new bool();
            string type = "";
            Curve joinedCurve;
            double changeLength=new double();
            double startParameter = new double();

            // Access the input parameters individually. 
            if (!DA.GetDataList(0, curves)) return;
            if (!DA.GetData(1, ref startParameter)) return;
            if (!DA.GetData(2, ref changeLength)) return;
            if (!DA.GetData(3, ref reverse)) return;
            if (!DA.GetData(4, ref type)) return;

            // Check input values
            if (startParameter < 0.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Parameter value is not in the range of 0 to 1."); }
            if (startParameter > 1.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Parameter value is not in the range of 0 to 1."); }
            if (curves[0].GetLength() < changeLength) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The length of the layer change exceeds the length of the base contour."); }
            if (Curves.NumberClosed(curves) != curves.Count) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more curves are not closed"); }
           
            // Create the code line
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
            curvesCopy = Curves.CutTransitionEnd(curves, changeLength);
            if (type == "Linear")
            {
                joinedCurve = Curves.JoinLinear(curvesCopy, reverse);
            }
            else if (type == "Interpolation")
            {
                joinedCurve = Curves.JoinOutsideInterpolated(curvesCopy, reverse);
            }
            else
            {
                joinedCurve = Line.Unset.ToNurbsCurve();
            }

            // Assign the output parameters
            DA.SetData(0, joinedCurve);
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
            get { return Properties.Resources.ExampleIcon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0B51A54D-2D15-4879-AD49-005FCF834F30"); }
        }

    }
}