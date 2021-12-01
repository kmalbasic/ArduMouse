using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ArduMouseReceiverCS
{
    class Program
    {


        static void Main(string[] args)
        {
            // Initialization stuff, declarations, etc.

            Console.WriteLine("ArduMouseReceiver - C# version (dev) - github.com/kmalbasic \n\n");
            Console.Write("Input transmitter's IP: ");
            string ip = Console.ReadLine();
            Console.OutputEncoding = Encoding.UTF8;
            byte[] data = new byte[1024];
            string input, string_data;

            Mouse mouse = new Mouse(9600, "COM5");
            // Regular expression for filtering desired data out of received data
            string pattern = @"[-+][0-9]+";
            Regex re = new Regex(pattern);

            IPEndPoint ip_endpoint = new IPEndPoint(IPAddress.Parse(ip), 7998);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);

            try
            {
                // connect to server ip_ep
                server.Connect(ip_endpoint);
            }
            catch (SocketException e)
            {
                // catch exception
                Console.WriteLine("[ArduMouse] Unable to connect to server.");
                Console.WriteLine(e.ToString());
                return;
            }

            //we probably receive nothing, who cares though
            int recv = server.Receive(data);
            string_data = Encoding.UTF8.GetString(data, 0, recv);

            Console.WriteLine(string_data);

            while (true)
            {
                // receive data from host
                data = new byte[1024];
                recv = server.Receive(data);
                if(recv == 0)
                {
                    // received nothing, disconnect
                    break;
                }

                // decode the bytes received
                string_data = Encoding.UTF8.GetString(data, 0, recv);

                if (string_data == "C")
                {
                    mouse.click();
                }
                else
                {
                    // match our regular expression
                    MatchCollection deltas = re.Matches(string_data);  // access it like this -> matchedAuthors[count].Value

                    Mouse.mouse_pos pos = mouse.convert2mouse_pos(deltas[0].Value, deltas[1].Value);
                    mouse.move(pos);
                }
            }

            // closing connection, exiting the program
            Console.WriteLine("[ArduMouse] Disconnected from server.");
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            Console.ReadLine();
        }
    }
}
