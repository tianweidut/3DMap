// PlanarText.cs by Charles Petzold, June 2007
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Petzold.Text3D
{
    public class PlanarText : Text3DBase
    {
        public static readonly DependencyProperty ZProperty =
            DependencyProperty.Register("Z",
                typeof(double),
                typeof(PlanarText),
                new PropertyMetadata(0.0, PropertyChanged));

        public double Z
        {
            set { SetValue(ZProperty, value); }
            get { return (double)GetValue(ZProperty); }
        }

        // PropertyChanged method needs to transfer materials from this object to the
        //  internal GeometryModel3D, in the process making them parts of DrawingBrush's
        //  based on GeometryDrawing's based on TextGeometry.
        protected override void MaterialPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            GeometryModel3D model = Content as GeometryModel3D;

            if (args.Property == MaterialProperty)
            {
                if (args.NewValue == null)
                    model.Material = null;

                else
                {
                    if (!AreMaterialsParallel(model.Material, args.NewValue as Material))
                        model.Material = (args.NewValue as Material).Clone();

                    TextifyParallelMaterials(model.Material, args.NewValue as Material);
                }
            }
            else if (args.Property == BackMaterialProperty)
            {
                if (args.NewValue == null)
                    model.BackMaterial = null;

                else
                {
                    if (!AreMaterialsParallel(model.BackMaterial, args.NewValue as Material))
                        model.BackMaterial = (args.NewValue as Material).Clone();

                    TextifyParallelMaterials(model.BackMaterial, args.NewValue as Material);
                }
            }
        }

        // Returns true if two materials have a parallel structure.
        bool AreMaterialsParallel(Material mat1, Material mat2)
        {
            if (mat1 is MaterialGroup && mat2 is MaterialGroup)
            {
                int num = (mat1 as MaterialGroup).Children.Count;

                if (num != (mat2 as MaterialGroup).Children.Count)
                    return false;

                for (int i = 0; i < num; i++)
                    if (!AreMaterialsParallel((mat1 as MaterialGroup).Children[i],
                                              (mat2 as MaterialGroup).Children[i]))
                        return false;

                return true;
            }
            return mat1 != null && mat2 != null && mat1.GetType() == mat2.GetType();
        }

        // Converts material brushes to DrawingBrush's based on text.
        void TextifyParallelMaterials(Material mat1, Material mat2)
        {
            if (mat1 is MaterialGroup)
            {
                int num = (mat1 as MaterialGroup).Children.Count;

                for (int i = 0; i < num; i++)
                {
                    TextifyParallelMaterials((mat1 as MaterialGroup).Children[i],
                                             (mat2 as MaterialGroup).Children[i]);
                }
            }
            else if (mat1 is DiffuseMaterial)
            {
                (mat1 as DiffuseMaterial).Brush =
                            TextifyBrush((mat2 as DiffuseMaterial).Brush);
            }
            else if (mat1 is SpecularMaterial)
            {
                (mat1 as SpecularMaterial).Brush =
                            TextifyBrush((mat2 as SpecularMaterial).Brush);
            }
            else if (mat1 is EmissiveMaterial)
            {
                (mat1 as EmissiveMaterial).Brush =
                            TextifyBrush((mat2 as EmissiveMaterial).Brush);
            }
        }

        // Convert brush to brush based on TextGeometry property.
        Brush TextifyBrush(Brush brushInp)
        {
            DrawingBrush brushOut = new DrawingBrush(
                            new GeometryDrawing(brushInp, null, TextGeometry));

            return brushOut;
        }

        protected override void PropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            // Simulate a MaterialPropertyChanged when most other properties change.
            if (args.Property != ZProperty)
            {
                MaterialPropertyChanged(
                        new DependencyPropertyChangedEventArgs(
                                MaterialProperty, null, Material));

                MaterialPropertyChanged(
                        new DependencyPropertyChangedEventArgs(
                                BackMaterialProperty, null, BackMaterial));
            }
            base.PropertyChanged(args);
        }

        // Define a plane consisting of two triangles.
        protected override void Triangulate(DependencyPropertyChangedEventArgs args, 
                                            Point3DCollection vertices, 
                                            Vector3DCollection normals, 
                                            Int32Collection indices, 
                                            PointCollection textures)
        {
            vertices.Clear();
            normals.Clear();
            indices.Clear();
            textures.Clear();

            if (TextGeometry == null)
                return;

            Rect rect = TextGeometry.Bounds;
            double top = 2 * Origin.Y - rect.Top;
            double bot = 2 * Origin.Y - rect.Bottom;

            // Define triangle vertices.
            vertices.Add(new Point3D(rect.Left, top, Z));
            vertices.Add(new Point3D(rect.Right, top, Z));
            vertices.Add(new Point3D(rect.Left, bot, Z));
            vertices.Add(new Point3D(rect.Right, bot, Z));

            // Define texture coordinates.
            textures.Add(new Point(0, 0));
            textures.Add(new Point(1, 0));
            textures.Add(new Point(0, 1));
            textures.Add(new Point(1, 1));

            // Define triangle indices.
            indices.Add(0);
            indices.Add(2);
            indices.Add(1);

            indices.Add(1);
            indices.Add(2);
            indices.Add(3);
        }
    }
}