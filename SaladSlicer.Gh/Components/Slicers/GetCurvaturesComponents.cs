// SPDX-License-Identifier: GPL-3.0-or-later
// Salad Slicer
// Project: https://github.com/3DCP-TUe/SaladSlicer
//
// Copyright (c) 2021-2026 Eindhoven University of Technology
//
// Authors:
//  - Arjen Deetman (2021-2026)
//  - Derk Bos (2021-2022)
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
using SaladSlicer.Gh.Parameters.Slicers;
using SaladSlicer.Gh.Goos.Slicers;

namespace SaladSlicer.Gh.Components.Slicers
{
    /// <summary>
    /// Represent a component that gets the curvatures.
    /// </summary>
    public class GetCurvaturesComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public GetCurvaturesComponent()
          : base("Get Curvatures", // Component name
              "C", // Component nickname
              "Defines the curvatures of the path at each frame of a sliced object.", // Description
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
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Curvatures", "K", "Curvatures as a datatree with Vectors.", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            GH_Structure<GH_SlicerObject> slicers;

            // Catch the input data
            if (!DA.GetDataTree(0, out slicers)) return;

            // Initialize component output
            GH_Structure<GH_Vector> curvatures = new GH_Structure<GH_Vector>();

            // Fill the output tree
            if (slicers.Branches.Count != 0)
            {
                for (int i = 0; i < slicers.Branches.Count; i++)
                {
                    // Gets the current path of this branch
                    GH_Path currentPath = slicers.Paths[i];

                    for (int j = 0; j < slicers.Branches[i].Count; j++)
                    {
                        ISlicer slicer = slicers.Branches[i][j].Value;

                        GH_Path path = new GH_Path(currentPath);
                        path = path.AppendElement(j); // Path index of object
                        path = path.AppendElement(0); // Path index of layer

                        List<List<Vector3d>> temp = new List<List<Vector3d>>() { };

                        try
                        {
                            temp = slicer.GetCurvatures();
                        }
                        catch (WarningException w)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, w.Message);
                        }
                        catch (Exception e)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                        }

                        for (int k = 0; k < temp.Count; k++)
                        {
                            curvatures.AppendRange(temp[k].ConvertAll(item => new GH_Vector(item)), path);
                            path = path.Increment(path.Length - 1); // Update path index of layer
                        }
                    }
                }
            }

            // Assign the output parameters
            DA.SetDataTree(0, curvatures);
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
            get { return Properties.Resources.GetCurvatures_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("EEEF1A11-95E1-462E-ABA2-F531BA33F446"); }
        }
    }
}