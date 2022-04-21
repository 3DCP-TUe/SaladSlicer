// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
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
    public class CurveSlicerComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public CurveSlicerComponent()
          : base("Curve Slicer", // Component name
              "CS", // Component nickname
              "Slices a curve to a program object.", // Description
              "Salad Slicer", // Category
              "Slicers") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Base contour as a Curve.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "Distance between frames as a Number", GH_ParamAccess.item, 20.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_CurveSlicer(), "Slicer Object", "SO", "Sliced curve as a program object.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            Curve contour = new Line().ToNurbsCurve();
            double distance = 20.0;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref contour)) return;
            if (!DA.GetData(1, ref distance)) return;

            // Check input values
            if (distance <= 0.0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The distance between two frames cannot be smaller than or equal to zero."); }
            if (contour.GetLength() < distance) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The distance between two frames exceeds the length of the base contour."); }
       
            // Declare the output variables
            CurveSlicer slicer = new CurveSlicer();
            
            // Create the slicer object
            try
            {
                slicer = new CurveSlicer(contour, distance);
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
            get { return Properties.Resources.CurveSlicer_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B278373A-2101-4B18-9401-CDFA1BC1E211"); }
        }
    }
}