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
    internal abstract class ItemBaseModel
    {
        protected string imageUri;
        protected Point locationCoordinates;
        protected bool IsSelected;
        protected Image Image;
        protected Border Element;
        protected string elementImageUri = "";
        protected bool IsDragging = false;
        protected Point mouseOffset;
        public List<Endpoint> Endpoints { get; protected set; }
        public Point location { get { return locationCoordinates; } }
        protected ItemBaseModel(string modelImageUri, Point modelCoordinates)
        {
            imageUri = modelImageUri;
            locationCoordinates = modelCoordinates;
        }
        protected void SetImage(Image Image, Border Element)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(imageUri, UriKind.RelativeOrAbsolute));
            Image.Source = bitmapImage;
            Image.Stretch = Stretch.Fill;
            Element.Child = Image;
        }
        protected void SetImage(SolidColorBrush imageColor, Border Element)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(imageUri, UriKind.RelativeOrAbsolute));

            ImageBrush imageBrush = new ImageBrush(bitmapImage);
            imageBrush.Stretch = Stretch.Fill;

            System.Windows.Media.Effects.BlurEffect blur = new System.Windows.Media.Effects.BlurEffect();
            blur.Radius = 0;

            Rectangle rectangle = new Rectangle();
            rectangle.Fill = imageColor;
            rectangle.Effect = blur;

            rectangle.OpacityMask = imageBrush;
            rectangle.Stretch = Stretch.Fill;

            Element.Child = rectangle;
        }
        public void ChangeLocation(Point newLocation)
        {
            locationCoordinates = newLocation;
        }
        public void ChangeSelection()
        {
            if (MainWindow.IsSelectionAvailable)
            {
                if (MainWindow.WiringMode == true)
                {
                    if (!IsSelected || MainWindow.IsSelectionAvailable)
                    {
                        IsSelected = !IsSelected;
                        MainWindow.SelectItem(this);

                        if (IsSelected)
                            SetImage(new SolidColorBrush(Colors.Red), Element);
                        else
                            SetImage(Image, Element);
                    }
                }
            }
            else if (MainWindow.WiringMode == true && IsSelected)
            {
                IsSelected = false;
                MainWindow.SelectItem(this);
                SetImage(Image, Element);
            }
        }
        public void DropSelection()
        {
            IsSelected = false;
            SetImage(Image, Element);
        }
        public void CreateModel(List<Endpoint> endpoints)
        {
            Element = new Border();
            Element.Width = 75;
            Element.Height = 75;
            Element.Background = new SolidColorBrush(Colors.Transparent);

            foreach (var point in endpoints)
            {
                Endpoints.Add(point);
            }

            Canvas.SetLeft(Element, locationCoordinates.X);
            Canvas.SetTop(Element, locationCoordinates.Y);
            Canvas.SetZIndex(Element, 1);

            SetImage(Image, Element);

            Element.MouseDown += Border_MouseDown;
            Element.MouseMove += Border_MouseMove;
            Element.MouseUp += Border_MouseUp;
            Element.KeyDown += Border_KeyDown;

            MainWindow.MainItemCanvas.Children.Add(Element);
        }
        private void Border_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                MessageBox.Show("Нажата клавиша Backspace!");
            }
        }
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDragging)
            {
                Point currentPos = e.GetPosition(MainWindow.MainItemCanvas);
                double x = currentPos.X - mouseOffset.X;
                double y = currentPos.Y - mouseOffset.Y;
                Canvas.SetLeft(Element, x);
                Canvas.SetTop(Element, y);

                locationCoordinates = new Point(x, y);

                foreach (var point in Endpoints)
                {
                    point.EndpointObject.RenderTransform = new TranslateTransform(x + point.Offset.X,
                                                                                  y + point.Offset.Y);
                    point.UpdateConnectionWire();
                    if (point.ConnectedWith != null)
                    {
                        point.ConnectedWith.UpdateConnectionWire();
                    }
                }
            }
        }
        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && IsDragging)
            {
                IsDragging = false;
                Element.ReleaseMouseCapture();
            }

            MainWindow.MainItemCanvas.Focus();
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.IsDeletionModeEntered)
            {
                DestructItem();
                return;
            }
            if (e.ChangedButton == MouseButton.Left)
            {
                IsDragging = true;
                mouseOffset = e.GetPosition(Element);
                Element.CaptureMouse();

                ChangeSelection();
            }
        }
        public void DestructItem()
        {
            foreach (var point in Endpoints)
            {
                point.Disconnect();
                MainWindow.MainItemCanvas.Children.Remove(point.EndpointObject);
            }
            ChangeSelection();
            MainWindow.MainItemCanvas.Children.Remove(Element);
        }
    }
}
