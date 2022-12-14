// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU Lesser General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// Salad Slicer Libs
using SaladSlicer.Geometry.Seams;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that set a seam at the closest plane intersection.
    /// </summary>
    public class SeamAtClosestPlaneIntersection : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public SeamAtClosestPlaneIntersection()
          : base("Seam at Closest Plane Intersection", // Component name
              "SCPI", // Component nickname
              "Redefines the startpoint of a closed curve based on the plane intersection closest to the plane origin.", // Description
              "Salad Slicer", // Category
              "Geometry part 2") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "P", "Plane.",  GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            Curve curve = null;
            Plane plane = Plane.WorldXY;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetData(1, ref plane)) return;

            // Declare the output variable
            Curve result = curve;

            // Create the new curve
            try
            {
                result = Locations.SeamAtClosestPlaneIntersection(curve, plane);
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
            DA.SetData(0, result);
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
            get { return Properties.Resources.SeamaAtPlane_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0A6F67B0-0BEB-49B8-BCDD-027C667CC4C1"); }
        }

    }
}