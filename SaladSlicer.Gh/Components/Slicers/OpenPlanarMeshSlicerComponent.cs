// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU Lesser General Public License as published 
// by the Free Software Foundation. For more information and the 
// see <https://github.com/MeshCP-TUe/SaladSlicer>.

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
    /// Represent a component that creates a Mesh slicer object.
    /// </summary>
    public class OpenPlanarMeshSlicerComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public OpenPlanarMeshSlicerComponent()
          : base("Open Planar Mesh Slicer", // Component name
              "CPM", // Component nickname
              "Defines a slicer object for an open planar mesh object.", // Description
              "Salad Slicer", // Category
              "Slicers") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Base geometry as a Mesh.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "Distance between frames as a Number", GH_ParamAccess.item, 20.0);
            pManager.AddBooleanParameter("Reverse", "R", "Reverse path direction", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Heights", "H", "Layer heights a list with Numbers.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_OpenPlanarMeshSlicer(), "Slicer Object", "SO", "Open Planar Mesh Slicer object.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            Mesh mesh = new Mesh() ;
            double distance = 20.0;
            bool reverse = false;
            List<double> heights = new List<double>();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetData(1, ref distance)) return;
            if (!DA.GetData(2, ref reverse)) return;
            if (!DA.GetDataList(3, heights)) return;

            // Check input values
            if (distance <= 0.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The distance between two frames cannot be smaller than or equal to zero."); }

            // Declare the output variables
            OpenPlanarMeshSlicer slicer = new OpenPlanarMeshSlicer();

            // Create the slicer object
            try
            {
                slicer = new OpenPlanarMeshSlicer(mesh, distance, reverse, heights);
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
            get { return Properties.Resources.OpenPlanarMeshSlicer_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C80C8089-857B-46F3-BA9F-FAD020EAB102"); }
        }
    }
}