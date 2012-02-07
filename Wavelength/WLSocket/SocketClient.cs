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
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Diagnostics;
using Wavelength.ShrapnelProtocol;

namespace Wavelength.WLSocket
{
    public class SocketClient
    {
        // Hostname of socket to connect to
        String _hostName;

        // Port Number of socket to connect to
        int _portNumber;

        // Cached Socket object that will be used by each call for the lifetime of this class
        Socket _socket = null;

        // Signaling object used to notify when an asynchronous operation is completed
        static ManualResetEvent _clientDone = new ManualResetEvent(false);

        // Define a timeout in milliseconds for each asynchronous call. If a response is not received within this 
        // timeout period, the call is aborted.
        const int TIMEOUT_MILLISECONDS = 10000;

        // The maximum size of the data buffer to use with the asynchronous socket methods
        const int MAX_BUFFER_SIZE = 2048;

        /// <summary>
        /// Construct a socket class by passisng in hostname portname
        /// </summary>
        /// <param name="hostName">The name of the host</param>
        /// <param name="portNumber">The port number to connect</param>
        /// <returns>A string representing the result of this connection attempt</returns>
        public SocketClient(String hostName, int portNumber)
        {
            //Set hostname and portNumber properties of socket
            _hostName = hostName;
            _portNumber = portNumber;
        }

        /// <summary>
        /// Construct a socket class with default hostname and portnumber
        /// </summary>
        /// <returns>A string representing the result of this connection attempt</returns>
        public SocketClient()
        {
            //Set default hostname and portNumber properties of socket
            _hostName = "10.41.88.47";
            _portNumber = 1337;
        }

        /// <summary>
        /// Attempt a TCP socket connection to the given host over the given port
        /// </summary>
        /// <param name="hostName">The name of the host</param>
        /// <param name="portNumber">The port number to connect</param>
        /// <returns>A string representing the result of this connection attempt</returns>
        public string Connect()
        {
            string result = string.Empty;

            // Create DnsEndPoint. The hostName and port are passed in to this method.
            DnsEndPoint hostEntry = new DnsEndPoint(_hostName, _portNumber);

            // Create a stream-based, TCP socket using the InterNetwork Address Family. 
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Create a SocketAsyncEventArgs object to be used in the connection request
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.RemoteEndPoint = hostEntry;

            // Inline event handler for the Completed event.
            // Note: This event handler was implemented inline in order to make this method self-contained.
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
            {
                Debug.WriteLine("Beginning Connection Attempt");
                // Retrieve the result of this request
                result = e.SocketError.ToString();
                Debug.WriteLine("Connection Attempt: " + result);
                

                // Signal that the request is complete, unblocking the UI thread
                _clientDone.Set();
            });

            // Sets the state of the event to nonsignaled, causing threads to block
            _clientDone.Reset();

            // Make an asynchronous Connect request over the socket
            _socket.ConnectAsync(socketEventArg);

            // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
            // If no response comes back within this time then proceed
            _clientDone.WaitOne(TIMEOUT_MILLISECONDS);

            return result;
        }

        /// <summary>
        /// Listens for a response from a socket
        /// </summary>
        /// <returns>The message from the socket that is currently connected to</returns>
        public string Receive()
        {
            string response = "Operation Timeout";

            // We are receiving over an established socket connection
            if (_socket != null)
            {
                // Create SocketAsyncEventArgs context object
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;

                // Setup the buffer to receive the data
                socketEventArg.SetBuffer(new Byte[MAX_BUFFER_SIZE], 0, MAX_BUFFER_SIZE);

                //Debug
                Debug.WriteLine("Connection Attempt...");

                // Inline event handler for the Completed event.
                // Note: This even handler was implemented inline in order to make this method self-contained.
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                {
                    if (e.SocketError == SocketError.Success)
                    {
                        // Retrieve the data from the buffer
                        Debug.WriteLine("Receive Attempt: Success");
                        response = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                        response = response.Trim('\0');
                        Debug.WriteLine("Length of Response from connection: " + response.Length);
                    }
                    else
                    {
                        Debug.WriteLine("Receive Attempt: Failure");
                        response = e.SocketError.ToString();
                    }

                    _clientDone.Set();
                });

                

                // Sets the state of the event to nonsignaled, causing threads to block
                _clientDone.Reset();

                // Make an asynchronous Receive request over the socket
                _socket.ReceiveAsync(socketEventArg);

                // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
                // If no response comes back within this time then proceed
                _clientDone.WaitOne(TIMEOUT_MILLISECONDS);
            }
            else
            {
                response = "Socket is not initialized";
            }

            return response;
        }

        /// <summary>
        /// Send the given data to the server using the established connection
        /// </summary>
        /// <param name="data">The data to send to the server</param>
        /// <returns>The result of the Send request</returns>
        public string Send(string data)
        {
            byte[] payload = Encoding.UTF8.GetBytes(data);
            return Send(payload);
        }

        /// <summary>
        /// Send the given data to the server using the established connection
        /// </summary>
        /// <param name="data">The data to send to the server</param>
        /// <returns>The result of the Send request</returns>
        public string Send(byte[] data)
        {
            string response = "Operation Timeout";

            // We are re-using the _socket object that was initialized in the Connect method
            if (_socket != null)
            {
                // Create SocketAsyncEventArgs context object
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();

                // Set properties on context object
                socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;
                socketEventArg.UserToken = null;

                //Debug
                Debug.WriteLine("Attempting to Send");

                // Inline event handler for the Completed event.
                // Note: This event handler was implemented inline in order to make this method self-contained.
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                {
                    response = e.SocketError.ToString();
                    Debug.WriteLine("Send Attempt: " + response);

                    // Unblock the UI thread
                    _clientDone.Set();
                });

                // Add the data to be sent into the buffer
                socketEventArg.SetBuffer(data, 0, data.Length);

                // Sets the state of the event to nonsignaled, causing threads to block
                _clientDone.Reset();

                // Make an asynchronous Send request over the socket
                _socket.SendAsync(socketEventArg);

                // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
                // If no response comes back within this time then proceed
                _clientDone.WaitOne(TIMEOUT_MILLISECONDS);
            }
            else
            {
                response = "Socket is not initialized";
                Debug.WriteLine("Send Error: " + response);
            }

            return response;
        }

        /// <summary>
        /// Closes the Socket connection and releases all associated resources
        /// </summary>
        public void Close()
        {
            if (_socket != null)
            {
                _socket.Close();
            }
        }

        /// <summary>
        /// Test whether you can receive a message back from a socket
        /// </summary>
        public void TestReceive()
        {
            this.Connect();
            this.Receive();
            this.Close();
        }

        /// <summary>
        /// Test whether you can write a message to a socket
        /// </summary>
        public void TestWrite()
        {
            this.Connect();
            this.Send("Tim blows big bubbles blatantly");
            this.Close();
        }

    }
}
