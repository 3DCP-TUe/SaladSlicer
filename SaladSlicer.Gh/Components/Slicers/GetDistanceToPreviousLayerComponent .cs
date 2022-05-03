// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Slicers;
using SaladSlicer.Gh.Parameters.Slicers;
using SaladSlicer.Gh.Utils ;
using SaladSlicer.Interfaces;

namespace SaladSlicer.Gh.Components.Slicers
{
    /// <summary>
    /// Represent a component that creates the contours.
    /// </summary>
    public class GetDistanceToPreviousLayerComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public GetDistanceToPreviousLayerComponent()
          : base("Get Distances To Previous Contours", // Component name
              "DPC", // Component nickname
              "Gets the distance of every frame to previous contour.", // Description
              "Salad Slicer", // Category
              "Slicers") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_SlicerObject(), "Slicer Object", "SO", "Slicer object.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "P", "Plane (printbed) to calculate distance to for the frames in the first layer.", GH_ParamAccess.item, Plane.WorldXY);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Distances", "D", "List of distances.", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Delta X", "X", "List of distances in x-direction.", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Delta Y", "Y", "List of distances in y-direction.", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Delta Z", "Z", "List of distances in z-direction.", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            ISlicer slicer = new ClosedPlanar2DSlicer();
            Plane plane = new Plane();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref slicer)) return;
            if (!DA.GetData(1, ref plane)) return;

            // Declare the output variables
            List<List<double>> dist = new List<List<double>>();
            List<List<double>> dx = new List<List<double>>();
            List<List<double>> dy = new List<List<double>>();
            List<List<double>> dz = new List<List<double>>();

            // Calculate distances
            try
            {
                dist = slicer.GetDistanceToPreviousLayer(plane, out dx, out dy, out dz);
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
            DA.SetDataTree(0, HelperMethods.ListInListToDataTree(dist));
            DA.SetDataTree(1, HelperMethods.ListInListToDataTree(dx));
            DA.SetDataTree(2, HelperMethods.ListInListToDataTree(dy));
            DA.SetDataTree(3, HelperMethods.ListInListToDataTree(dz));
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
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
            get { return Properties.Resources.GetDistancesBetweenContours_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("941B95AE-3F20-47DB-B81D-B7F8A2600EDA"); }
        }
    }
}