﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Windows.Forms;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// Salad Slicer Libs
using SaladSlicer.Gh.Goos.Slicers;

namespace SaladSlicer.Gh.Parameters.Slicers
{
    /// <summary>
    /// Represents the Param_ClosedPlanar3DSlicer class
    /// </summary>
    public class Param_ClosedPlanar3DSlicer : GH_PersistentGeometryParam<GH_ClosedPlanar3DSlicer>, IGH_PreviewObject
    {
        /// <summary>
        /// Initializes a new instance of the Param_Object class
        /// </summary>
        public Param_ClosedPlanar3DSlicer()
          : base(new GH_InstanceDescription("Closed Planar 3D Slicer", // Parameter name
              "CP3D", // Component nickname
              "Defines a Closed Planar 3D Slicer object.", // Description
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
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.Param_ClosedPlanar3DSlicer_Icon; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6D5DFEFD-87F8-4550-9F33-ACE278E1E167"); }
        }
        #endregion

        #region disable pick parameters (we do not allow users to pick parameters)
        /// <summary>
        /// Returns a canceled result. 
        /// Picking items is disabled for this parameter.
        /// </summary>
        /// <param name="values"> Empty list. </param>
        /// <returns> Canceled result. </returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_ClosedPlanar3DSlicer> values)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Returns a canceled result. 
        /// Picking items is disabled for this parameter. 
        /// </summary>
        /// <param name="value"> Null item.  </param>
        /// <returns> Canceled result. </returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_ClosedPlanar3DSlicer value)
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

        #region preview methods
        /// <summary>
        /// Implement this function to draw all shaded meshes. 
        /// If the viewport does not support shading, this function will not be called.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Preview_DrawMeshes(args);
        }

        /// <summary>
        /// Implement this function to draw all wire and point previews.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            Preview_DrawWires(args);
        }

        private bool m_hidden = false;

        /// <summary>
        /// Gets or sets the hidden flag for this component. Does not affect Hidden flags on parameters associated with this component.
        /// </summary>
        public bool Hidden
        {
            get { return m_hidden; }
            set { m_hidden = value; }
        }

        /// <summary>
        /// Gets the clipping box for this data. The clipping box is typically the same as the boundingbox.
        /// </summary>
        public BoundingBox ClippingBox
        {
            get { return Preview_ComputeClippingBox(); }
        }

        /// <summary>
        /// If a single parameter is PreviewCapable, so is the component. Override this property if you need special Preview flags.
        /// </summary>
        public bool IsPreviewCapable
        {
            get { return true; }
        }
        #endregion
    }
}