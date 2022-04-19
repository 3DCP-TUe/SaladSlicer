// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Lib
using Rhino.Geometry;
// Salad Slicer Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Gh.Parameters.CodeGeneration;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    public class SetTemperatureComponent :GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public SetTemperatureComponent()
          : base("Set Temperature", // Component name
              "ST", // Component nickname
              "Defines the temperature settings as a Program Object", // Description
              "Salad Slicer", // Category
              "Code Generation") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Hotend", "H", "Hotend Temperature, not used if zero.", GH_ParamAccess.item,180);
            pManager.AddNumberParameter("Bed", "B", "Bed Temperature, not used if zero.", GH_ParamAccess.item, 50);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Program Object", "PO", "Temperature settings as a Program Object.", GH_ParamAccess.item);
        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            double hotEnd =new double();
            double bed = new double();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref hotEnd)) return;
            if (!DA.GetData(1, ref bed)) return;

            // Create the code line
            SetTemperature tempObject = new SetTemperature(hotEnd,bed);

            // Assign the output parameters
            DA.SetData(0, tempObject);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
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
            get { return new Guid("2FCE3C3B-94D8-4F40-8B1B-94160F6C4968"); }
        }
    }
}
