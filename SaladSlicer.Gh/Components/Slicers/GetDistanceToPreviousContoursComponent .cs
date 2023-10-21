// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
// Salad Slicer Libs
using SaladSlicer.Interfaces;
using SaladSlicer.Gh.Parameters.Slicers;
using SaladSlicer.Gh.Goos.Slicers;

namespace SaladSlicer.Gh.Components.Slicers
{
    /// <summary>
    /// Represent a component that creates the contours.
    /// </summary>
    public class GetDistanceToPreviousContoursComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public GetDistanceToPreviousContoursComponent()
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
            pManager.AddParameter(new Param_SlicerObject(), "Slicer Object", "SO", "Slicer object.", GH_ParamAccess.tree);
            pManager.AddPlaneParameter("Plane", "P", "Plane (printbed) to calculate distance to for the frames in the first layer.", GH_ParamAccess.tree, Plane.WorldXY);
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
            // Input variables
            GH_Structure<GH_SlicerObject> slicers;
            GH_Structure<GH_Plane> planes;

            // Catch the input data
            if (!DA.GetDataTree(0, out slicers)) return;
            if (!DA.GetDataTree(1, out planes)) return;

            // Initialize component output
            GH_Structure<GH_Number> dist = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> dx = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> dy = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> dz = new GH_Structure<GH_Number>();

            // Fill the output tree
            int maxBranches = Math.Max(slicers.Branches.Count, planes.Branches.Count);

            for (int i = 0; i < maxBranches; i++)
            {
                int iSlicer = Math.Min(i, slicers.Branches.Count - 1);
                int iPlane = Math.Min(i, planes.Branches.Count - 1);

                List<GH_SlicerObject> slicerBranch = slicers.Branches[iSlicer];
                List<GH_Plane> planeBranch = planes.Branches[iPlane];

                int maxBranchLength = Math.Max(slicerBranch.Count, planeBranch.Count);

                for (int j = 0; j < maxBranchLength; j++)
                {
                    int jSlicer = Math.Min(j, slicerBranch.Count - 1);
                    int jPlane = Math.Min(j, planeBranch.Count - 1);

                    ISlicer slicer = slicerBranch[jSlicer].Value;
                    Plane plane = planeBranch[jPlane].Value;

                    GH_Path path = new GH_Path(i, j);
                    path = path.AppendElement(0);

                    List<List<double>> tempDist = new List<List<double>>() { };
                    List<List<double>> tempX = new List<List<double>>() { };
                    List<List<double>> tempY = new List<List<double>>() { };
                    List<List<double>> tempZ = new List<List<double>>() { };

                    try
                    {
                        tempDist = slicer.GetDistanceToPreviousLayer(plane, out tempX, out tempY, out tempZ);
                    }
                    catch (WarningException w)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, w.Message);
                    }
                    catch (Exception e)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                    }

                    for (int k = 0; k < tempDist.Count; k++)
                    {
                        dist.AppendRange(tempDist[k].ConvertAll(item => new GH_Number(item)), path);
                        dx.AppendRange(tempX[k].ConvertAll(item => new GH_Number(item)), path);
                        dy.AppendRange(tempY[k].ConvertAll(item => new GH_Number(item)), path);
                        dz.AppendRange(tempZ[k].ConvertAll(item => new GH_Number(item)), path);
                        path = path.Increment(path.Length - 1);
                    }
                }
            }

            // Assign the output parameters
            DA.SetDataTree(0, dist);
            DA.SetDataTree(1, dx);
            DA.SetDataTree(2, dy);
            DA.SetDataTree(3, dz);
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
            get { return Properties.Resources.GetDistancesToPreviousContorus_Icon; }
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