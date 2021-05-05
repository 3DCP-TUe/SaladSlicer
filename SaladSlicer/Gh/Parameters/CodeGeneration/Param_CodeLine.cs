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
using SaladSlicer.Gh.Goos.CodeGeneration;

namespace SaladSlicer.Gh.CodeGeneration.Parameters
{
    /// <summary>
    /// Represents the Param_CodeLine class
    /// </summary>
    public class Param_CodeLine : GH_PersistentParam<GH_CodeLine>
    {
        /// <summary>
        /// Initializes a new instance of the Param_CodeLine class
        /// </summary>
        public Param_CodeLine()
          : base(new GH_InstanceDescription("Code Line", // Parameter name
              "CL", // Component nickname
              "Defines a custom Code Line.", // Description
              "Salad Slicer", // Category
              "Parameters")) // Subcategory)
        { 
        }

        #region properties
        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ExampleIcon; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("23A0C393-40D8-4BD8-80E4-B45631E3FA7D"); }
        }
        #endregion

        // We do not allow users to pick parameters, therefore the following 4 methods disable all this ui.
        #region disable pick parameters
        protected override GH_GetterResult Prompt_Plural(ref List<GH_CodeLine> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_CodeLine value)
        {
            return GH_GetterResult.cancel;
        }

        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
            return item;
        }

        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
            return item;
        }
        #endregion
    }
}
