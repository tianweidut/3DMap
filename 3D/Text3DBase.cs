// Texte3DBase.cs by Charles Petzold, June 2007
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Petzold.Text3D
{
    public abstract class Text3DBase : ModelVisualBase
    {
        // Text dependency property and property.
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text",
                typeof(string),
                typeof(Text3DBase),
                new UIPropertyMetadata("", TextPropertyChanged, null, true));

        public string Text
        {
            set { SetValue(TextProperty, value); }
            get { return (string)GetValue(TextProperty); }
        }

        // FontFamily dependency property and property.
        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(
                typeof(Text3DBase),
                new FrameworkPropertyMetadata(TextPropertyChanged));

        public FontFamily FontFamily
        {
            set { SetValue(FontFamilyProperty, value); }
            get { return (FontFamily)GetValue(FontFamilyProperty); }
        }

        // FontStyle dependency property and property.
        public static readonly DependencyProperty FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner(
                typeof(Text3DBase),
                new FrameworkPropertyMetadata(TextPropertyChanged));

        public FontStyle FontStyle
        {
            set { SetValue(FontStyleProperty, value); }
            get { return (FontStyle)GetValue(FontStyleProperty); }
        }

        // FontWeight dependency property and property.
        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(
                typeof(Text3DBase),
                new FrameworkPropertyMetadata(TextPropertyChanged));

        public FontWeight FontWeight
        {
            set { SetValue(FontWeightProperty, value); }
            get { return (FontWeight)GetValue(FontWeightProperty); }
        }

        // FontStretch dependency property and property.
        public static readonly DependencyProperty FontStretchProperty =
            TextElement.FontStretchProperty.AddOwner(
                typeof(Text3DBase),
                new FrameworkPropertyMetadata(TextPropertyChanged));

        public FontStretch FontStretch
        {
            set { SetValue(FontStretchProperty, value); }
            get { return (FontStretch)GetValue(FontStretchProperty); }
        }

        // FontSize dependency property and property.
        public static readonly DependencyProperty FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner(
                typeof(Text3DBase),
                new FrameworkPropertyMetadata(1.0, 
                        FrameworkPropertyMetadataOptions.None, 
                        TextPropertyChanged, null, true));

        public double FontSize
        {
            set { SetValue(FontSizeProperty, value); }
            get { return (double)GetValue(FontSizeProperty); }
        }

        // Origin dependency property and property.
        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register("Origin",
                typeof(Point),
                typeof(Text3DBase),
                new UIPropertyMetadata(new Point(), 
                            TextPropertyChanged, null, true));

        public Point Origin
        {
            set { SetValue(OriginProperty, value); }
            get { return (Point)GetValue(OriginProperty); }
        }

        // TextGeometry read-only dependency property and property
        static readonly DependencyPropertyKey TextGeometryKey =
            DependencyProperty.RegisterReadOnly("TextGeometry",
                typeof(Geometry),
                typeof(Text3DBase),
                new PropertyMetadata(null));

        public static readonly DependencyProperty TextGeometryProperty =
            TextGeometryKey.DependencyProperty;

        public Geometry TextGeometry
        {
            private set { SetValue(TextGeometryKey, value); }
            get { return (Geometry)GetValue(TextGeometryProperty); }
        }

        // TextPropertyChanged handlers.
        static void TextPropertyChanged(DependencyObject obj,
                                        DependencyPropertyChangedEventArgs args)
        {
            (obj as Text3DBase).TextPropertyChanged(args);
        }

        protected virtual void TextPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            FormattedText formtxt =
                    new FormattedText(Text, CultureInfo.CurrentCulture,
                                      FlowDirection.LeftToRight,
                        new Typeface(FontFamily, FontStyle,
                                     FontWeight, FontStretch),
                    FontSize, Brushes.Transparent);

            TextGeometry = formtxt.BuildGeometry(Origin);

            PropertyChanged(args);
        }
    }
}
