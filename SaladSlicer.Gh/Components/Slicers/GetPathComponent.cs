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
    /// Represent a component that creates the path.
    /// </summary>
    public class GetPathComponent : GH_Component
    {
        #region fields
        private bool _expire = false;
        private bool _valueListAdded = false;
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public GetPathComponent()
          : base("Get Path", // Component name
              "P", // Component nickname
              "Defines the path of a slicer object", // Description
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
            pManager.AddCurveParameter("Path", "P", "Path as a Curve.", GH_ParamAccess.item);
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
            DA.SetData(0, slicer.GetPath((PathType)type));
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
            get { return Properties.Resources.GetPath_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C4C3B4B6-AED7-4613-B1E4-94C32C2931B4"); }
        }
    }
}