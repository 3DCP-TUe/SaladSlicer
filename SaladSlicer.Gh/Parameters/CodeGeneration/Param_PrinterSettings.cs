// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU Lesser General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Windows.Forms;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Salad Slicer Libs
using SaladSlicer.Gh.Goos.CodeGeneration;

namespace SaladSlicer.Gh.Parameters.CodeGeneration
{
    /// <summary>
    /// Represents the Param_PrinterSettings class
    /// </summary>
    public class Param_PrinterSettings : GH_PersistentGeometryParam<GH_PrinterSettings>
    {
        /// <summary>
        /// Initializes a new instance of the Param_PrinterSettings class
        /// </summary>
        public Param_PrinterSettings()
          : base(new GH_InstanceDescription("Printer Settings", // Parameter name
              "PS", // Component nickname
              "Defines the Printer Settings.", // Description
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
            get { return Properties.Resources.Param_PrinterSettings_Icon; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4BE44EC4-6996-4607-B24E-1B701FC86EE7"); }
        }
        #endregion

        #region disable pick parameters (we do not allow users to pick parameters)
        /// <summary>
        /// Returns a canceled result. 
        /// Picking items is disabled for this parameter.
        /// </summary>
        /// <param name="values"> Empty list. </param>
        /// <returns> Canceled result. </returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_PrinterSettings> values)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Returns a canceled result. 
        /// Picking items is disabled for this parameter. 
        /// </summary>
        /// <param name="value"> Null item.  </param>
        /// <returns> Canceled result. </returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_PrinterSettings value)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Returns a hidden tool strip menu item for picking a single item.
        /// Picking items is disabled for this parameter.
        /// </summary>
        /// <returns> A hidden  tool strip menu item. </returns>
        protected override ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            ToolStripMenuItem item = new ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };

            return item;
        }

        /// <summary>
        /// Returns a hidden tool strip menu item for picking multiple items.
        /// Picking items is disabled for this parameter.
        /// </summary>
        /// <returns> A hidden tool strip menu item. </returns>
        protected override ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            ToolStripMenuItem item = new ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };

            return item;
        }
        #endregion
    }
}
