// This file is part of SaladSlicer. SaladSlicer is licensed 
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
using SaladSlicer.Core.Geometry.Seams;
using SaladSlicer.Core.Enumerations;
using SaladSlicer.Gh.Utils;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that joins curves using linear interpolation. 
    /// </summary>
    public class JoinOpenCurvesComponent : GH_Component
    {
        #region fields
        private bool _expire = false;
        private bool _valueListAdded = false; 
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public JoinOpenCurvesComponent()
          : base("Join Open Curves", // Component name
              "JOC", // Component nickname
              "Joins a list of open curves between end and start points. Connection type 'Linear' connects the curves with the shortest path and 'Bezier' interpolates between the curves smoothly.", // Description
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
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Joined curve.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Connections", "C", "List of connections between input curves.", GH_ParamAccess.list);
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
                PointF location = new PointF(this.Params.Input[0].Attributes.InputGrip.X - 120, this.Params.Input[0].Attributes.InputGrip.Y - 11);
                _expire = HelperMethods.CreateValueList(this, 1, typeof(OpenTransition));
                _valueListAdded = true;
            }

            // Expire solution of this component
            if (_expire == true)
            {
                _valueListAdded = true;
                _expire = false;
                this.ExpireSolution(true);
            }

            // Declare variable of input parameters
            List<Curve> curves = new List<Curve>();
            int type = new int();
            Curve joinedCurve;
            List<Curve> transitions=new List<Curve>();

            // Access the input parameters individually. 
            if (!DA.GetDataList(0, curves)) return;
            if (!DA.GetData(1, ref type)) return;

            // Create the code line            
            if (type == 0)
            {
                (joinedCurve,transitions) = Transitions.JoinLinear(curves);
            }
            else if (type == 1)
            {
                (joinedCurve, transitions) = Transitions.JoinBezier(curves);
            }
            else
            {
                joinedCurve = Line.Unset.ToNurbsCurve();
            }

            // Assign the output parameters
            DA.SetData(0, joinedCurve);
            DA.SetDataList(1, transitions);
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
            get { return Properties.Resources.JoinOpenCurves_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("E8F8F706-2DEE-497C-8CAD-44FADD8445AE"); }
        }

    }
}