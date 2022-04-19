// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Utils;

namespace SaladSlicer.Gh.Components.Utilities
{
    /// <summary>
    /// Represents the component that converts a velocity specified in mm/min to mm/s.
    /// </summary>
    public class ConvertVelocityMmsMmminComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public ConvertVelocityMmsMmminComponent()
          : base("Convert Velocity - mm/min to mm/s", // Component name
              "CV", // Component nickname
              "Convert a velocity specified in mm/min to mm/s.", // Description
              "Salad Slicer", // Category
              "Utilities") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Velocity", "V", "Velocity in mm/min as a number.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Velocity", "V", "Velocity in mm/s as a number.",  GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            double velocity = 0.0;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref velocity)) return;

            // Assign the output parameters
            DA.SetData(0, HelperMethods.MillimetersMinuteToMillimetersSecond(velocity));
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
            get { return Properties.Resources.ConvertVelocityMmsMmmin_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("74677E15-2CA1-4434-9768-29470D9EF506"); }
        }
    }
}