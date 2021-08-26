// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
using System.Drawing;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
using SaladSlicer.Core.Geometry;
using SaladSlicer.Core.Geometry.Seams;
using SaladSlicer.Gh.Utils;
using SaladSlicer.Core.Enumerations;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that joins curves using linear interpolation. 
    /// </summary>
    public class JoinClosedCurvesComponent : GH_Component
    {
        #region fields
        private bool _expire = false;
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
            pManager.AddIntegerParameter("Connection type", "T", "Sets the type of connection [0 = Linear,1 = Bezier]", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Changelength", "L", "Sets the length over which to connect to the next layer", GH_ParamAccess.item, 100);
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
            //Create a value list
            _expire = HelperMethods.CreateValueList(this, 1, typeof(Transition));
            
            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Declare variable of input parameters
            List<Curve> curves = new List<Curve>();
            int type =new int();
            Curve joinedCurve;
            double changeLength=new double();

            // Access the input parameters individually. 
            if (!DA.GetDataList(0, curves)) return;
            if (!DA.GetData(1, ref type)) return;
            if (!DA.GetData(2, ref changeLength)) return;

            // Check input values
            if (curves[0].GetLength() < changeLength) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The length of the layer change exceeds the length of the base contour."); }
            if (Curves.NumberClosed(curves) != curves.Count) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more curves are not closed"); }
            if (type != 0 && type != 1 && type != 2)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Comment type value <" + type + "> is invalid. " +
                    "In can only be set to 0, 1 and 2. Use 0 for linear, 1 for bezier and 2 for interpolated connections.");
            }
            
            // Create the code line
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
            
            if (type == 0)
            {
                curvesCopy = Transitions.CutTransitionEnd(curves, changeLength);
                joinedCurve = Transitions.JoinLinear(curvesCopy);
            }
            else if (type == 1)
            {
                curvesCopy = Transitions.CutTransitionEnd(curves, changeLength);
                joinedCurve = Transitions.JoinBezier(curvesCopy);
            }
            else if (type == 2)
            {
                joinedCurve = Transitions.JoinInterpolated(curvesCopy, changeLength);
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