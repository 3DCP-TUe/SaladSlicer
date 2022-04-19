// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// Salad Slicer Libs
using SaladSlicer.Geometry.Seams;

namespace SaladSlicer.Gh.Components.Geometry
{
    /// <summary>
    /// Represents the component that set a seam based on the parameter.
    /// </summary>
    public class SeamAtParamComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public SeamAtParamComponent()
          : base("Seam at Parameter", // Component name
              "SP", // Component nickname
              "Redefines the startpoint of a closed curve based on a parameter between 0 and 1.", // Description
              "Salad Slicer", // Category
              "Geometry") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve", GH_ParamAccess.item);
            pManager.AddNumberParameter("Parameter", "t", "A parameter that redefines the startpoint of the curve.",  GH_ParamAccess.item, 0.0);
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
            double param = 0.0;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetData(1, ref param)) return;

            // Declare the output variable
            Curve result = curve;

            // Create the new curve
            try
            {
                result = Locations.SeamAtParam(curve, param);
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
            get { return Properties.Resources.SeamAtParameter_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F791CB38-CA24-4C92-9343-648CF6AE1203"); }
        }

    }
}