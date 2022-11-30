// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
// Salad Libs
using SaladSlicer.Interfaces;

namespace SaladSlicer.Utils
{
    /// <summary>
    /// Represents general helper methods
    /// </summary>
    public static class HelperMethods
    {
        /// <summary>
        /// Serializes a common object to a byte array. 
        /// Typically used for serializing meshes and data inside Goo classes.
        /// Taken from <https://github.com/RobotComponents/RobotComponents> (GPL v3).
        /// </summary>
        /// <param name="obj"> The common object. </param>
        /// <returns> The byte array. </returns>
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null) { return null; }

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes a byte array to a common object. 
        /// Typically used for deserializing meshes and data inside Goo classes.
        /// Taken from <https://github.com/RobotComponents/RobotComponents> (GPL v3).
        /// </summary>
        /// <param name="data"> The byte array. </param>
        /// <returns> The common object. </returns>
        public static object ByteArrayToObject(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Converts a velocity specified in mm/s to mm/min.
        /// </summary>
        /// <param name="velocity"> The velocity in mm/s. </param>
        /// <returns> The velocity in mm/min. </returns>
        public static double MillimetersSecondToMillimetersMinute(double velocity)
        {
            return velocity * 60.0;
        }

        /// <summary>
        /// Converts a velocity specified mm/min to mm/s.
        /// </summary>
        /// <param name="velocity"> The velocity in mm/min. </param>
        /// <returns> The velocity in mm/s. </returns>
        public static double MillimetersMinuteToMillimetersSecond(double velocity)
        {
            return velocity / 60.0;
        }

        /// <summary>
        /// Returns the version number of the assembly (as MAJOR.MINOR.PATCH).
        /// </summary>
        /// <returns> The version number. </returns>
        public static string GetVersionNumber()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string[] split = fileVersionInfo.ProductVersion.Split('.');
            string version = split[0] + "." + split[1] + "." + split[2];

            return version;
        }

        /// <summary>
        /// Matches the data structure of the added variable.
        /// </summary>
        /// <param name="addVariable"> The AddVariable interface to check. </param>
        /// <param name="data"> The data to match with the AddVariable structure. </param>
        /// <returns> Matched data list. </returns>
        public static List<List<string>> MatchAddedVariable(IAddVariable addVariable, List<List<string>> data)
        {
            // Data is already matching
            if (CheckAddedVariable(addVariable, data))
            {
                return data;
            }

            // User defined one list that matches the number of frames
            if (data.Count == 1)
            {
                int framesCounter = 0;

                for (int i = 0; i < addVariable.FramesByLayer.Count; i++)
                {
                    framesCounter += addVariable.FramesByLayer[i].Count;
                }

                if (framesCounter == data[0].Count)
                {
                    List<List<string>> result1 = new List<List<string>>() { };

                    int counter = 0;

                    for (int i = 0; i < addVariable.FramesByLayer.Count; i++)
                    {
                        List<string> temp = new List<string>() { };

                        for (int j = 0; j < addVariable.FramesByLayer[i].Count; j++)
                        {
                            temp.Add(data[0][counter]);
                            counter++;
                        }

                        result1.Add(temp);
                    }

                    return result1;
                }
            }

            // Otherwise: asynchronisch data mathcing
            List<List<string>> result2 = new List<List<string>>() { };

            for (int i = 0; i < addVariable.FramesByLayer.Count; i++)
            {
                List<string> temp = new List<string>() { };
                List<string> sub;

                if (i < data.Count)
                {
                    sub = data[i];
                }
                else
                {
                    sub = data[data.Count - 1];
                }

                for (int j = 0; j < addVariable.FramesByLayer[i].Count; j++)
                {
                    if (j < sub.Count)
                    {
                        temp.Add(sub[j]);
                    }
                    else
                    {
                        temp.Add(sub[sub.Count - 1]);
                    }
                }

                result2.Add(temp);
            }

            return result2;
        }

        /// <summary>
        /// Checks the data structure of the added variable.
        /// </summary>
        /// <param name="addVariable"> The Added Variable interface to check. </param>
        /// <param name="data"> The data to check with the AddVariable structure. </param>
        /// <returns> Indicates whether the data structure matches or not. </returns>
        public static bool CheckAddedVariable(IAddVariable addVariable, List<List<string>> data)
        {
            // Check the number of layers
            if (addVariable.FramesByLayer.Count != data.Count)
            {
                return false;
            }

            // Check frames by layer
            for (int i = 0; i < addVariable.FramesByLayer.Count; i++)
            {
                if (addVariable.FramesByLayer[i].Count != data[i].Count)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
