// EllipticalText.cs by Charles Petzold, June 2007
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Petzold.Text3D
{
    public class EllipticalText : GeometryTextBase
    {
        // EllipseWidth dependency property and property.
        public static readonly DependencyProperty EllipseWidthProperty =
            DependencyProperty.Register("EllipseWidth",
                typeof(double),
                typeof(EllipticalText),
                new PropertyMetadata(0.05, PropertyChanged));

        public double EllipseWidth
        {
            set { SetValue(EllipseWidthProperty, value); }
            get { return (double)GetValue(EllipseWidthProperty); }
        }

        // Slices dependency property and property.
        public static readonly DependencyProperty SlicesProperty =
            DependencyProperty.Register("Slices",
                typeof(int),
                typeof(EllipticalText),
                new PropertyMetadata(16, PropertyChanged));

        public int Slices
        {
            set { SetValue(SlicesProperty, value); }
            get { return (int)GetValue(SlicesProperty); }
        }

        // ProcessFigure override.
        protected override void ProcessFigure(CircularList<Point> list,
                                              Point3DCollection vertices,
                                              Vector3DCollection normals,
                                              Int32Collection indices,
                                              PointCollection textures)
        {
            int k = Slices * list.Count;
            int offset = vertices.Count;

            for (int i = 0; i < list.Count; i++)
            {
                Point ptBefore = list[i - 1];
                Point pt = list[i];
                Point ptAfter = list[i + 1];

                Vector v1 = pt - ptBefore;
                v1.Normalize();
                Vector v1Rotated = new Vector(-v1.Y, v1.X);

                Vector v2 = ptAfter - pt;
                v2.Normalize();
                Vector v2Rotated = new Vector(-v2.Y, v2.X);

                Line2D line1 = new Line2D(pt, ptBefore);
                Line2D line2 = new Line2D(pt, ptAfter);

                for (int slice = 0; slice < Slices; slice++)
                {
                    // Angle ranges from 0 to 360 degrees.
                    double angle = slice * 2 * Math.PI / Slices;
                    double scale = EllipseWidth / 2 * Math.Sin(angle);
                    double depth = -Depth / 2 * (1 - Math.Cos(angle));

                    Line2D line1Shifted = line1 + scale * v1Rotated;
                    Line2D line2Shifted = line2 + scale * v2Rotated;
                    Point ptIntersect = line1Shifted * line2Shifted;

                    // Set vertex.
                    vertices.Add(new Point3D(ptIntersect.X, ptIntersect.Y, depth));

                    // Set texture coordinate.
                    textures.Add(new Point((double)i / list.Count, 
                                           Math.Sin(angle / 2)));

                    // Set triangle indices.
                    indices.Add(offset + (Slices * i + slice + 0) % k);
                    indices.Add(offset + (Slices * i + slice + 1) % k);
                    indices.Add(offset + (Slices * i + slice + 0 + Slices) % k);

                    indices.Add(offset + (Slices * i + slice + 0 + Slices) % k);
                    indices.Add(offset + (Slices * i + slice + 1) % k);
                    indices.Add(offset + (Slices * i + slice + 1 + Slices) % k);
                }
            }
        }
    }
}
