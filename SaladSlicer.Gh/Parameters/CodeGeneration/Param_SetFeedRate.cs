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
    /// Represents the Param_FeedRate class
    /// </summary>
    public class Param_SetFeedRate : GH_PersistentGeometryParam<GH_SetFeedRate>
    {
        /// <summary>
        /// Initializes a new instance of the Param_FeedRate class
        /// </summary>
        public Param_SetFeedRate()
          : base(new GH_InstanceDescription("Set Feed Rate", // Parameter name
              "FR", // Component nickname
              "Defines a Feed Rate.", // Description
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
            get { return Properties.Resources.Param_SetFeedRate_Icon; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5D839DEF-81D8-4EDD-94F1-05943B580795"); }
        }
        #endregion

        #region disable pick parameters (we do not allow users to pick parameters)
        /// <summary>
        /// Returns a canceled result. 
        /// Picking items is disabled for this parameter.
        /// </summary>
        /// <param name="values"> Empty list. </param>
        /// <returns> Canceled result. </returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_SetFeedRate> values)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Returns a canceled result. 
        /// Picking items is disabled for this parameter. 
        /// </summary>
        /// <param name="value"> Null item.  </param>
        /// <returns> Canceled result. </returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_SetFeedRate value)
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
