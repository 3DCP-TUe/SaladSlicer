﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
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
    public class RedefineStartpointComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public RedefineStartpointComponent()
          : base("Set Start Point", // Component name
              "RS", // Component nickname
              "Redefines the startpoint of a closed curve based on a parameter between 0 and 1.", // Description
              "Salad Slicer", // Category
              "Geometry") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve", GH_ParamAccess.item);
            pManager.AddNumberParameter("Parameter", "P", "A parameter thet redefines the startpoint of the curve [0 to 1]", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            Curve curve = new Line().ToNurbsCurve();
            double param = new double();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetData(1, ref param)) return;

            // Check input values
            if (param < 0.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Parameter value is not in the range of 0 to 1."); }
            if (param > 1.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Parameter value is not in the range of 0 to 1."); }
            
            // Create the code line
            Curve curveCopy = curve.DuplicateCurve();
            curveCopy.Domain = new Interval(0, 1);
            curveCopy = Curves.SetStartPointAtParam(curveCopy, param);
            curveCopy.Domain= new Interval(0, curveCopy.GetLength());
            // Assign the output parameters
            DA.SetData(0, curveCopy);
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
            get { return Properties.Resources.ExampleIcon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F791CB38-CA24-4C92-9343-648CF6AE1203"); }
        }

    }
}