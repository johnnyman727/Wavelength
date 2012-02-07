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
using Wavelength.WLSocket;

namespace Wavelength.ShrapnelProtocol
{
    public static class ShrapnelHandler
    {

        public static void SendMessage(String json_message)
        {
            ShrapnelMessage shrapnelMessage = new ShrapnelMessage(json_message);
            SendMessageHelper(shrapnelMessage);
        }

        public static void SendMessage(String json_message, Byte[] binaryAttachment)
        {
            ShrapnelMessage shrapnelMessage = new ShrapnelMessage(json_message, binaryAttachment);
            SendMessageHelper(shrapnelMessage);
        }

        private static void SendMessageHelper(ShrapnelMessage shrapnelMessage)
        {
            SocketClient socketClient = new SocketClient();     
       
            //Conect to socket
            socketClient.Connect();

            // Send json length
            socketClient.Send(shrapnelMessage.PackUp(shrapnelMessage._jsonLengthUnsigned));

            // If there is a binary attachment, send length, else send 00
            if (shrapnelMessage._binaryLengthUnsigned > 0)
            {
                socketClient.Send(shrapnelMessage.PackUp(shrapnelMessage._jsonLengthUnsigned));
            }
            else
            {
                Byte[] noAttachment = new Byte[2];
                socketClient.Send(noAttachment); 
            }

            //send json
            socketClient.Send(shrapnelMessage._jsonMessage);

            //send binary if it exists
            if (shrapnelMessage._binaryLengthUnsigned > 0)
            {
                socketClient.Send(shrapnelMessage._binaryAttachment);
            }
        }

    }
}
