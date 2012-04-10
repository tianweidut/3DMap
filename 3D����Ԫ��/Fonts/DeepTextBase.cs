// DeepTextBase.cs by Charles Petzold, June 2007
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Petzold.Text3D
{
    public abstract class DeepTextBase : Text3DBase
    {
        // Depth dependency property and property.
        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.Register("Depth",
                typeof(double),
                typeof(DeepTextBase),
                new PropertyMetadata(1.0, PropertyChanged));

        public double Depth
        {
            set { SetValue(DepthProperty, value); }
            get { return (double)GetValue(DepthProperty); }
        }
    }
}



