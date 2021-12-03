// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
using System.Drawing;
// Grasshopper Libs
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Gh.Utils
{
    /// <summary>
    /// Represents general helper methods
    /// </summary>
    public static class HelperMethods
    {
        /// <summary>
        /// Creates a Grasshopper value list and returns true if it's created
        /// </summary>
        /// <param name="component">Component to connect to</param>
        /// <param name="inputIndex">Index of the input to connect the list to</param>
        /// <param name="enumType">Enumeration to take values from</param>
        /// <returns></returns>
        public static bool CreateValueList(GH_Component component,int inputIndex, Type enumType)
        {
            if (component.Params.Input[inputIndex].SourceCount == 0)
            {
                var parameter = component.Params.Input[inputIndex];

                // Creates the empty value list
                GH_ValueList obj = new GH_ValueList();
                obj.CreateAttributes();
                obj.ListMode = GH_ValueListMode.DropDown;
                obj.ListItems.Clear();

                // Add the items to the value list
                string[] names = Enum.GetNames(enumType);
                int[] values = (int[])Enum.GetValues(enumType);

                for (int i = 0; i < names.Length; i++)
                {
                    obj.ListItems.Add(new GH_ValueListItem(names[i], values[i].ToString()));
                }

                // Make point where the valuelist should be created on the canvas
                obj.Attributes.Pivot = new PointF(parameter.Attributes.InputGrip.X - 120, parameter.Attributes.InputGrip.Y - 11);

                // Add the value list to the active canvas
                Instances.ActiveCanvas.Document.AddObject(obj, false);

                // Connect the value list to the input parameter
                parameter.AddSource(obj);

                // Collect data
                parameter.CollectData();

                // Expire value list
                obj.ExpireSolution(true);

                // Return that it's created
                return true; 
            }
            else
            {
                // Return that it isn't created
                return false;
            }
        }

        /// <summary>
        /// Returns a datatree with doubles created from a nested list with doubles.
        /// </summary>
        /// <param name="data"> The nested list with doubles. </param>
        /// <returns> The datatree with doubles. </returns>
        public static DataTree<double> ListInListToDataTree(List<List<double>> data)
        {
            DataTree<double> result = new DataTree<double>();

            for (int i = 0; i < data.Count; i++)
            {
                result.AddRange(data[i], new GH_Path(i));
            }

            return result;
        }

        /// <summary>
        /// Returns a datatree with planes created from a nested list with planes.
        /// </summary>
        /// <param name="data"> The nested list with planes. </param>
        /// <returns> The datatree with planes. </returns>
        public static DataTree<Plane> ListInListToDataTree(List<List<Plane>> data)
        {
            DataTree<Plane> result = new DataTree<Plane>();

            for (int i = 0; i < data.Count; i++)
            {
                result.AddRange(data[i], new GH_Path(i));
            }

            return result;
        }

        /// <summary>
        /// Returns a datatree with vectors created from a nested list with vectors.
        /// </summary>
        /// <param name="data"> The nested list with vectors. </param>
        /// <returns> The datatree with vectors. </returns>
        public static DataTree<Vector3d> ListInListToDataTree(List<List<Vector3d>> data)
        {
            DataTree<Vector3d> result = new DataTree<Vector3d>();

            for (int i = 0; i < data.Count; i++)
            {
                result.AddRange(data[i], new GH_Path(i));
            }

            return result;
        }
    }
}

