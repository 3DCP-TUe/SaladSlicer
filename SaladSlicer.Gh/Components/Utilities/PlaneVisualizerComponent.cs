// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Gh.Components.Utilities
{
    /// <summary>
    /// Represents the component that visualizes the orientation of a plane. 
    /// </summary>
    public class PlaneVisualizerComponent : GH_Component
    {
        #region fields
        private readonly List<Plane> _planes = new List<Plane>();
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public PlaneVisualizerComponent()
          : base("Plane Visualizer", // Component name
              "PV", // Component nickname
              "Visualizes the orientation vectors of a plane.", // Description
              "Salad Slicer", // Category
              "Utilities") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            // This component has no ouput parameters. It only visualizes the plane orientation.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            _planes.Clear();

            // Access the input parameters individually. 
            if (!DA.GetDataTree(0, out GH_Structure<GH_Plane> inputPlanes)) { return; }

            // Flatten the datatree to a list
            for (int i = 0; i < inputPlanes.Branches.Count; i++)
            {
                var branches = inputPlanes.Branches[i];

                for (int j = 0; j < branches.Count; j++)
                {
                    _planes.Add(branches[j].Value);
                }
            }
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
            get { return Properties.Resources.PlaneVisualizer_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("A909346E-EE64-45A9-88FB-069CED58A6DF"); }
        }

        /// <summary>
        /// Displays the three vectors of all the planes in the list.
        /// </summary>
        /// <param name="args"> Preview display arguments for IGH_PreviewObjects. </param>
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            for (int i = 0; i < _planes.Count; i++)
            {
                Plane plane = _planes[i];

                if (plane != Plane.Unset)
                {
                    if (plane.IsValid == true)
                    {
                        args.Display.DrawDirectionArrow(plane.Origin, plane.ZAxis, System.Drawing.Color.Blue);
                        args.Display.DrawDirectionArrow(plane.Origin, plane.XAxis, System.Drawing.Color.Red);
                        args.Display.DrawDirectionArrow(plane.Origin, plane.YAxis, System.Drawing.Color.Green);
                    }
                }
            }
        }
    }
}