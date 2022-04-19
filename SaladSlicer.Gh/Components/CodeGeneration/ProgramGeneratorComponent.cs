// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Interfaces;
using SaladSlicer.Enumerations;
using SaladSlicer.Gh.Utils;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    /// <summary>
    /// Represent a component that creates the program.
    /// </summary>
    public class ProgramGeneratorComponent : GH_Component
    {
        #region fields
        private List<string> _program = new List<string>();
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public ProgramGeneratorComponent()
          : base("Program Generator", // Component name
              "PG", // Component nickname
              "Program Generator.", // Description
              "Salad Slicer", // Category
              "Code Generation") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Program/Slicer Objects", "O", "All Slicer Objects and Program Objects that can generate a NC program", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Program", "P", "The program as a list with code lines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            List<IProgram> objects = new List<IProgram>();

            // Access the input parameters individually. 
            if (!DA.GetDataList(0, objects)) return;

            // Create the program
            _program.Clear();
            ProgramGenerator programGenerator = new ProgramGenerator();
            try 
            {
                _program = programGenerator.CreateProgram(objects); 
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
            DA.SetDataList(0, _program);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.septenary; }
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
            get { return Properties.Resources.CodeGenerator_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("A387F8ED-44EF-4157-983C-41C88042A67F"); }
        }

        #region save program to file
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Save Program to file", MenuItemClickSaveProgramFile);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Save Program to file" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickSaveProgramFile(object sender, EventArgs e)
        {
            SaveProgram();
        }
        
        /// <summary>
        /// Saves the program to a file
        /// </summary>
        private void SaveProgram()
        {
            // Create save file dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "mpf",
                Filter = "NC Program File|*.mpf",
                Title = "Save a Program file"
            };

            // If result of dialog is OK the file can be saved
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Check the file name
                if (saveFileDialog.FileName != "")
                {
                    // Write RAPID code to file
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false))
                    {
                        for (int i = 0; i < _program.Count; i++)
                        writer.WriteLine(_program[i]);
                    }
                }
            }
        }
        #endregion
    }
}
