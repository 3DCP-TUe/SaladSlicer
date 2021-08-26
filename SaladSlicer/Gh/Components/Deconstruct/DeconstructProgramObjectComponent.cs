// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Core.CodeGeneration;
using SaladSlicer.Core.Slicers;
using SaladSlicer.Gh.Parameters.CodeGeneration;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    /// <summary>
    /// Represent a component that deconstruct a Porgram Object.
    /// </summary>
    public class DeconstructProgramObjectComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public DeconstructProgramObjectComponent()
          : base("Deconstruct Program Object", // Component name
              "DPO", // Component nickname
              "Deconstructs a Program Object", // Description
              "Salad Slicer", // Category
              "Deconstruct") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Object(), "Program Object", "PO", "Program Object.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Path", "P", "Path as a Curve.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Frames", "F", "Frames as a list with Planes.", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Start Frame", "S", "Start frame as a Plane.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("End Frame", "E", "End frame as a Plane.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            IObject programObject = new ClosedPlanar2DSlicer();
            
            // Access the input parameters individually. 
            if (!DA.GetData(0, ref programObject)) return;

            // Assign the output parameters
            DA.SetData(0, programObject.GetPath());
            DA.SetDataList(1, programObject.Frames);
            DA.SetData(2, programObject.FrameAtStart);
            DA.SetData(3, programObject.FrameAtEnd);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
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
            get { return Properties.Resources.ExampleIcon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ACF32A02-B093-4D72-822C-C51D5310C8CE"); }
        }
    } 
}