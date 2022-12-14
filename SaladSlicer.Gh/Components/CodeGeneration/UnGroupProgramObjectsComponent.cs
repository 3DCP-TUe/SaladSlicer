// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU Lesser General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Gh.Parameters.CodeGeneration;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    /// <summary>
    /// Represent a component that generates a set with program objects.
    /// </summary>
    public class UnGroupProgramObjectsComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public UnGroupProgramObjectsComponent()
          : base("Ungroup Program Objects", // Component name
              "UG", // Component nickname
              "Ungroup a set of program objects.", // Description
              "Salad Slicer", // Category
              "Code Generation") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ProgramGroup(), "Program Group", "G", "Group with program objects", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Program Objects", "PO", "Program objects inside group.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            ProgramGroup group = new ProgramGroup();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref group)) return;

            // Assign the output parameters
            DA.SetDataList(0, group.Objects);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }
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
            get { return Properties.Resources.UnGroupProgramObjects_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D2B0943A-E765-4B29-8014-614F22016471"); }
        }
    }
}