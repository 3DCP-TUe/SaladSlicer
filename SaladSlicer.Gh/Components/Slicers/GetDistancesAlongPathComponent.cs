// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU Lesser General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Slicers;
using SaladSlicer.Interfaces;
using SaladSlicer.Enumerations;
using SaladSlicer.Gh.Parameters.Slicers;
using SaladSlicer.Gh.Utils;

namespace SaladSlicer.Gh.Components.Slicers
{
    /// <summary>
    /// Represent a component that gets the distances of the frame along the path.
    /// </summary>
    public class GetDistancesAlongPathComponent : GH_Component
    {
        #region fields
        private bool _expire = false;
        private bool _valueListAdded = false;
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public GetDistancesAlongPathComponent()
          : base("Get Distances Along Path", // Component name
              "DAP", // Component nickname
              "Defines the distances of the frames along a path of a slicer object", // Description
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
            pManager.AddIntegerParameter("Path Type", "T", "Sets the type of path [0 = Original, 1 = Spline interpolated, 3 = Linear interpolated]", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Distances", "D", "Distances as list with Numbers.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Create a value list
            if (_valueListAdded == false)
            {
                _expire = HelperMethods.CreateValueList(this, 1, typeof(PathType));
                _valueListAdded = true;
            }

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Declare variable of input parameters
            ISlicer slicer = new ClosedPlanar2DSlicer();
            int type = 0;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref slicer)) return;
            if (!DA.GetData(1, ref type)) return;

            // Assign the output parameters
            DA.SetDataList(0, slicer.GetDistancesAlongPath((PathType)type));
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
            get { return null; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("92A020DF-D5AC-4B41-B31C-C657E8EA6C56"); }
        }
    }
}