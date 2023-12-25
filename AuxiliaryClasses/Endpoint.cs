using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace SimpleCircuitDesigner
{
    internal class Endpoint
    {
        private EllipseGeometry pointShape;
        private SolidColorBrush accentColor;
        private Point startPoint;
        private Point endPoint;
        public Endpoint ConnectedWith { get; private set; }
        public Path connectionPath { get; private set; }
        public Path EndpointObject { get; private set; }
        public Point Offset { get; private set; }
        public bool IsConnected { get; private set; }
        public ItemBaseModel BaseItem { get; private set; }


        //          Constructor           //

        public Endpoint(ItemBaseModel model, Point Offset)
        {
            this.Offset = Offset;
            BaseItem = model;
            IsConnected = false;
            ConnectedWith = null;
            accentColor = Brushes.DimGray;

            pointShape = new EllipseGeometry(new Point(0, 0), 3, 3);
            DrawEndpoint();
        }
        public void ConnectCall(Endpoint connectTo)
        {
            if (!connectTo.IsConnected && !this.IsConnected)
            {
                IsConnected = true;
                ConnectedWith = connectTo;
                connectTo.ConnectResponse(this);

                InitializeConnectionWire();
                connectTo.InitializeConnectionWire();

                UpdateConnectionWire();
            }
        }
        private void ConnectResponse(Endpoint connectTo)
        {
            IsConnected = true;
            ConnectedWith = connectTo;
        }
        public void Disconnect()
        {
            if (ConnectedWith != null)
            {
                ConnectedWith.DisconnectResponse();
                IsConnected = false;

                if (connectionPath != null && MainWindow.MainItemCanvas.Children.Contains(connectionPath))
                {
                    MainWindow.MainItemCanvas.Children.Remove(connectionPath);
                    connectionPath.Data = null;
                    connectionPath = null;
                }

                ConnectedWith = null;
            }
        }
        private void DisconnectResponse()
        {
            IsConnected = false;

            if (connectionPath != null && MainWindow.MainItemCanvas.Children.Contains(connectionPath))
            {
                MainWindow.MainItemCanvas.Children.Remove(connectionPath);
                connectionPath.Data = null;
                connectionPath = null;
            }

            ConnectedWith = null;
        }
        public void ReplaceEndpoint(Point newOffset)
        {
            Offset = newOffset;
            EndpointObject.RenderTransform = new TranslateTransform(BaseItem.location.X + Offset.X,
                                                                    BaseItem.location.Y + Offset.Y);
            UpdateConnectionWire();
        }
        public void UpdateConnectionWire()
        {
            if (IsConnected)
            {
                startPoint = new Point(this.BaseItem.location.X + this.Offset.X,
                                       this.BaseItem.location.Y + this.Offset.Y);
                endPoint = new Point(ConnectedWith.BaseItem.location.X + ConnectedWith.Offset.X,
                                     ConnectedWith.BaseItem.location.Y + ConnectedWith.Offset.Y);

                Point controlPoint1 = new Point((startPoint.X + endPoint.X) / 2, startPoint.Y);
                Point controlPoint2 = new Point((startPoint.X + endPoint.X) / 2, endPoint.Y);

                PathFigure pathFigure = new PathFigure();
                pathFigure.StartPoint = startPoint;
                BezierSegment bezierSegment = new BezierSegment
                {
                    Point1 = controlPoint1,
                    Point2 = controlPoint2,
                    Point3 = endPoint
                };

                pathFigure.Segments = new PathSegmentCollection { bezierSegment };

                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Figures = new PathFigureCollection { pathFigure };

                connectionPath.Data = pathGeometry;

                if (!MainWindow.MainItemCanvas.Children.Contains(connectionPath))
                    MainWindow.MainItemCanvas.Children.Add(connectionPath);
            }
        }
        private void InitializeConnectionWire()
        {
            connectionPath = new Path();
            connectionPath.Stroke = new SolidColorBrush(Colors.SlateGray);
            connectionPath.StrokeThickness = 5;

            connectionPath.StrokeLineJoin = PenLineJoin.Round;
            connectionPath.StrokeStartLineCap = PenLineCap.Round;
            connectionPath.StrokeEndLineCap = PenLineCap.Round;

            Canvas.SetZIndex(connectionPath, 1);
        }
        public void ChangeAccentColor(SolidColorBrush color)
        {
            EndpointObject.Stroke = color;
        }
        private void DrawEndpoint()
        {
            EndpointObject = new Path();
            EndpointObject.Data = pointShape;
            EndpointObject.StrokeThickness = 3;
            EndpointObject.Stroke = accentColor;
            Canvas.SetZIndex(EndpointObject, 2);
            EndpointObject.RenderTransform = new TranslateTransform(BaseItem.location.X + Offset.X,
                                                                    BaseItem.location.Y + Offset.Y);
            MainWindow.MainItemCanvas.Children.Add(EndpointObject);
        }
    }
}
