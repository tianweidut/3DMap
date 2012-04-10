// SolidText.cs by Charles Petzold, June 2007
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Petzold.Text3D
{
    public class SolidText : DeepTextBase
    {
        public SolidText()
        {
            // Create RibbonText and two PlanarText children.
            RibbonText ribbon = new RibbonText();
            ribbon.Depth = Depth;
            Children.Add(ribbon);

            PlanarText planar = new PlanarText();
            Children.Add(planar);

            planar = new PlanarText();
            planar.Z = -Depth;
            Children.Add(planar);
        }

        // SideMaterial dependency property and property.
        public static readonly DependencyProperty SideMaterialProperty =
            DependencyProperty.Register("SideMaterial",
                typeof(Material),
                typeof(SolidText),
                new PropertyMetadata(SideMaterialChanged));

        public Material SideMaterial
        {
            set { SetValue(SideMaterialProperty, value); }
            get { return (Material)GetValue(SideMaterialProperty); }
        }

        // SideMaterialChanged handlers.
        static void SideMaterialChanged(DependencyObject obj,
                                    DependencyPropertyChangedEventArgs args)
        {
            (obj as SolidText).SideMaterialChanged(args);
        }

        void SideMaterialChanged(DependencyPropertyChangedEventArgs args)
        {
            // Transfer SideMaterial to RibbonText.
            Text3DBase txtbase = Children[0] as Text3DBase;
            txtbase.Material = args.NewValue as Material;
            txtbase.BackMaterial = args.NewValue as Material;
        }

        // MaterialChanged override.
        protected override void MaterialPropertyChanged(
                                    DependencyPropertyChangedEventArgs args)
        {
            // Transfer Material and BackMaterial properties to PlanarText.
            if (args.Property == MaterialProperty)
                (Children[1] as PlanarText).Material = 
                                        args.NewValue as Material;

            else if (args.Property == BackMaterialProperty)
                (Children[2] as PlanarText).BackMaterial = 
                                        args.NewValue as Material;
        }

        // TextPropertyChanged override.
        protected override void TextPropertyChanged(
                                    DependencyPropertyChangedEventArgs args)
        {
            base.TextPropertyChanged(args);

            // Transfer text-related property to all three children.
            for (int i = 0; i < 3; i++)
            {
                Text3DBase txtbase = Children[i] as Text3DBase;
                txtbase.SetValue(args.Property, args.NewValue);
            }
        }

        // PropertyChanged override.
        protected override void PropertyChanged(
                                    DependencyPropertyChangedEventArgs args)
        {
            if (args.Property == DepthProperty)
            {
                // Set Depth property to RibbonText and PlanarText children.
                double depth = (double)args.NewValue;
                (Children[0] as DeepTextBase).Depth = depth;
                (Children[2] as PlanarText).Z = -depth;
            }

            base.PropertyChanged(args);
        }

        // Move on, move on. Nothing to see here. 
        protected override void Triangulate(
                                    DependencyPropertyChangedEventArgs args, 
                                    Point3DCollection vertices, 
                                    Vector3DCollection normals, 
                                    Int32Collection indices, 
                                    PointCollection textures)
        {
            return;
        }
    }
}