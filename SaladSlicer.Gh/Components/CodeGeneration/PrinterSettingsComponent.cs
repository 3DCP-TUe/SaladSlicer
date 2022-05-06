// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Gh.Utils;
using SaladSlicer.Enumerations;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    /// <summary>
    /// Represent a component that generates a custom Code Line.
    /// </summary>
    public class PrinterSettingsComponent : GH_Component
    {
        #region fields
        private bool _expire = false;
        private bool _valueList1Added = false;
        private bool _valueList2Added = false;
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public PrinterSettingsComponent()
          : base("Program Settings", // Component name
              "S", // Component nickname
              "Defines or redefines the settings for the program", // Description
              "Salad Slicer", // Category
              "Code Generation") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Program Type", "T", "Set the program type for the program.", GH_ParamAccess.item,0);
            pManager.AddIntegerParameter("Interpolation Type", "I", "Set the interpolation type for the program.", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Hot End Temperature", "HT", "Set the temperature of the hot end. Negative values are ignored.", GH_ParamAccess.item, -1);
            pManager.AddNumberParameter("Bed Temperature", "BT", "Set the bed temperature. Negative values are ignored.", GH_ParamAccess.item, -1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Settings Object", "SO", "Feedrate as a Program Object.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Create a value list
            if (_valueList1Added == false)
            {
                _expire = HelperMethods.CreateValueList(this, 0, typeof(ProgramTypes));
                _valueList1Added = true;
            }
            if (_valueList2Added == false)
            {
                _expire = HelperMethods.CreateValueList(this, 1, typeof(InterpolationTypes));
                _valueList2Added = true;
            }

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Declare variable of input parameters
            int programType = new int();
            int interpolation = new int();
            double hotend = new double();
            double bed = new double();

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref programType)) return;
            if (!DA.GetData(1, ref interpolation)) return;
            if (!DA.GetData(2, ref hotend)) return;
            if (!DA.GetData(3, ref bed)) return;

            // Declare the output variables
            SaladSlicer.CodeGeneration.PrinterSettings programSetting = new SaladSlicer.CodeGeneration.PrinterSettings();
            
            // Create the program settings
            try
            {
                programSetting = new SaladSlicer.CodeGeneration.PrinterSettings(programType, interpolation, hotend, bed);
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
            DA.SetData(0, programSetting);
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
            get { return Properties.Resources.ProgramSettings_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C837C754-739D-4691-8643-27B61299086D"); }
        }
    }
}