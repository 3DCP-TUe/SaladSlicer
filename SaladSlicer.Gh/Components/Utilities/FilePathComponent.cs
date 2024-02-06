// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Linq;
// Grasshopper Libs
using Grasshopper.Kernel;

namespace SaladSlicer.Gh.Components.Utilities
{
    /// <summary>
    /// Represents the component gives the Grashopper file path
    /// </summary>
    public class FilePathComponent : GH_Component
    {
        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public FilePathComponent()
          : base("File Path", // Component name
              "FP", // Component nickname
              "Gives the file path of this Grasshopper file.", // Description
              "Salad Slicer", // Category
              "Utilities") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // This component has no input parameters
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("File Path", "FP", "File path of this Grasshopper file.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare the path
            GH_Document doc = this.OnPingDocument();
            string filePath = doc.FilePath;
            string[] filePathSplit = filePath.Split('\\');
            filePathSplit = filePathSplit.Take(filePathSplit.Length - 1).ToArray();
            string path = string.Join("\\", filePathSplit);

            // Assign the output parameters
            DA.SetData(0, path);
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
            get { return Properties.Resources.FilePath_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("417530E5-4B2D-4650-BF54-4878A9675C14"); }
        }
    }
}