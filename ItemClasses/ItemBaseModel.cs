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
using System.Xml.Linq;
using System.Reflection;
using System.Windows.Media.Media3D;
using System.Globalization;

namespace SimpleCircuitDesigner
{
    internal abstract class ItemBaseModel
    {
        protected string imageUri;
        protected string elementImageUri = "";
        protected bool IsSelected;
        protected bool IsImageRequired = true;
        protected bool IsDragging = false;
        protected bool IsInfoBorderOpened = false;
        protected Image Image;
        protected Border Element;
        protected Border InfoBorder;
        protected Border dynamicBorder;
        protected Point mouseOffset;
        protected Point locationCoordinates;
        protected List<(string, object, object, TextBox, Type)> InfoBorderValues;
        public Border BaseElement { get { return Element; } }
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
            if (IsImageRequired)
                Element.Child = Image;
            if (!IsImageRequired)
                UpdateBorderColor();
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

            if (!IsImageRequired)
                UpdateBorderColor();

            if (IsImageRequired)
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

            Element.Cursor = Cursors.ScrollAll;

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

                Canvas.SetLeft(InfoBorder, x + 100);
                Canvas.SetTop(InfoBorder, y);

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
            else if(e.ChangedButton == MouseButton.Right)
            {
                IsInfoBorderOpened = !IsInfoBorderOpened;
                if (IsInfoBorderOpened)
                    MainWindow.MainItemCanvas.Children.Add(InfoBorder);
                else
                    MainWindow.MainItemCanvas.Children.Remove(InfoBorder);
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
        private void UpdateBorderColor()
        {
            if (IsSelected)
                dynamicBorder.BorderBrush = Brushes.Red;
            else
                dynamicBorder.BorderBrush = Brushes.Gray;
        }
        protected Border CreateInfoBorder(params (string, object, object, Type)[] elementParamsTextValues)
        {
            InfoBorderValues = new List<(string, object, object, TextBox, Type)>();
            Border border = new Border
            {
                BorderBrush = Brushes.Gray,
                Background = Brushes.LightGray,
                BorderThickness = new Thickness(3),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                CornerRadius = new CornerRadius(15),
                Child = null
            };

            Grid grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();

            for (int i = 0; i < elementParamsTextValues.Length; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var label = new Label
                {
                    Content = elementParamsTextValues[i].Item1,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(5, 5, 0, 5),
                    FontFamily = new FontFamily("Consolas")
                };

                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);

                var textBox = new TextBox
                {
                    Width = 75,
                    Margin = new Thickness(0, 5, 5, 5),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontFamily = new FontFamily("Consolas")
                };

                textBox.KeyDown += KeyDown;

                Grid.SetRow(textBox, i);
                Grid.SetColumn(textBox, 1);

                grid.Children.Add(label);
                grid.Children.Add(textBox);

                InfoBorderValues.Add(new(elementParamsTextValues[i].Item1,
                         elementParamsTextValues[i].Item2,
                         elementParamsTextValues[i].Item3,
                         textBox,
                         elementParamsTextValues[i].Item4));
            }

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

            grid.Loaded += (sender, e) =>
            {
                double width = 0;
                double height = 0;

                foreach (UIElement child in grid.Children)
                {
                    if (child is FrameworkElement element)
                    {
                        width = Math.Max(width, element.Height);
                        height += element.Height;
                    }
                }

                border.Width = width;
                border.Height = height;
            };

            border.Child = grid;

            Canvas.SetLeft(border, locationCoordinates.X + 100);
            Canvas.SetTop(border, locationCoordinates.Y);
            Canvas.SetZIndex(border, 3);

            return border;
        }
        private void KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(textBox.Text))
            {
                for (int i = 0; i < InfoBorderValues.Count; i++)
                    if (InfoBorderValues[i].Item4 == textBox)
                    {
                        var type = InfoBorderValues[i].Item5;
                        var element = InfoBorderValues[i].Item2;
                        var value = InfoBorderValues[i].Item3;
                        var valueToSet = InfoBorderValues[i].Item4.Text;
                        switch (type.Name)
                        {
                            case "EssentialNameNode":
                                var tempNameNode = (EssentialNameNode)element;
                                tempNameNode.SetNodeText((string)valueToSet);
                                break;
                            case "EssentialItemGND":
                                var tempGND = (EssentialItemGND)element;
                                if (textBox.Text == "true")
                                    tempGND.SetGrouded(bool.Parse(valueToSet));
                                else if(textBox.Text == "false")
                                    tempGND.SetGrouded(bool.Parse(valueToSet));
                                else
                                    MessageBox.Show("Type 'true' or 'false'");
                                break;
                            case "DCSourcesCurrentSource":
                                var tempCurrentSource = (DCSourcesCurrentSource)element;
                                try
                                {
                                    var tempAmperage = double.Parse(valueToSet);
                                    tempCurrentSource.SetAmperage(tempAmperage);
                                }
                                catch
                                {
                                    MessageBox.Show("Введите значение вида 0,0");
                                }
                                break;
                            case "DCSourcesVoltageSource":
                                var tempVoltageSource = (DCSourcesVoltageSource)element;
                                try
                                {
                                    var tempVoltage = double.Parse(valueToSet);
                                    tempVoltageSource.SetVoltage(tempVoltage);
                                }
                                catch
                                {
                                    MessageBox.Show("Введите значение вида 0,0");
                                }
                                break;
                            case "PassiveItemCapasitor":
                                var tempCapasitor = (PassiveItemCapasitor)element;
                                try
                                {
                                    var tempFarad = double.Parse(valueToSet);
                                    tempCapasitor.SetFarad(tempFarad);
                                }
                                catch
                                {
                                    MessageBox.Show("Введите значение вида 0,0");
                                }
                                break;
                            case "PassiveItemInductor":
                                var tempInductor = (PassiveItemInductor)element;
                                try
                                {
                                    var tempHenry = double.Parse(valueToSet);
                                    tempInductor.SetHenry(tempHenry);
                                }
                                catch
                                {
                                    MessageBox.Show("Введите значение вида 0,0");
                                }
                                break;
                            case "PassiveItemResistor":
                                var tempResistor = (PassiveItemResistor)element;
                                try
                                {
                                    var tempOhm = double.Parse(valueToSet);
                                    tempResistor.SetOhm(tempOhm);
                                }
                                catch
                                {
                                    MessageBox.Show("Введите значение вида 0,0");
                                }
                                break;
                            case "SwitchesSPDTGate":
                                var tempSPDT = (SwitchesSPDTGate)element;
                                if (textBox.Text == "up")
                                    tempSPDT.SetUpOrDown(valueToSet);
                                else if (textBox.Text == "down")
                                    tempSPDT.SetUpOrDown(valueToSet);
                                else
                                    MessageBox.Show("Type 'true' or 'false'");
                                break;
                            case "SwitchesSPSTGate":
                                var tempSPST = (SwitchesSPSTGate)element;
                                if (textBox.Text == "true")
                                    tempSPST.SetIsClosed(bool.Parse(valueToSet));
                                else if (textBox.Text == "false")
                                    tempSPST.SetIsClosed(bool.Parse(valueToSet));
                                else
                                    MessageBox.Show("Type 'true' or 'false'");
                                break;
                            case "SwitchesSPSTRelay":
                                var tempRelay = (SwitchesSPSTRelay)element;
                                switch (value.GetType().Name)
                                {
                                    case "Double":
                                        try
                                        {
                                            var tempRelayOhm = double.Parse(valueToSet);
                                            tempRelay.SetOhm(tempRelayOhm);
                                        }
                                        catch
                                        {
                                            MessageBox.Show("Введите значение вида 0,0");
                                        }
                                        break;
                                    case "Boolean":
                                        if (textBox.Text == "true")
                                            tempRelay.SetIsClosed(bool.Parse(valueToSet));
                                        else if (textBox.Text == "false")
                                            tempRelay.SetIsClosed(bool.Parse(valueToSet));
                                        else
                                            MessageBox.Show("Type 'true' or 'false'");
                                        break;
                                }
                                break;
                        }
                    }
            }
        }
    }
}
