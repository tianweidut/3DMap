// RoundedText.cs by Charles Petzold, June 2007
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Petzold.Text3D
{
    public class RoundedText : SolidText
    {
        public RoundedText()
        {
            // Replace first child set by SolidText constructor.
            Children[0] = new EllipticalText();
        }

        // EllipseWidth dependency property and property.
        public static readonly DependencyProperty EllipseWidthProperty =
            EllipticalText.EllipseWidthProperty.AddOwner(
                typeof(RoundedText));

        public double EllipseWidth
        {
            set { SetValue(EllipseWidthProperty, value); }
            get { return (double)GetValue(EllipseWidthProperty); }
        }

        // Slices dependency property and property.
        public static readonly DependencyProperty SlicesProperty =
            EllipticalText.SlicesProperty.AddOwner(
                typeof(RoundedText));

        public int Slices
        {
            set { SetValue(SlicesProperty, value); }
            get { return (int)GetValue(SlicesProperty); }
        }

        protected override void PropertyChanged(
                            DependencyPropertyChangedEventArgs args)
        {
            if (args.Property == EllipseWidthProperty)
                (Children[0] as EllipticalText).EllipseWidth = 
                                                    (double)args.NewValue;

            else if (args.Property == SlicesProperty)
                (Children[0] as EllipticalText).Slices = (int)args.NewValue;

            base.PropertyChanged(args);
        }
    }
}