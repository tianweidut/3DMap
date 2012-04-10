// Line2D.cs by Charles Petzold, June 2007
using System;
using System.Windows;
using System.Windows.Media;

namespace Petzold.Text3D
{
    // Represents Line as Ax + By + C = 0.
    public struct Line2D
    {
        double a, b, c;

        public double A
        {
            get { return a; }
            set { a = value; }
        }

        public double B
        {
            get { return b; }
            set { b = value; }
        }

        public double C
        {
            get { return c; }
            set { c = value; }
        }

        public Line2D(double a, double b, double c)
        {
            this.a = this.b = this.c = 0;

            A = a;
            B = b;
            C = c;
        }

        public Line2D(Point pt1, Point pt2)
        {
            this.a = this.b = this.c = 0;

            A = pt1.Y - pt2.Y;
            B = pt2.X - pt1.X;
            C = pt1.X * pt2.Y - pt2.X * pt1.Y;
        }

        // Shifts line in direction of vector
        public static Line2D operator +(Line2D line, Vector vect)
        {
            return new Line2D(line.A, line.B,
                        line.C - line.A * vect.X - line.B * vect.Y);
        }

        // Finds intersection of two lines
        public static Point operator *(Line2D line1, Line2D line2)
        {
            double den = line1.A * line2.B - line2.A * line1.B;

            return new Point((line1.B * line2.C - line2.B * line1.C) / den,
                             (line2.A * line1.C - line1.A * line2.C) / den);
        }

        public override string ToString()
        {
            return String.Format("{0}x + {1}y + {2}", A, B, C);
        }
    }
}
