using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace ArduMouseReceiverCS
{
    class Mouse
    {
        public struct mouse_pos {
            public int x;
            public int y;
        }

        public SerialPort arduino_port;
        public Mouse(int port_no, string com)
        {
            arduino_port = new SerialPort(com, port_no);
            arduino_port.Open();
        }

        public mouse_pos convert2mouse_pos(string x, string y)
        {
            mouse_pos return_val;
            int x_pos = Int32.Parse(x);
            int y_pos = Int32.Parse(y);

            return_val.x = x_pos;
            return_val.y = y_pos;

            return return_val;
        }

        public void move(mouse_pos coordinates)
        {
            arduino_port.Write("M");
            arduino_port.Write(coordinates.x.ToString());
            arduino_port.Write(coordinates.y.ToString());
        }

        public void click()
        {
            arduino_port.Write("C");
        }


    }
}
