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
        private bool IsDragging = false;
        private Point elementLocation = new Point(0, 0);
        private Point mouseOffset;

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
            IsSelected = false;
            Image = new Image();
            Endpoints = new List<Endpoint>();
        }
        public override void CreateModel()
        {
            Element = new Border();
            Element.Width = 75;
            Element.Height = 75;
            Element.Background = new SolidColorBrush(Colors.Transparent);

            Endpoint endpoint = new Endpoint(this, new Point(36.5, 2));
            Endpoints.Add(endpoint);

            Canvas.SetLeft(Element, elementLocation.X);
            Canvas.SetTop(Element, elementLocation.Y);
            Canvas.SetZIndex(Element, 1);

            SetImage(elementImageUri, Image, Element);

            Element.MouseDown += Border_MouseDown;
            Element.MouseMove += Border_MouseMove;
            Element.MouseUp += Border_MouseUp;
            Element.KeyDown += Border_KeyDown;

            MainWindow.MainItemCanvas.Children.Add(Element);
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                IsDragging = true;
                mouseOffset = e.GetPosition(Element);
                Element.CaptureMouse();

                ChangeSelection();
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
            if (IsDragging)
            {
                Point currentPos = e.GetPosition(MainWindow.MainItemCanvas);
                double x = currentPos.X - mouseOffset.X;
                double y = currentPos.Y - mouseOffset.Y;
                Canvas.SetLeft(Element, x);
                Canvas.SetTop(Element, y);

                foreach (var point in Endpoints)
                {
                    point.EndpointObject.RenderTransform = new TranslateTransform(x + point.Offset.X,
                                                                                  y + point.Offset.Y);
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

        public override void DestructModel()
        {
            MessageBox.Show("Модель разрушена!");
        }
    }
}