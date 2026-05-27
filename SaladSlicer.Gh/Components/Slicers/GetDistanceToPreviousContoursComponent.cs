// SPDX-License-Identifier: GPL-3.0-or-later
// Salad Slicer
// Project: https://github.com/3DCP-TUe/SaladSlicer
//
// Copyright (c) 2021-2026 Eindhoven University of Technology
//
// Authors:
//  - Arjen Deetman (2021-2023)
//  - Derk Bos (2021-2022)
//  - Matthew Ferguson (2021)
// 
// For license details, see the LICENSE file in the project root.

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
using SaladSlicer.Enumerations;
using SaladSlicer.Gh.Parameters.Slicers;
using SaladSlicer.Gh.Goos.Slicers;
using SaladSlicer.Gh.Utils;

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
            pManager.AddPlaneParameter("Plane", "P", "Plane (printbed) to calculate distance to for the frames in the first layer.", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddIntegerParameter("Structure", "S", "Sets the output datatree structure; frames by layer (0) or by object (1).", GH_ParamAccess.item, 0);
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
            // Creates the input value list and attachs it to the input parameter
            if (this.Params.Input[2].SourceCount == 0)
            {
                HelperMethods.CreateValueList(this, 2, typeof(OutputStructure), true);
                this.ExpireSolution(true);
            }

            // Input variables
            GH_Structure<GH_SlicerObject> slicers;
            Plane plane = Plane.WorldXY;
            int outputStructure = 0;

            // Catch the input data
            if (!DA.GetDataTree(0, out slicers)) return;
            if (!DA.GetData(1, ref plane)) return;
            if (!DA.GetData(2, ref outputStructure)) { return; }

            // Check input
            if (outputStructure != 0 & outputStructure != 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The set datatree structure type is not valid; set equal to 0 for frames by layer or 1 for frames by object.");
                return;
            }

            // Initialize component output
            GH_Structure<GH_Number> dist = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> dx = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> dy = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> dz = new GH_Structure<GH_Number>();

            // Fill the output tree
            for (int i = 0; i < slicers.Branches.Count; i++)
            {
                GH_Path currentPath;

                // Gets the current path of this branch
                currentPath = slicers.Paths[i];

                for (int j = 0; j < slicers.Branches[i].Count; j++)
                {
                    ISlicer slicer = slicers.Branches[i][j].Value;

                    GH_Path path = new GH_Path(currentPath);
                    path = path.AppendElement(j); // Path index of object
                    
                    // Stores the frames of each layer in its own datatree branch
                    if (outputStructure == 0)
                    {
                        path = path.AppendElement(0); // Path index of layer
                    }

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

                        if (outputStructure == 0)
                        {
                            path = path.Increment(path.Length - 1); // Update path index of layer
                        }
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
            get { return new Guid("B92E7623-8994-43D2-A389-EA7B8FAE6F44"); }
        }
    }
}