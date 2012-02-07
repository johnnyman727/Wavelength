using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using Wavelength.TestClasses;
using System.IO;
using System.Text;


namespace Wavelength.JSON
{
    public static class JSONHandler
    {

        /// <summary>
        /// Create a String from a JSON Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Thing"></param>
        /// <returns>String of object in JSON Format</returns>
        public static string ObjectToJSON<T>(T Thing)
        {
            // Serialize the object
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = 
                new System.Runtime.Serialization.Json.DataContractJsonSerializer(Thing.GetType());

            // Create temporary stream to write to
            MemoryStream ms = new MemoryStream();

            //Write serializer to the stream
            serializer.WriteObject(ms, Thing);

            //Grab String
            string retVal = UTF8Encoding.UTF8.GetString(ms.ToArray(), 0, ms.ToArray().Length);

            //Delete the memory stream
            ms.Dispose();

            //Return the object string
            return retVal;
        }

        //Find byte size of JSON string

        //Make buffer with message [2^16, 65k]
        //First Two bytes: unsigned, big-endian that is the length of the string
        //Rest is JSON message
    }
}
