// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
using GH_IO.Serialization;
// Salad Slicer Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Interfaces;

namespace SaladSlicer.Gh.Components.CodeGeneration
{
    /// <summary>
    /// Represent a component that creates the program.
    /// </summary>
    public class ProgramGeneratorComponent : GH_Component
    {
        #region fields
        private List<string> _program = new List<string>();
        private Curve _path = null;
        private bool _previewPath = true;
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
            
            // Declare the output variables
            ProgramGenerator programGenerator = new ProgramGenerator();
            
            // Create the program
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

            // Create the
            if (_previewPath == true)
            {
                try
                {
                    _path = programGenerator.CreatePath(objects);
                }
                catch (Exception error)
                {
                    // Only for debugging
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, error.Message);
                }
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
            get { return Properties.Resources.ProgramGenerator_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("E1335F06-C7CB-4589-A88A-A0FDEC6FF76B"); }
        }

        #region menu items
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Save Program to file", MenuItemClickSaveProgramFile);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Preview Path", MenuItemClickPreviewPath, true, _previewPath);
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
        /// Handles the event when the custom menu item "Preview Path" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickPreviewPath(object sender, EventArgs e)
        {
            RecordUndoEvent("Preview Path");
            _previewPath = !_previewPath;
            this.ExpireSolution(true);
        }

        /// <summary>
        /// Write all required data for deserialization to an IO archive.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Preview Path", _previewPath);
            return base.Write(writer);
        }

        /// <summary>
        /// Read all required data for deserialization from an IO archive.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            _previewPath = reader.GetBoolean("Preview Path");
            return base.Read(reader);
        }
        #endregion

        #region save program to file
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

        #region preview path
        /// <summary>
        /// Gets the clipping box for all preview geometry drawn by this component and all associated parameters.
        /// </summary>
        public override BoundingBox ClippingBox
        {
            get { return GetBoundingBox(); }
        }

        /// <summary>
        /// Returns the bounding box for all preview geometry drawn by this component.
        /// </summary>
        /// <returns></returns>
        private BoundingBox GetBoundingBox()
        {
            BoundingBox result = new BoundingBox();

            // Get bouding box of all the output parameters
            for (int i = 0; i < Params.Output.Count; i++)
            {
                if (Params.Output[i] is IGH_PreviewObject previewObject)
                {
                    result.Union(previewObject.ClippingBox);
                }
            }

            // Add bounding box of custom preview
            if (_previewPath)
            {
                if (_path != null)
                {
                    result.Union(_path.GetBoundingBox(false));
                }
            }

            return result;
        }

        /// <summary>
        /// Draw preview wires for this component and all associated parameters.
        /// </summary>
        /// <param name="args"> Display data used during preview drawing. </param>
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);

            if (_previewPath == true)
            {
                if (_path != null)
                {
                    args.Display.DrawCurve(_path, args.WireColour, args.DefaultCurveThickness);
                }
            }
        }
        #endregion
    }
}
