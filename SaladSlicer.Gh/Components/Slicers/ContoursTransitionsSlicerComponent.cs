﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.Collections.Generic;
//Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Slicers;
using SaladSlicer.Gh.Parameters.Slicers;

namespace SaladSlicer.Gh.Components.Slicers
{
    /// <summary>
    /// Represent a component that creates a program object as a sliced curve.
    /// </summary>
    public class ContoursTransitionsSlicerComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public ContoursTransitionsSlicerComponent()
          : base("Contours Transitions Slicer", // Component name
              "CTS", // Component nickname
              "Slices a list of contours and transitions to a Slicer Object. This component is intented to be used together with the Join Closed Contours and the Join Open Contours components.", // Description
              "Salad Slicer", // Category
              "Slicers") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Contours", "C", "List of curves with length n.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Transitions", "T", "List of connections with length n-1.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distance", "D", "Distance between frames as a Number", GH_ParamAccess.item, 20.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_ContoursTransitionsSlicer(), "Slicer Object", "SO", "Sliced contours and transitions as a program object.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            List<Curve> contours = new List<Curve>();
            List<Curve> transitions = new List<Curve>();
            double distance = 20.0;

            // Access the input parameters individually. 
            if (!DA.GetDataList(0, contours)) return;
            if (!DA.GetDataList(1, transitions)) return;
            if (!DA.GetData(2, ref distance)) return;

            // Check input values
            if (distance <= 0.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The distance between two frames cannot be smaller than or equal to zero."); }
            if (contours.Count - 1 != transitions.Count) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The number of defined transitions does not match with the number of contours."); }

            // Declare the output variables
            ContoursTransitionsSlicer slicer = new ContoursTransitionsSlicer();
            slicer.Slice();

            // Create the slicer object
            try
            {
                slicer = new ContoursTransitionsSlicer(contours, transitions, distance);
                slicer.Slice();
            }
            catch (WarningException warning)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, warning.Message);
            }
            catch (Exception error)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, error.Message);
            }

            // Assign the output parameters
            DA.SetData(0, slicer);
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
            get { return Properties.Resources.ContoursTransitionsSlicer_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F36953E2-A076-4750-B475-BF15018384CF"); }
        }
    }
}