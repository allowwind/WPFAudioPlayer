using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WPFAudioPlayer
{
    public class MarkTicker : FrameworkElement
    {
        public long Tick { get; private set; }      



        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("ClickEvent", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventArgs<Object>), typeof(MarkTicker));
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        void FireClick()
        {
            RoutedEventArgs eventArgs = new RoutedEventArgs(MarkTicker.ClickEvent, this);
            this.RaiseEvent(eventArgs);
        }

        public MarkTicker(long tick):this()
        {
            Tick = tick;
             
        }
        public MarkTicker()
        {
            this.MouseLeftButtonUp += MarkTicker_MouseLeftButtonUp;
            this.SizeChanged += MarkTicker_SizeChanged;
        }

        private void MarkTicker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void MarkTicker_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FireClick();
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(MarkTicker), new PropertyMetadata(Brushes.Red, OnDataChanged));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MarkTicker).InvalidateVisual();
        }

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(MarkTicker), new PropertyMetadata(Brushes.Transparent, OnDataChanged));


        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(MarkTicker), new PropertyMetadata(Brushes.Transparent, OnDataChanged));


        //DrawingVisual dv_background = new DrawingVisual();
        //DrawingVisual dv_chart = new DrawingVisual();

        //protected override int VisualChildrenCount => _children.Count;

        //protected override Visual GetVisualChild(int index)
        //{
        //    if (index < 0 || index >= _children.Count)
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }

        //    return _children[index];
        //}
        //private readonly VisualCollection _children;
        //public MarkTicker()
        //{
        //    _children = new VisualCollection(this);
        //    _children.Add(dv_background);
        //    _children.Add(dv_chart);

        //}


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(Background, new Pen(BorderBrush, 0), new Rect(0, 0,   RenderSize.Width,  RenderSize.Height));
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure() { IsClosed = true, StartPoint = new Point( RenderSize.Width/2, 0) };
            pathFigure.Segments.Add(new LineSegment(new Point( RenderSize.Width,  RenderSize.Height), false));
            pathFigure.Segments.Add(new LineSegment(new Point(0,  RenderSize.Height), false));
            pathGeometry.Figures.Add(pathFigure);
            drawingContext.DrawGeometry(Fill, new Pen(BorderBrush, 0), pathGeometry);
        }
    }
}
