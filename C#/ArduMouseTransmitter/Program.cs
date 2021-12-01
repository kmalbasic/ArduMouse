using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.InteropServices;
using System.Net.Sockets;

// todo: implement a simple communication class that is based on continuous data sending to the receiver -- DONE
namespace ArduMouseTransmitterCS
{
    class Program
    {

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Int32 ArrowKeys);

        static void Main(string[] args)
        {
            // Initialization stuff, declarations, etc.

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("ArduMouseTransmitter - C# version (dev) - github.com/kmalbasic \n\n");
            Console.Write("Input host IP: ");
            string ip = Console.ReadLine();
            Mouse mouse_obj = new Mouse();
            char sign_x; // for delta_x
            char sign_y; // for delta_y
            int recv;
            byte[] data = new byte[1024];
            IPEndPoint ip_endpoint = new IPEndPoint(IPAddress.Parse(ip), 7998);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);

            // binding ip&port and setting the limit to incoming connections

            socket.Bind(ip_endpoint);
            socket.Listen(5);
            Console.WriteLine("[ArduMouse] Waiting for a connection...");

            // accepting the connection and printing out fancy text

            Socket client = socket.Accept();
            IPEndPoint client_endpoint = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("[ArduMouse] Connection established.");

            // send the welcome message

            data = Encoding.UTF8.GetBytes("Connected to the host/transmitter.");
            client.Send(data, data.Length, SocketFlags.None);

            // setting prev_position to null -- (0,0)

            mouse_obj.prev_position.x = 0;
            mouse_obj.prev_position.y = 0;

            // main program loop

            while (true)
            {
                data = new byte[1024];
                // GetCursorPos basically (for C/C++ comrades)
                mouse_obj.curr_position = mouse_obj.get_cursor_coordinates();

                short status = GetAsyncKeyState(1);
                if ((status & 1) == 1)
                {
                    client.Send(Encoding.UTF8.GetBytes("C"));
                }
                // Calculating delta between current position and previous position
                Mouse.mouse_delta delta = mouse_obj.calculate_delta(mouse_obj.prev_position, mouse_obj.curr_position);
                if (delta.x > 0) sign_x = '+'; else sign_x = '-';
                if (delta.y > 0) sign_y = '+'; else sign_y = '-';
                // sanity check and performance improvement sort of
                if (delta.x != 0 || delta.y != 0)
                {
                    // send deltas to the receiver
                    client.Send(Encoding.UTF8.GetBytes(sign_x + delta.x.ToString() + " " + sign_y + delta.y.ToString()));
                }
            }

            // No need to keep everything running, close the connections and sockets
            client.Close();
            socket.Close();
            Console.WriteLine("[ArduMouse] Connection is closed. Restart the transmitter if you want to use ArduMouse again.");
            Console.ReadLine();

        }
    }
}
