// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Gh.Components
{
    /// <summary>
    /// Represent a component that constructs a line between two points. 
    /// </summary>
    public class ExampleComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public ExampleComponent()
          : base("Example", // Component name
              "E", // Component nickname
              "An example component description.", // Description
              "Salad Slicer", // Category
              "Example Components") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Start", "S", "Starting point as a point.", GH_ParamAccess.item);
            pManager.AddPointParameter("End", "E", "End point as a point.", GH_ParamAccess.item);

            //pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Line", "L", "Line as a Curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            Point3d start = new Point3d();
            Point3d end = new Point3d();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref start)) return;
            if (!DA.GetData(1, ref end)) return;

            // Create the line
            LineCurve line = new LineCurve(start, end);

            // Assign the output parameters
            DA.SetData(0, line);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; } // <-- change this to primary to make the component visible in grasshopper
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
            get { return new Guid("72A0A189-DB6D-4BDF-BD69-CC9D3C13FD51"); }
        }
    }
}
