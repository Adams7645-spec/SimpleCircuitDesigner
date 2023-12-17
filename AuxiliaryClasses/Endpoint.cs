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
        private Endpoint ConnectedWith;
        private SolidColorBrush accentColor;
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

            pointShape = new EllipseGeometry(new Point(0, 0), 5, 5);
            DrawEndpoint();
        }
        public void ConnectCall(Endpoint connectTo)
        {
            if (!connectTo.IsConnected && !this.IsConnected)
            {
                IsConnected = true;
                ConnectedWith = connectTo;
                connectTo.ConnectResponse(this);

                DrawConnectionWire();
            }
        }
        private void ConnectResponse(Endpoint connectTo)
        {
            IsConnected = true;
            ConnectedWith = connectTo;
        }
        public void Disconnect()
        {
            IsConnected = false;
        }
        private void DrawConnectionWire()
        {
            Line connectionLine = new Line();

            connectionLine.Stroke = new SolidColorBrush(Colors.DimGray);
            connectionLine.StrokeThickness = 2; // толщина линии

            connectionLine.X1 = this.BaseItem.location.X + this.Offset.X;
            connectionLine.Y1 = this.BaseItem.location.Y + this.Offset.Y;

            connectionLine.X2 = ConnectedWith.BaseItem.location.X + ConnectedWith.Offset.X;
            connectionLine.Y2 = ConnectedWith.BaseItem.location.Y + ConnectedWith.Offset.Y;

            MainWindow.MainItemCanvas.Children.Add(connectionLine);
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
