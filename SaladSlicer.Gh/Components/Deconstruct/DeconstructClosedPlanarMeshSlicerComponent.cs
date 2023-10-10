// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// see <https://github.com/MeshCP-TUe/SaladSlicer>.

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
    /// Represent a component that deconstruct a Closed Planar Mesh Slicer object.
    /// </summary>
    public class DeconstructClosedPlanarMeshSlicerComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public DeconstructClosedPlanarMeshSlicerComponent()
          : base("Deconstruct Closed Planar Mesh Slicer", // Component name
              "DCPM", // Component nickname
              "Deconstructs a Closed Planar Mesh Slicer", // Description
              "Salad Slicer", // Category
              "Deconstruct") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ClosedPlanarMeshSlicer(), "Closed Planar Mesh", "CPM", "Closed Planar Mesh Slicer.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh as a Mesh.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Parameter", "t", "Parameter for layer change as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Length for layer change as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "Distance between frames as a Number", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heights", "H", "Absolute layer heights a list with Numbers.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            ClosedPlanarMeshSlicer slicer = new ClosedPlanarMeshSlicer();
            
            // Access the input parameters individually. 
            if (!DA.GetData(0, ref slicer)) return;

            // Assign the output parameters
            DA.SetData(0, slicer.Mesh);
            DA.SetData(1, slicer.SeamLocation);
            DA.SetData(2, slicer.SeamLength);
            DA.SetData(3, slicer.Distance);
            DA.SetDataList(4, slicer.Heights);
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
            get { return Properties.Resources.DeconstructClosedPlanarMeshSlicer_Icon; }
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