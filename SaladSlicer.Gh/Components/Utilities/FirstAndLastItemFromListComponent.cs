// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;

namespace SaladSlicer.Gh.Components.Utilities
{
    /// <summary>
    /// Represents the component that picks the first and last item item from a list. 
    /// </summary>
    public class FirstAndLastItemFromListComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public FirstAndLastItemFromListComponent()
          : base("First and last item from list", // Component name
              "FiLi", // Component nickname
              "Picks the first and last item from a list.", // Description
              "Salad Slicer", // Category
              "Utilities") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("List", "L", "Base list", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("First item", "F", "Item at the first index.",  GH_ParamAccess.item);
            pManager.AddNumberParameter("Last item", "L", "Item at the last index.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            List<object> list = new List<object>();

            // Access the input parameters individually. 
            if (!DA.GetDataList(0, list)) return;

            // Assign the output parameters
            DA.SetData(0, list[0]);
            DA.SetData(1, list[list.Count - 1]);
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
            get { return null; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("761636F3-BE09-476A-A0CC-F6B9B9C7E457"); }
        }
    }
}