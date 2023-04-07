// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU Lesser General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// Salad Libs
using SaladSlicer.Geometry;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that maps contours on a surface. 
    /// </summary>
    public class SurfaceContourMappingComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public SurfaceContourMappingComponent()
          : base("Surface Contour Mapping", // Component name
              "SCM", // Component nickname
              "Maps a contour on a surface.", // Description
              "Salad Slicer", // Category
              "Geometry part 1") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Surface as a Surface.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number of curves", "N", "Number of curves to create.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("Number of samples", "S", "Number of sample points along curves.", GH_ParamAccess.item, 10);
            pManager.AddBooleanParameter("Tranpose", "T", "Indicates whether or not the surface should be tranposed.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curves as a list with Curves.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            Surface surface = Surface.CreateExtrusion(new Line(0.0, 0.0, 0.0, 0.0, 0.0, 1.0).ToNurbsCurve(), new Vector3d(1.0, 0.0, 0.0));
            int count = 10;
            int samples = 10;
            bool transpose = false;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref surface)) return;
            if (!DA.GetData(1, ref count)) return;
            if (!DA.GetData(2, ref samples)) return;
            if (!DA.GetData(3, ref transpose)) return;

            // Declare the output variables
            List<Curve> result = new List<Curve>();

            // Create the new curves
            try
            {
                result = Surfaces.ContourMapping(surface, count, samples, transpose);
            }
            catch (WarningException w)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, w.Message);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }

            // Assign the output parameters
            DA.SetDataList(0, result);
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
            get { return Properties.Resources.SurfaceContourMapping_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("006BE303-42E8-4F10-AD5B-84C5705BCD36"); }
        }
    }
}