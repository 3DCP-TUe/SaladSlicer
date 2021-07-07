// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Core.Slicers;
using SaladSlicer.Gh.Parameters.Slicers;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    /// <summary>
    /// Represent a component that deconstruct a Closed Planar 3D Slicer object.
    /// </summary>
    public class DeconstructClosedPlanar3DSlicerComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public DeconstructClosedPlanar3DSlicerComponent()
          : base("Deconstruct Closed Planar 3D Slicer", // Component name
              "DCP3D", // Component nickname
              "Deconstructs a Closed Planar 3D Slicer", // Description
              "Salad Slicer", // Category
              "Deconstruct") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ClosedPlanar3DSlicer(), "Closed Planar 3D", "CP3D", "Closed Planar 3D Slicer.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh as a Mesh.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Contours", "C", "Contour as a list with Curves.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Parameter", "t", "Parameter for layer change as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Length for layer change as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "Distance between frames as a Number", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heights", "H", "Absolute layer heights a list with Numbers.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Path", "P", "Path as a Curve", GH_ParamAccess.item);
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
            ClosedPlanar3DSlicer slicer = new ClosedPlanar3DSlicer();
            
            // Access the input parameters individually. 
            if (!DA.GetData(0, ref slicer)) return;

            // Assign the output parameters
            DA.SetData(0, slicer.Mesh);
            DA.SetDataList(1, slicer.Contours);
            DA.SetData(2, slicer.ChangeParameter);
            DA.SetData(3, slicer.ChangeLength);
            DA.SetData(4, slicer.Distance);
            DA.SetDataList(5, slicer.Heights);
            DA.SetData(6, slicer.InterpolatedPath);
            DA.SetDataList(7, slicer.Frames);
            DA.SetData(8, slicer.FrameAtStart);
            DA.SetData(9, slicer.FrameAtEnd);
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
            get { return new Guid("B2D60172-A0C7-4718-9889-C691AD6BDF4F"); }
        }
    } 
}