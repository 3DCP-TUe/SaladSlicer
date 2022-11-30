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
using SaladSlicer.Interfaces;
using SaladSlicer.Slicers;

namespace SaladSlicer.Gh.Components.Slicers
{
    /// <summary>
    /// Represent a component that generates a custom Code Line.
    /// </summary>
    public class AddVariableComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public AddVariableComponent()
          : base("Add Variable", // Component name
              "AV", // Component nickname
              "Defines an additional variable that is controlled by the code besides the standard X,Y and Z axis. Examples can be extruders (E) feedrate (F) or a rotational axis (C).", // Description
              "Salad Slicer", // Category
              "Slicers") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Slicer Object", "SO", "Slicer Object to which the variable should be added.", GH_ParamAccess.item);
            pManager.AddTextParameter("Prefix", "P", "Prefix.", GH_ParamAccess.item);
            pManager.AddTextParameter("Values", "V", "Values of the axis to be added. Should have equal size as the frames in the object.", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Slicer Object", "SO", "Slicer Object to which the axis is added.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            IAddVariable slicer = new CurveSlicer();
            string prefix = "";
            List<List<string>> values = new List<List<string>>();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref slicer)) return;
            if (!DA.GetData(1, ref prefix)) return;
            if (!DA.GetDataTree(2, out GH_Structure<GH_String> tree)) return;

            // Create the code line
            IAddVariable slicerCopy = slicer.DuplicateAddVariableObject();
            
            for (int i = 0; i < tree.PathCount; i++)
            {
                List<string> valueTemp = new List<string>();
                GH_Path path = tree.get_Path(i);
                List<GH_String> branch = (List<GH_String>)tree.get_Branch(path);
                
                for (int j =0; j < branch.Count; j++)
                {
                    valueTemp.Add(branch[j].Value);
                }
                
                values.Add(valueTemp);
            }
            
            try
            {
                slicerCopy.AddVariable(prefix, values);
            }
            catch (WarningException warning)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, warning.Message);
            }
            catch (Exception error)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, error.Message);
            }

            // Assign the output parameters
            DA.SetData(0, slicerCopy);
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
            get { return Properties.Resources.AddVariable_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("65CE8F8A-C5DC-4249-9F6B-4591D2D31887"); }
        }
    }
}