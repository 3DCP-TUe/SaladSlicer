// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.Collections.Generic;
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
    public class GetDistancesAlongContoursComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public GetDistancesAlongContoursComponent()
          : base("Get Distances Along Contours", // Component name
              "DBC", // Component nickname
              "Gets the distance of every frame to the beginning of the contours", // Description
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
            pManager.AddNumberParameter("Distances", "D", "List of distances", GH_ParamAccess.tree);
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
            GH_Structure<GH_Number> distances = new GH_Structure<GH_Number>();

            // Fill the output tree
            if (slicers.Branches.Count != 0)
            {
                for (int i = 0; i < slicers.Branches.Count; i++)
                {
                    for (int j = 0; j < slicers.Branches[i].Count; j++)
                    {
                        ISlicer slicer = slicers.Branches[i][j].Value;

                        GH_Path path = new GH_Path(i, j);
                        path = path.AppendElement(0);

                        List<List<double>> temp = new List<List<double>>() { };

                        try
                        {
                            temp = slicer.GetDistancesAlongContours();
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
                            distances.AppendRange(temp[k].ConvertAll(item => new GH_Number(item)), path);
                            path = path.Increment(path.Length - 1);
                        }
                    }
                }
            }

            // Assign the output parameters
            DA.SetDataTree(0, distances);
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
            get { return Properties.Resources.GetDistancesAlongContours_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("AEA874C9-4D80-4513-B3D9-91E6F1AD600E"); }
        }
    }
}