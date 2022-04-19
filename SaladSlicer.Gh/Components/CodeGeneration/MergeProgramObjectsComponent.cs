// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
// Salad Slicer Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Interfaces;
using SaladSlicer.Slicers;
using SaladSlicer.Gh.Utils;
using SaladSlicer.Enumerations;
using SaladSlicer.Gh.Parameters.Slicers;
using SaladSlicer.Gh.Goos.CodeGeneration;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    /// <summary>
    /// Represent a component that generates a custom Code Line.
    /// </summary>
    public class MergeProgramObjectsComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public MergeProgramObjectsComponent()
          : base("Merge Program Object", // Component name
              "MPO", // Component nickname
              "Merges Program Objects so that they appear on a single line the program.", // Description
              "Salad Slicer", // Category
              "Code Generation") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Program Objects", "POs", "List of program object that should be added together.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Program Object", "PO", "Merged Program Objects", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            List<IProgram> list = new List<IProgram>();

            // Access the input parameters individually. 
            if (!DA.GetDataList(0, list)) return;

            // Create the code line
            string newCodeLine = "";
            for (int i = 0; i < list.Count; i++)
            {
                string string2 = "";
                try 
                {
                    string2 = list[i].ToSingleString();
                }
                catch (WarningException warning)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, warning.Message);
                }
                catch (Exception error)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, error.Message);
                }
                newCodeLine = $"{newCodeLine} {string2}";
            }
            CodeLine result = new CodeLine(newCodeLine);

            // Assign the output parameters
            DA.SetData(0, result);
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
            get { return Properties.Resources.MergeProgramObject_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0D084C71-4A4C-4230-BC45-6412FFA8073B"); }
        }
    }
}