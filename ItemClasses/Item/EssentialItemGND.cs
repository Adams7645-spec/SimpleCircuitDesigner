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
    class EssentialItemGND : ItemBaseModel
    {
        private AuxiliaryEnums.GNDDisplayType displayType = AuxiliaryEnums.GNDDisplayType.Signal;
        private string elementImageUri = "";
        private string elementName = "";
        private bool IsGrounded = false;
        private bool isDragging = false;
        private Point elementLocation = new Point(0, 0);
        private Point mouseOffset;
        private Border? Element;
        private List<Ellipse> endPoints = new List<Ellipse>();

        public EssentialItemGND(string DeffElementImageUri,
                                string ElementName,
                                Point ElementCoordinates,
                                AuxiliaryEnums.GNDDisplayType DisplayType) : base(ElementName, DeffElementImageUri, ElementCoordinates)
        {
            IsGrounded = true;
            elementImageUri = DeffElementImageUri;
            elementName = ElementName;
            elementLocation = ElementCoordinates;
            displayType = DisplayType;
        }

        public void ChangeDisplayType(AuxiliaryEnums.GNDDisplayType type, Border border)
        {
            displayType = type;
            switch (type)
            {
                case AuxiliaryEnums.GNDDisplayType.Signal:
                    SetElementImage(new Uri(@"E:/Programm/SimpleCircuitDesigner/ImageSource/GND.png", UriKind.RelativeOrAbsolute), border);
                    break;
                case AuxiliaryEnums.GNDDisplayType.Earth:
                    SetElementImage(new Uri(@"", UriKind.RelativeOrAbsolute), border);
                    break;
                case AuxiliaryEnums.GNDDisplayType.Chassis:
                    SetElementImage(new Uri(@"", UriKind.RelativeOrAbsolute), border);
                    break;
            }
        }

        public override void CreateModel()
        {
            var endPoint = CreateEndPoint(new TranslateTransform(31.5, 0));
            endPoints.Add(endPoint);
            Element = new Border();
            Element.Width = 75;
            Element.Height = 75;
            Element.Background = new SolidColorBrush(Colors.Transparent);

            Canvas.SetLeft(Element, elementLocation.X);
            Canvas.SetTop(Element, elementLocation.Y);
            Canvas.SetLeft(endPoint, elementLocation.X);
            Canvas.SetTop(endPoint, elementLocation.Y);
            Canvas.SetZIndex(Element, 1);
            Canvas.SetZIndex(endPoint, 2);

            BitmapImage bitmapImage = new BitmapImage(new Uri(elementImageUri, UriKind.RelativeOrAbsolute));
            Image image = new Image();
            image.Source = bitmapImage;
            image.Stretch = Stretch.Fill;
            Element.Child = image;

            Element.MouseDown += Border_MouseDown;
            Element.MouseMove += Border_MouseMove;
            Element.MouseUp += Border_MouseUp;
            Element.KeyDown += Border_KeyDown;
            Element.MouseRightButtonDown += Border_MouseRightButtonDown;

            MainWindow.MainItemCanvas.Children.Add(endPoint);

            MainWindow.MainItemCanvas.Children.Add(Element);
        }

        private void Border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var relativeElement = sender as Border;
            ChangeDisplayType(AuxiliaryEnums.GNDDisplayType.Signal, relativeElement);
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isDragging = true;
                mouseOffset = e.GetPosition(Element);
                Element.CaptureMouse();
            }
        }

        private void Border_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                MessageBox.Show("Нажата клавиша Backspace!");
                DestructModel();
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPos = e.GetPosition(MainWindow.MainItemCanvas);
                double x = currentPos.X - mouseOffset.X;
                double y = currentPos.Y - mouseOffset.Y;
                Canvas.SetLeft(Element, x);
                Canvas.SetTop(Element, y);

                foreach (var point in endPoints)
                {
                    Canvas.SetTop(point, y);
                    Canvas.SetLeft(point, x);
                }
            }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && isDragging)
            {
                isDragging = false;
                Element.ReleaseMouseCapture();
            }

            MainWindow.MainItemCanvas.Focus();
        }
        public override void DestructModel()
        {
            MessageBox.Show("Модель разрушена!");
        }

        private void SetElementImage(Uri uri, Border border)
        {
            if (border.Child is Image image)
            {
                image.Source = new BitmapImage(uri);
            }
            else
            {
                Image newImage = new Image();
                newImage.Source = new BitmapImage(uri);
                newImage.Stretch = Stretch.Fill;
                border.Child = newImage;
            }
        }
    }
}