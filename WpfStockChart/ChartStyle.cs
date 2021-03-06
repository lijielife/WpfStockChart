﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfStockChart
{
    internal class ChartStyle
    {
        private string title = "Title";
        private string xLabel = "X Axis";
        private string yLabel = "Y Axis";
        private Canvas textCanvas;
        private bool isXGrid = true;
        private bool isYGrid = true;
        private Brush gridLineColor = Brushes.LightGray;
        private double xTick = 0.2;
        private double yTick = 0.2;
        private GridLinePatternEnum gridLinePattern;
        private double leftOffset = 20;
        private double bottomOffset = 15;
        private double rightOffset = 10;
        private Line gridline = new Line();

        private double xmin = 0;
        private double xmax = 1;
        private double ymin = 0;
        private double ymax = 1;

        private Canvas chartCanvas;

        public Canvas ChartCanvas
        {
            get { return chartCanvas; }
            set { chartCanvas = value; }
        }
        public double Xmin
        {
            get { return xmin; }
            set { xmin = value; }
        }
        public double Xmax
        {
            get { return xmax; }
            set { xmax = value; }
        }
        public double Ymin
        {
            get { return ymin; }
            set { ymin = value; }
        }
        public double Ymax
        {
            get { return ymax; }
            set { ymax = value; }
        }

        public Point NormalizePoint(Point pt)
        {
            if (ChartCanvas.Width.ToString() == "NaN")
                ChartCanvas.Width = 270;
            if (ChartCanvas.Height.ToString() == "NaN")
                ChartCanvas.Height = 250;
            Point result = new Point();
            result.X = (pt.X - Xmin) * ChartCanvas.Width / (Xmax - Xmin);
            result.Y = ChartCanvas.Height - (pt.Y - Ymin) * ChartCanvas.Height / (Ymax - Ymin);
            return result;
        }

        public double TimeSpanToDouble(TimeSpan ts)
        {
            DateTime dt = DateTime.Parse("1 Jan");
            double d1 = BitConverter.ToDouble(BitConverter.GetBytes(dt.Ticks), 0);
            dt += ts;
            double d2 = BitConverter.ToDouble(BitConverter.GetBytes(dt.Ticks), 0);
            return d2 - d1;
        }

        public double DateToDouble(string date)
        {
            DateTime dt = DateTime.Parse(date);
            return BitConverter.ToDouble(BitConverter.GetBytes(dt.Ticks), 0);
        }

        public DateTime DoubleToDate(double d)
        {
            return new DateTime(BitConverter.ToInt64(BitConverter.GetBytes(d), 0));
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string XLabel
        {
            get { return xLabel; }
            set { xLabel = value; }
        }

        public string YLabel
        {
            get { return yLabel; }
            set { yLabel = value; }
        }

        public GridLinePatternEnum GridLinePattern
        {
            get { return gridLinePattern; }
            set { gridLinePattern = value; }
        }

        public double XTick
        {
            get { return xTick; }
            set { xTick = value; }
        }

        public double YTick
        {
            get { return yTick; }
            set { yTick = value; }
        }

        public Brush GridLineColor
        {
            get { return gridLineColor; }
            set { gridLineColor = value; }
        }
        
        public Canvas TextCanvas
        {
            get { return textCanvas; }
            set { textCanvas = value; }
        }

        public bool IsXGrid
        {
            get { return isXGrid; }
            set { isXGrid = value; }
        }

        public bool IsYGrid
        {
            get { return isYGrid; }
            set { isYGrid = value; }
        }

        public void AddChartStyle(TextBlock tbTitle, TextBlock tbXLabel, TextBlock tbYLabel, DataSeries ds)
        {
            Point pt = new Point();
            Line tick = new Line();
            double offset = 0;
            double dx, dy;
            TextBlock tb = new TextBlock();

            // determin right offset:
            tb.Text = Ymax.ToString();
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Size size = tb.DesiredSize;
            rightOffset = size.Width / 2 + 2;

            // determine left offset:
            for (dy = Ymin; dy <= Ymax; dy += YTick)
            {
                pt = NormalizePoint(new Point(Xmin, dy));
                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.TextAlignment = TextAlignment.Right;
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                if (offset < size.Width)
                    offset = size.Width;
            }
            leftOffset = offset + 5;

            Canvas.SetLeft(ChartCanvas, leftOffset);
            Canvas.SetBottom(ChartCanvas, bottomOffset);
            ChartCanvas.Width = TextCanvas.Width - leftOffset - rightOffset;
            ChartCanvas.Height = TextCanvas.Height - bottomOffset - size.Height / 1.5;
            Rectangle chartRect = new Rectangle();
            chartRect.Stroke = Brushes.Black;
            chartRect.Width = ChartCanvas.Width;
            chartRect.Height = ChartCanvas.Height;
            ChartCanvas.Children.Add(chartRect);

            // Create vertical gridlines:
            if(IsYGrid == true)
            {
                for (dx = Xmin + XTick; dx < Xmax; dx += XTick)
                {
                    gridline = new Line();
                    AddLinePattern();
                    gridline.X1 = NormalizePoint(new Point(dx, Ymin)).X;
                    gridline.Y1 = NormalizePoint(new Point(dx, Ymin)).Y;
                    gridline.X2 = NormalizePoint(new Point(dx, Ymax)).X;
                    gridline.Y2 = NormalizePoint(new Point(dx, Ymax)).Y;
                    ChartCanvas.Children.Add(gridline);
                }
            }

            // Create horizontal gridlines:
            if (IsXGrid == true)
            {
                for (dy = Ymin + YTick; dy < Ymax; dy += YTick)
                {
                    gridline = new Line();
                    AddLinePattern();
                    gridline.X1 = NormalizePoint(new Point(Xmin, dy)).X;
                    gridline.Y1 = NormalizePoint(new Point(Xmin, dy)).Y;
                    gridline.X2 = NormalizePoint(new Point(Xmax, dy)).X;
                    gridline.Y2 = NormalizePoint(new Point(Xmax, dy)).Y;
                    ChartCanvas.Children.Add(gridline);
                }
            }

            // Create x-axis tick marks:
            for (dx = Xmin; dx < Xmax; dx += xTick)
            {
                pt = NormalizePoint(new Point(dx, Ymin));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X;
                tick.Y2 = pt.Y - 5;
                ChartCanvas.Children.Add(tick);

                if (dx >= 0 && dx < ds.DataString.GetLength(1))
                {
                    tb = new TextBlock();
                    double d0 = DateToDouble(ds.DataString[0, 0]);
                    double d1 = DateToDouble(ds.DataString[0, 1]);
                    double d = DateToDouble(ds.DataString[0, (int)dx]);
                    if (d0 > d1)
                        d = DateToDouble(ds.DataString[0, ds.DataString.GetLength(1) - 1 - (int)dx]);
                    tb.Text = DoubleToDate(d).ToString("m");
                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;
                    TextCanvas.Children.Add(tb);
                    Canvas.SetLeft(tb, leftOffset + pt.X - size.Width / 2);
                    Canvas.SetTop(tb, pt.Y + 2 + size.Height / 2);
                }
            }

            // Create y-axis tick marks:
            for (dy = Ymin; dy <= Ymax; dy += YTick)
            {
                pt = NormalizePoint(new Point(Xmin, dy));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X + 5;
                tick.Y2 = pt.Y;
                ChartCanvas.Children.Add(tick);

                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                TextCanvas.Children.Add(tb);
                Canvas.SetRight(tb, ChartCanvas.Width + 10);
                Canvas.SetTop(tb, pt.Y);
            }

            // Add title and labels:
            tbTitle.Text = Title;
            tbXLabel.Text = XLabel;
            tbYLabel.Text = YLabel;
            tbXLabel.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
            tbTitle.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
        }

        public void AddLinePattern()
        {
            gridline.Stroke = GridLineColor;
            gridline.StrokeThickness = 1;

            switch(GridLinePattern)
            {
                case GridLinePatternEnum.Dash:
                    gridline.StrokeDashArray = new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case GridLinePatternEnum.Dot:
                    gridline.StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case GridLinePatternEnum.DashDot:
                    gridline.StrokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
            }
        }

        public enum GridLinePatternEnum
        {
            Solid = 1,
            Dash = 2,
            Dot = 3,
            DashDot = 4
        }
    }
}