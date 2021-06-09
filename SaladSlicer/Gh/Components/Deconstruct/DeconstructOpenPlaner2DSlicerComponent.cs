﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Core.Slicers;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    /// <summary>
    /// Represent a component that deconstruct a Open Planar 2.5D Slicer object.
    /// </summary>
    public class DeconstructOpenPlanar2DSlicerComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public DeconstructOpenPlanar2DSlicerComponent()
          : base("Deconstruct Open Planar 2.5D Slicer", // Component name
              "DOP2D", // Component nickname
              "Deconstructs an Open Planar 2.5D Slicer", // Description
              "Salad Slicer", // Category
              "Deconstruct") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>s
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // TODO: Param_OpenPlanar2DSlicer
            pManager.AddGenericParameter("Open Planar 2.5D", "OP2D", "Closed Planar 2.5D Slicer.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Contours", "C", "Contour as a list with Curves.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distance", "D", "Distance between frames as a Number", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heights", "H", "Absolute layer heights a list with Numbers.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Path", "P", "Path as a Curve.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Frames", "F", "Frames as a list with Planes.", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Start Frame", "S", "Start frame as a Plane.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("End Frame", "E", "End frame as a Plane.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            OpenPlanar2DSlicer slicer = new OpenPlanar2DSlicer();
            
            // Access the input parameters individually. 
            if (!DA.GetData(0, ref slicer)) return;

            // Assign the output parameters
            DA.SetDataList(0, slicer.Contours);
            DA.SetData(1, slicer.Distance);
            DA.SetDataList(2, slicer.Heights);
            DA.SetData(3, slicer.InterpolatedPath);
            DA.SetDataList(4, slicer.Frames);
            DA.SetData(5, slicer.FrameAtStart);
            DA.SetData(6, slicer.FrameAtEnd);
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
            get { return new Guid("A6FFF9C8-9DA6-418B-88A1-AC92D3227D9C"); }
        }
    } 
}