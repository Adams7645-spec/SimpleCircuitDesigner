﻿using System;
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
        protected string name;
        protected string imageUri;
        protected Point locationCoordinates;
        protected bool IsSelected;
        protected Image Image;
        protected Border Element;
        public List<Endpoint> Endpoints { get; protected set; }
        public Point location { get { return locationCoordinates; } }
        protected ItemBaseModel(string modelName, string modelImageUri, Point modelCoordinates)
        {
            name = modelName;
            imageUri = modelImageUri;
            locationCoordinates = modelCoordinates;
        }
        protected void SetImage(string uri, Image Image, Border Element)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
            Image.Source = bitmapImage;
            Image.Stretch = Stretch.Fill;
            Element.Child = Image;
        }

        protected void SetImage(SolidColorBrush imageColor, string uri, Border Element)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));

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
                            SetImage(new SolidColorBrush(Colors.Red), imageUri, Element);
                        else
                            SetImage(imageUri, Image, Element);
                    }
                }
            }
            else if (MainWindow.WiringMode == true && IsSelected)
            {
                IsSelected = false;
                MainWindow.SelectItem(this);
                SetImage(imageUri, Image, Element);
            }
        }
        public void DropSelection()
        {
            IsSelected = false;
            MainWindow.SelectItem(null);
            SetImage(imageUri, Image, Element);
        }
        public abstract void CreateModel();
        public abstract void DestructModel();
    }
}
