// SPDX-License-Identifier: GPL-3.0-or-later
// Salad Slicer
// Project: https://github.com/3DCP-TUe/SaladSlicer
//
// Copyright (c) 2021-2026 Eindhoven University of Technology
//
// Authors:
//  - Arjen Deetman (2021-2024)
//  - Derk Bos (2021-2022)
// 
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Slicers;
using SaladSlicer.Gh.Parameters.Slicers;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    /// <summary>
    /// Represent a component that deconstruct a Curves Transitions Slicer object.
    /// </summary>
    public class DeconstructCurvesTransitionsSlicerComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public DeconstructCurvesTransitionsSlicerComponent()
          : base("Deconstruct Curves Transitions Slicer", // Component name
              "CTS", // Component nickname
              "Deconstructs a Curves Transitions Slicer", // Description
              "Salad Slicer", // Category
              "Deconstruct") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ContoursTransitionsSlicer(), "Curve Transitions Slicer", "CTS", "Curve Transitions Slicer.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Contours", "C", "Contours as a list with Curves.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Transitions", "T", "Transitions as a list with Curves.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distance", "D", "Distance between frames as a Number", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            ContoursTransitionsSlicer slicer = new ContoursTransitionsSlicer();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref slicer)) return;

            // Assign the output parameters
            DA.SetDataList(0, slicer.Contours);
            DA.SetDataList(1, slicer.Transitions);
            DA.SetData(2, slicer.Distance);
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
            get { return Properties.Resources.DecontstructCurvesTransitionsSlicer_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7FD659BC-8C49-4ADC-8007-3A04FE2D33A0"); }
        }
    }
}