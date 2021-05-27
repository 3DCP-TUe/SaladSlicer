// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
using System.Drawing;
// Grasshopper Libs
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
// Rhino Libs
using Rhino.Geometry;
using SaladSlicer.Core.Geometry;
using SaladSlicer.Core.Enumerations;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that joins curves using linear interpolation. 
    /// </summary>
    public class JoinClosedCurvesComponent : GH_Component
    {
        #region fields
        private bool _created = false;
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
            pManager.AddIntegerParameter("Connection type", "T", "Sets the type of connection [0 = Linear,1 = Bezier]", GH_ParamAccess.item,1);

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
            // Expire solution of this component
            if (_created == false)
            {
                CreateValueList();
                _created = true;
            }
            // Declare variable of input parameters
            List<Curve> curves = new List<Curve>();
            bool reverse = new bool();
            int type =new int();
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
            if (type != 0 && type != 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Comment type value <" + type + "> is invalid. " +
                    "In can only be set to 0 or 1. Use 0 for linear and 1 for bezier connections.");
            }
            // Create the code line
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
            curvesCopy = Curves.CutTransitionEnd(curves, changeLength);
            if (type == 0)
            {
                joinedCurve = Curves.JoinLinear(curvesCopy, reverse);
            }
            else if (type == 1)
            {
                joinedCurve = Curves.JoinBezier(curvesCopy, reverse);
            }
            else
            {
                joinedCurve = Line.Unset.ToNurbsCurve();
            }

            // Assign the output parameters
            DA.SetData(0, joinedCurve);
        }
        #region valuelist
        /// <summary>
        /// Creates the value list for the transition type and connects it the input parameter is other source is connected
        /// </summary>
        private void CreateValueList()
        {
            if (this.Params.Input[4].SourceCount == 0)
            {
                var parameter = Params.Input[4];
                // Creates the empty value list
                GH_ValueList obj = new GH_ValueList();
                obj.CreateAttributes();
                obj.ListMode = GH_ValueListMode.DropDown;
                obj.ListItems.Clear();
                obj.ToggleItem(1);
                // Add the items to the value list
                string[] names = Enum.GetNames(typeof(Transition));
                int[] values = (int[])Enum.GetValues(typeof(Transition));

                for (int i = 0; i < names.Length; i++)
                {
                    obj.ListItems.Add(new GH_ValueListItem(names[i], values[i].ToString()));
                }

                //obj.SelectedItems.Remove(new GH_ValueListItem(names[0], values[0].ToString()));
                //obj.SelectedItems.Add(new GH_ValueListItem(names[1], values[1].ToString()));
                // Make point where the valuelist should be created on the canvas
                obj.Attributes.Pivot = new PointF(this.Attributes.Pivot.X - this.Attributes.Bounds.Width - obj.Attributes.Bounds.Width / 4, this.Attributes.Pivot.Y + this.Attributes.Bounds.Height / 2 - obj.Attributes.Bounds.Height);
                // Add the value list to the active canvas
                Instances.ActiveCanvas.Document.AddObject(obj, false);

                // Connect the value list to the input parameter
                parameter.AddSource(obj);

                // Collect data
                parameter.CollectData();

                // Created
                _created = true;

                // Expire value list
                obj.ExpireSolution(true);
            }
        }
        #endregion
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