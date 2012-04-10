// GeometryTextBase.cs by Charles Petzold, June 2007
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Petzold.Text3D
{
    public abstract class GeometryTextBase : DeepTextBase
    {
        // Field prevent re-allocations during mesh generation.
        CircularList<Point> list = new CircularList<Point>();

        protected override void Triangulate(
                                    DependencyPropertyChangedEventArgs args, 
                                    Point3DCollection vertices, 
                                    Vector3DCollection normals, 
                                    Int32Collection indices, 
                                    PointCollection textures)
        {
            // Clear all four collections.
            vertices.Clear();
            normals.Clear();
            indices.Clear();
            textures.Clear();

            // Convert TextGeometry to series of closed polylines.
            PathGeometry path = 
                TextGeometry.GetFlattenedPathGeometry(0.001, 
                                                ToleranceType.Relative);

            foreach (PathFigure fig in path.Figures)
            {
                list.Clear();
                list.Add(fig.StartPoint);

                foreach (PathSegment seg in fig.Segments)
                {
                    if (seg is LineSegment)
                    {
                        LineSegment lineseg = seg as LineSegment;
                        list.Add(lineseg.Point);
                    }

                    else if (seg is PolyLineSegment)
                    {
                        PolyLineSegment polyline = seg as PolyLineSegment;

                        for (int i = 0; i < polyline.Points.Count; i++)
                            list.Add(polyline.Points[i]);
                    }
                }

                // Figure is complete. Post-processing follows.
                if (list.Count > 0)
                {
                    // Remove last point if it's the same as the first.
                    if (list[0] == list[list.Count - 1])
                        list.RemoveAt(list.Count - 1);

                    // Convert points to Y increasing up.
                    for (int i = 0; i < list.Count; i++)
                    {
                        Point pt = list[i];
                        pt.Y = 2 * Origin.Y - pt.Y;
                        list[i] = pt;
                    }

                    // For each figure, process the points.
                    ProcessFigure(list, vertices, normals, indices, textures);
                }
            }
        }

        // Abstract method to convert figure to mesh geometry.
        protected abstract void ProcessFigure(CircularList<Point> list,
                                              Point3DCollection vertices,
                                              Vector3DCollection normals,
                                              Int32Collection indices,
                                              PointCollection textures);
    }
}



