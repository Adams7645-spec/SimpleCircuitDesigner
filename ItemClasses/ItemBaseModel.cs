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
        protected string name;
        protected string imageUri;
        protected Point locationCoordinates;
        protected ItemBaseModel(string modelName, string modelImageUri, Point modelCoordinates)
        {
            name = modelName;
            imageUri = modelImageUri;
            locationCoordinates = modelCoordinates;
        }
        public abstract void CreateModel();
        public abstract void DestructModel();
        protected Ellipse CreateEndPoint(TranslateTransform translate)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 10;
            ellipse.Height = 10;
            ellipse.Fill = Brushes.Transparent;
            ellipse.Stroke = Brushes.Black;
            ellipse.StrokeThickness = 2;
            TranslateTransform transform = translate;
            ellipse.RenderTransform = transform;
            return ellipse;
        }
    }
}
