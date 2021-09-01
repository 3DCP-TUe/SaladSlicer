// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Core.CodeGeneration;
using SaladSlicer.Core.Interfaces;
using SaladSlicer.Core.Slicers;
using SaladSlicer.Gh.Utils;
using SaladSlicer.Core.Enumerations;
using SaladSlicer.Gh.Parameters.Slicers;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    /// <summary>
    /// Represent a component that generates a custom Code Line.
    /// </summary>
    public class AddVariableComponent : GH_Component
    {
        #region fields
        private bool _expire = false;
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public AddVariableComponent()
          : base("Add Variable", // Component name
              "Add Var", // Component nickname
              "Defines an additional variable that is controlled by the code besides the standard X,Y and Z axis. Examples can be extruders (E) feedrate (F) or a rotational axis (C).", // Description
              "Salad Slicer", // Category
              "Code Generation") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Slicer Object", "SO", "Slicer Object to which the variable should be added.", GH_ParamAccess.item);
            pManager.AddTextParameter("Prefix", "P", "Prefix.", GH_ParamAccess.item,"E");
            pManager.AddIntegerParameter("Method", "M", "Method that is used to calculate the value of the added variable.", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Factor", "F", "Factor to use.", GH_ParamAccess.item,12);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Slicer Object", "SO", "Slicer Object to which the axis is added", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Create a value list
            _expire = HelperMethods.CreateValueList(this, 2, typeof(AddVariableMethod));

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }
            // Declare variable of input parameters

            IAddVariable slicerObject = new CurveSlicer();
            string prefix = "";
            double factor = new double();
            int  method = new int();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref slicerObject)) return;
            if (!DA.GetData(1, ref prefix)) return;
            if (!DA.GetData(2, ref method)) return;
            if (!DA.GetData(3, ref factor)) return;

            // Warnings
            if (slicerObject.Contours.Count<2 && method == 1) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Cannot use the ByLayerdistance method on less than two curves."); }

            // Create the code line
            IAddVariable newSlicerObject = slicerObject.DuplicateAddVariableObject();
            if (method == 0)
            {
                newSlicerObject.AddVariableByDisplacement(prefix, factor);
            }
            else if (method == 1)
            {
                newSlicerObject.AddVariableByLayerDistance(prefix, factor);
            }
            
            // Assign the output parameters
            DA.SetData(0, newSlicerObject);
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
            get { return Properties.Resources.ExampleIcon; }
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