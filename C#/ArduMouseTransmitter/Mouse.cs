using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ArduMouseTransmitterCS
{
    class Mouse
    {
        // storing mouse position, separated structures for convenience
        public struct mouse_pos
        {
            public int x;
            public int y;
        }

        // storing deltas
        public struct mouse_delta
        {
            public int x;
            public int y;
        }

        // POINT struct replica (from C++)
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        // importing GetCursorPos from user32.dll
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        // storing coords
        public mouse_pos prev_position;
        public mouse_pos curr_position;

        // basically rewritten GetCursorPos so it works with mouse_pos struct
        public mouse_pos get_cursor_coordinates()
        {
            POINT coordinates;
            GetCursorPos(out coordinates);
            mouse_pos point2mouse_pos;
            point2mouse_pos.x = coordinates.X;
            point2mouse_pos.y = coordinates.Y;

            return point2mouse_pos;
        }

        // calculating the delta, selfexplanatory 
        public mouse_delta calculate_delta(mouse_pos prev_pos, mouse_pos curr_pos)
        {
            mouse_delta delta;

            if (prev_pos.x - curr_pos.x > 0 || prev_pos.x - curr_pos.x < 0)
                delta.x = (prev_pos.x - curr_pos.x) * (-1);
            else
                delta.x = 0;

            if (prev_pos.y - curr_pos.y > 0 || prev_pos.y - curr_pos.y < 0)
                delta.y = (prev_pos.y - curr_pos.y) * (-1);
            else
                delta.y = 0;

            this.prev_position = curr_pos;

            return delta;
        }
    }
}
