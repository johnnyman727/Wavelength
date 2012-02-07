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
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Wavelength.ShrapnelProtocol
{
    public class ShrapnelMessage
    {
        // JSON Message
        public String _jsonMessage;

        // JSON Length
        int _jsonLength;

        //JSON Unsigned Length
        public UInt16 _jsonLengthUnsigned;

        // Binary Attatchment
        public Byte[] _binaryAttachment = null;

        // Binary Length
        int _binaryLength;

        // Unsigned Binary Length of JSON 
        public UInt16 _binaryLengthUnsigned = 0;

        //Boolean _readyToSend = false;

        public ShrapnelMessage(String json)
        {
            // Find length of json String as binary
            _jsonLength = getBinaryStringLength(json);

            //Set Unsigned Length
            _jsonLengthUnsigned = Convert.ToUInt16(_jsonLength);

            // Store json String
            _jsonMessage = json;

            //TODO: Check if string is too big and throw exception

            // Set ready to send
            //_readyToSend = true;
        }

        public ShrapnelMessage(String json, Byte[] binaryAttachment) : this(json)
        {
            // Set Binary Attachment
            _binaryAttachment = binaryAttachment;

            // Set Binary Attachment length
            _binaryLength = binaryAttachment.Length;

            // Set Binary Attachment u16 length
            _binaryLengthUnsigned = Convert.ToUInt16(_binaryLength);
        }

        public int getBinaryStringLength(string json)
        {
            // Set up utf 8 encoder
            UTF8Encoding utf8 = new UTF8Encoding();
             
            // Set up Memory Stream
            MemoryStream ms = new MemoryStream();

            // Set up binary writer with stream and encoding
            BinaryWriter bw = new BinaryWriter(ms, utf8);

            // Write json to stream 
            bw.Write(json.ToCharArray());

            //Return the length
            return (int)ms.Length;
        }

        public byte[] PackUp(UInt16 length)
        {
            //Return u16 as a byte array
            return BitConverter.GetBytes(length);
        }
    }
}
