// ModelVisualBase.cs by Charles Petzold, June 2007
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Petzold.Text3D
{
    public abstract class ModelVisualBase : ModelVisual3D
    {
        // Public parameterless constructor.
        public ModelVisualBase()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            GeometryModel3D model = new GeometryModel3D(mesh, null);
            Content = model;
        }

        // Material dependency property and property.
        public static readonly DependencyProperty MaterialProperty =
            GeometryModel3D.MaterialProperty.AddOwner(
                typeof(ModelVisualBase),
                new PropertyMetadata(null, MaterialPropertyChanged));

        public Material Material
        {
            get { return (Material)GetValue(MaterialProperty); }
            set { SetValue(MaterialProperty, value); }
        }

        // BackMaterial dependency property and property.
        public static readonly DependencyProperty BackMaterialProperty =
            GeometryModel3D.BackMaterialProperty.AddOwner(
                typeof(ModelVisualBase),
                new PropertyMetadata(null, MaterialPropertyChanged));

        public Material BackMaterial
        {
            get { return (Material)GetValue(BackMaterialProperty); }
            set { SetValue(BackMaterialProperty, value); }
        }

        // MaterialChanged handlers.
        static void MaterialPropertyChanged(DependencyObject obj,
                                DependencyPropertyChangedEventArgs args)
        {
            (obj as ModelVisualBase).MaterialPropertyChanged(args);
        }

        protected virtual void MaterialPropertyChanged(
                                DependencyPropertyChangedEventArgs args)
        {
            GeometryModel3D model = Content as GeometryModel3D;

            if (args.Property == MaterialProperty)
                model.Material = (Material)args.NewValue;

            else if (args.Property == BackMaterialProperty)
                model.BackMaterial = (Material)args.NewValue;
        }

        // Non-Material property changed handlers.
        protected static void PropertyChanged(DependencyObject obj,
                                      DependencyPropertyChangedEventArgs args)
        {
            (obj as Text3DBase).PropertyChanged(args);
        }

        protected virtual void PropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            // Obtain the MeshGeometry3D.
            MeshGeometry3D mesh = 
                (Content as GeometryModel3D).Geometry as MeshGeometry3D;

            // Get all four collections.
            Point3DCollection vertices = mesh.Positions;
            Vector3DCollection normals = mesh.Normals;
            Int32Collection indices = mesh.TriangleIndices;
            PointCollection textures = mesh.TextureCoordinates;

            // Set the MeshGeometry3D collections to null while updating.
            mesh.Positions = null;
            mesh.Normals = null;
            mesh.TriangleIndices = null;
            mesh.TextureCoordinates = null;

            // Generate the vertices, etc.
            Triangulate(args, vertices, normals, indices, textures);

            // Set the updated collections to the MeshGeometry3D.
            mesh.TextureCoordinates = textures;
            mesh.TriangleIndices = indices;
            mesh.Normals = normals;
            mesh.Positions = vertices;
        }

        protected abstract void Triangulate(DependencyPropertyChangedEventArgs args,
                                            Point3DCollection vertices,
                                            Vector3DCollection normals,
                                            Int32Collection indices,
                                            PointCollection textures);
    }
}
