// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Drawing;
//Grasshopper
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
//SaladSlicer
using SaladSlicer.Gh.Components.Geometry;



namespace SaladSlicer.Gh.Utils
{
    /// <summary>
    /// Represents general helper methods
    /// </summary>
    public static class HelperMethodsGh
    {
        /// <summary>
        /// Creates a Grasshopper value list
        /// </summary>
        /// <param name="component">Component to connect to</param>
        /// <param name="inputIndex">Intex of the input to connect the list to</param>
        /// <param name="enumType">Enumeration to take values from</param>
        /// <param name="expire">_expire field parameter</param>
        /// <returns></returns>
        public static bool CreateValueList(GH_Component component,int inputIndex, Type enumType, bool expire)
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
                //obj.ToggleItem(1);

                // Connect the value list to the input parameter
                parameter.AddSource(obj);

                // Collect data
                parameter.CollectData();

                // Created
                expire = true;

                // Expire value list
                obj.ExpireSolution(true);
            }
            return expire;
        }
    }


}

