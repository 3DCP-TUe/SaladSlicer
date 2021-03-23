// System Libs
using System;
using System.IO;
using System.Collections.Generic;
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
    }
}
