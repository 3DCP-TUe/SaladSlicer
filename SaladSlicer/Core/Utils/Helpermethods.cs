// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SaladSlicer.Core.Utils
{
    /// <summary>
    /// Represents general helper methods
    /// </summary>
    public static class Helpermethods
    {
        /// <summary>
        /// Serializes a common object to a byte array. 
        /// Typically used for serializing meshes and data inside Goo classes.
        /// Taken from <https://github.com/RobotComponents/RobotComponents> (GPL v3).
        /// </summary>
        /// <param name="obj"> The common object. </param>
        /// <returns> The byte array. </returns>
        public static byte[] ObjectToByteArray(Object obj)
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
        public static Object ByteArrayToObject(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return (Object)formatter.Deserialize(stream);
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
    }
}
