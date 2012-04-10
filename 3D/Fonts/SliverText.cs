// SliverText.cs (c) by Charles Petzold, June 2007
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Petzold.Text3D
{
    public class SliverText : GeometryTextBase
    {
        // SliverWidth dependency property and property.
        public static readonly DependencyProperty SliverWidthProperty =
            DependencyProperty.Register("SliverWidth",
                typeof(double),
                typeof(SliverText),
                new PropertyMetadata(0.05, PropertyChanged));

        public double SliverWidth
        {
            set { SetValue(SliverWidthProperty, value); }
            get { return (double)GetValue(SliverWidthProperty); }
        }

        // ProcessFigure overrride.
        protected override void ProcessFigure(CircularList<Point> list,
                                              Point3DCollection vertices,
                                              Vector3DCollection normals,
                                              Int32Collection indices, 
                                              PointCollection textures)
        {
            int offset = vertices.Count;

            for (int i = 0; i <= list.Count; i++)
            {
                Point ptBefore = list[i - 1];
                Point pt = list[i];
                Point ptAfter = list[i + 1];

                Vector v1 = pt - ptBefore;
                v1.Normalize();
                // Rotate by 90 degrees.
                Vector v1Rotated = new Vector(-v1.Y, v1.X);

                Vector v2 = ptAfter - pt;
                v2.Normalize();
                Vector v2Rotated = new Vector(-v2.Y, v2.X);

                Line2D line1 = new Line2D(pt, ptBefore);
                Line2D line2 = new Line2D(pt, ptAfter);

                double scale = SliverWidth / 2;
                Line2D line1Shifted = line1 + scale * v1Rotated;
                Line2D line2Shifted = line2 + scale * v2Rotated;
                Point ptIntersect = line1Shifted * line2Shifted;

                Point ptOuter = ptIntersect;
                Point ptInner = pt + -(ptIntersect - pt);

                // Set triangles vertices.
                vertices.Add(new Point3D(ptOuter.X, ptOuter.Y, -Depth));
                vertices.Add(new Point3D(ptInner.X, ptInner.Y, -Depth));

                vertices.Add(new Point3D(ptOuter.X, ptOuter.Y, 0));
                vertices.Add(new Point3D(ptInner.X, ptInner.Y, 0));

                // Set texture coordinates.
                textures.Add(new Point(ptOuter.X, ptOuter.Y));
                textures.Add(new Point(ptInner.X, ptInner.Y));
                textures.Add(new Point(ptOuter.X, ptOuter.Y));
                textures.Add(new Point(ptInner.X, ptInner.Y));

                // Set triangle indices.
                if (i < list.Count)
                {
                    // Top
                    indices.Add(offset + 4 * i + 0);
                    indices.Add(offset + 4 * i + 1);
                    indices.Add(offset + 4 * i + 4);

                    indices.Add(offset + 4 * i + 1);
                    indices.Add(offset + 4 * i + 5);
                    indices.Add(offset + 4 * i + 4);

                    // Bottom
                    indices.Add(offset + 4 * i + 2);
                    indices.Add(offset + 4 * i + 6);
                    indices.Add(offset + 4 * i + 3);

                    indices.Add(offset + 4 * i + 3);
                    indices.Add(offset + 4 * i + 6);
                    indices.Add(offset + 4 * i + 7);

                    // Outer Side
                    indices.Add(offset + 4 * i + 0);
                    indices.Add(offset + 4 * i + 2);
                    indices.Add(offset + 4 * i + 4);

                    indices.Add(offset + 4 * i + 2);
                    indices.Add(offset + 4 * i + 6);
                    indices.Add(offset + 4 * i + 4);

                    // Inner Side
                    indices.Add(offset + 4 * i + 1);
                    indices.Add(offset + 4 * i + 5);
                    indices.Add(offset + 4 * i + 3);

                    indices.Add(offset + 4 * i + 3);
                    indices.Add(offset + 4 * i + 5);
                    indices.Add(offset + 4 * i + 7);
                }
            }
        }
    }
}
