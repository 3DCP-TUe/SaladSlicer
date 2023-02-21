// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU Lesser General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Gh.Components.Nozzles
{
    /// <summary>
    /// Represents the reactangular nozzle component.
    /// </summary>
    public class RectangularNozzleComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public RectangularNozzleComponent()
          : base("Rectangular Nozzle", // Component name
              "RecNoz", // Component nickname
              "Creates a rectanglur nozzle.", // Description
              "Salad Slicer", // Category
              "Nozzles") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Box Width", "BW", "The width of the bouding box as a Number.", GH_ParamAccess.item, 70.0);
            pManager.AddNumberParameter("Box Dept", "BD", "The depth of the bounding box as Number.", GH_ParamAccess.item, 50.0);
            pManager.AddNumberParameter("Fillet Radius", "R", "The fillet radius of the bouding box as a Number.", GH_ParamAccess.item, 10.0);
            pManager.AddNumberParameter("Length 1", "L1", "The length of the connector as a Number.", GH_ParamAccess.item, 24.5);
            pManager.AddNumberParameter("Length 2", "L2", "The length between the connector and the unredeuced rectangle as a Number.", GH_ParamAccess.item, 100.0);
            pManager.AddNumberParameter("Length 3", "L3", "The length between the unreduced and reduced rectangle as a Number.", GH_ParamAccess.item, 25.0);
            pManager.AddNumberParameter("Length 4", "L4", "The length of the final reduced cross section.", GH_ParamAccess.item, 25.0);
            pManager.AddNumberParameter("Outer Diameter", "OD", "The outer diameter of the connected pipe as a Number.", GH_ParamAccess.item, 30.0);
            pManager.AddNumberParameter("Inner Diameter", "ID", "The innner diameter of the connected pipe as a Number.", GH_ParamAccess.item, 25.0);
            pManager.AddNumberParameter("Nozzle Width", "NW", "The width of the nozzle as a Number.", GH_ParamAccess.item, 50.0);
            pManager.AddNumberParameter("Nozzle Height", "NH", "The height of the nozzle as a Number.", GH_ParamAccess.item, 10.0);
            pManager.AddBooleanParameter("Divide", "D", "Indicates whether or not the nozzle needs to be divide.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "The rectangular nozzle as a Brep.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            double boxWidth = 70.0;
            double boxDepth = 50.0;
            double filletRadius = 10.0;
            double length1 = 24.5;
            double length2 = 100.0;
            double length3 = 25.0;
            double length4 = 25.0;
            double outerDiameter = 30.0;
            double innerDiameter = 25.0;
            double nozzleWidth = 50.0;
            double nozzleHeight = 10.0;
            bool divide = false;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref boxWidth)) return;
            if (!DA.GetData(1, ref boxDepth)) return;
            if (!DA.GetData(2, ref filletRadius)) return;
            if (!DA.GetData(3, ref length1)) return;
            if (!DA.GetData(4, ref length2)) return;
            if (!DA.GetData(5, ref length3)) return;
            if (!DA.GetData(6, ref length4)) return;
            if (!DA.GetData(7, ref outerDiameter)) return;
            if (!DA.GetData(8, ref innerDiameter)) return;
            if (!DA.GetData(9, ref nozzleWidth)) return;
            if (!DA.GetData(10, ref nozzleHeight)) return;
            if (!DA.GetData(11, ref divide)) return;

            // Declare the output variables
            Brep result = new Brep();

            // Create the new curves
            try
            {
                // Set tolerance from document
                SaladSlicer.Nozzles.Nozzles.Tolerance = DocumentTolerance();

                // Result
                result = SaladSlicer.Nozzles.Nozzles.Rectangular(boxWidth, boxDepth, filletRadius, length1, length2, length3, length4, outerDiameter, innerDiameter, nozzleWidth, nozzleHeight, divide);
            }
            catch (WarningException w)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, w.Message);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }

            // Assign the output parameters
            DA.SetData(0, result);
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
            get { return null; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("CD2B9CB8-8C37-4DD6-9EE2-2E0B84BA96F0"); }
        }
    }
}