﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Drawing;
using System.Collections.Generic;
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
        private bool _valueListAdded = false;
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public JoinClosedCurvesComponent()
          : base("Join Closed Curves", // Component name
              "JCC", // Component nickname
              "Joins a list of closed curves between end and start points. Connection type 'Linear' connects the curves with the shortest path, 'Bezier' interpolates between the curves smoothly and 'Interpolate' moves linearly in Z-direction while using the interpolated X- and Y-coordinates of the two curves.", // Description
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
            pManager.AddIntegerParameter("Connection type", "T", "Sets the type of connection [0 = Linear,1 = Bezier]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Changelength", "L", "Sets the length over which to connect to the next layer", GH_ParamAccess.item, 100);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Joined Curve", "JC", "Joined curve.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Trimmed Curves", "TC", "List of curves between connections.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Connections", "Co", "List of connections between input curves.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Create a value list
            if (_valueListAdded == false)
            {
                _expire = HelperMethods.CreateValueList(this, 1, typeof(ClosedTransition));
                _valueListAdded = true;
            }
            
            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Declare variable of input parameters
            List<Curve> curves = new List<Curve>();
            int type = new int();
            double changeLength = new double();

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
                    "It can only be set to 0, 1 and 2. Use 0 for linear, 1 for bezier and 2 for interpolated connections.");
            }

            // Delcare variables
            List<Curve> transitions = new List<Curve>();
            List<Curve> curvesCopy = new List<Curve>();
            Curve joinedCurve;

            if (type == 0)
            {
                curvesCopy = Transitions.TrimCurveFromEnds(curves, changeLength);
                (joinedCurve,transitions) = Transitions.JoinLinear(curvesCopy);
            }
            else if (type == 1)
            {
                curvesCopy = Transitions.TrimCurveFromEnds(curves, changeLength);
                (joinedCurve, transitions) = Transitions.JoinBezier(curvesCopy);
            }
            else if (type == 2)
            {
                curvesCopy = Transitions.TrimCurveFromEnds(curves, changeLength);
                (joinedCurve, transitions) = Transitions.JoinInterpolated(curves, changeLength);
            }
            else
            {
                joinedCurve = Line.Unset.ToNurbsCurve();
            }

            // Assign the output parameters
            DA.SetData(0, joinedCurve);
            DA.SetDataList(1, curvesCopy);
            DA.SetDataList(2, transitions);
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
            get { return Properties.Resources.JoinClosedCurves_Icon; }
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