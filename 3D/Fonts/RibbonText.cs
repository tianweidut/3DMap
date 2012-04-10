// RibbonText.cs by Charles Petzold, June 2007
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Petzold.Text3D
{
    public class RibbonText : GeometryTextBase
    {
        protected override void ProcessFigure(CircularList<Point> list,
                                              Point3DCollection vertices,
                                              Vector3DCollection normals,
                                              Int32Collection indices, 
                                              PointCollection textures)
        {
            int offset = vertices.Count;

            for (int i = 0; i <= list.Count; i++)
            {
                Point pt = list[i];

                // Set vertices.
                vertices.Add(new Point3D(pt.X, pt.Y, 0));
                vertices.Add(new Point3D(pt.X, pt.Y, -Depth));

                // Set texture coordinates.
                textures.Add(new Point((double)i / list.Count, 0));
                textures.Add(new Point((double)i / list.Count, 1));

                // Set triangle indices.
                if (i < list.Count)
                {
                    indices.Add(offset + i * 2 + 0);
                    indices.Add(offset + i * 2 + 2);
                    indices.Add(offset + i * 2 + 1);
                    
                    indices.Add(offset + i * 2 + 1);
                    indices.Add(offset + i * 2 + 2);
                    indices.Add(offset + i * 2 + 3);
                }
            }
        }
    }
}
