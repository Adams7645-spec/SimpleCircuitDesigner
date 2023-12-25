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
    internal class EssentialNameNode : ItemBaseModel
    {
        string NodeText { get; set; }
        public EssentialNameNode(string DeffElementImageUri,
                                Point ElementCoordinates) : base(DeffElementImageUri, ElementCoordinates)
        {
            elementImageUri = DeffElementImageUri;
            locationCoordinates = ElementCoordinates;
            IsSelected = false;
            Image = new Image();
            Endpoints = new List<Endpoint>();
            IsImageRequired = false;
            dynamicBorder = new Border();
            InfoBorder = CreateInfoBorder(("Node text: ", this, NodeText, this.GetType()));

            CreateModel(new List<Endpoint> { new Endpoint(this, new Point(38.5, -3.5)) });
            AddDynamicLabel(Element);
        }
        public void SetNodeText(string nodeText)
        {
            NodeText = nodeText;
            UpdateBorderLabel(NodeText);
        }
        private void AddDynamicLabel(Border parentBorder)
        {
            dynamicBorder.BorderBrush = Brushes.Gray;
            dynamicBorder.BorderThickness = new Thickness(3);
            dynamicBorder.CornerRadius = new CornerRadius(15);
            dynamicBorder.Background = Brushes.Bisque;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Enter text";
            textBlock.FontSize = 15;
            textBlock.FontFamily = new FontFamily("Consolas");
            textBlock.FontWeight = FontWeights.SemiBold;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.TextWrapping = TextWrapping.Wrap;

            textBlock.SizeChanged += (sender, e) =>
            {
                dynamicBorder.Width = e.NewSize.Width + 15;
                dynamicBorder.Height = e.NewSize.Height + 15;

                if (parentBorder != null)
                {
                    parentBorder.Width = dynamicBorder.Width;
                    parentBorder.Height = dynamicBorder.Height;
                    Endpoints[0].ReplaceEndpoint(new Point(parentBorder.Width / 2, -3.5));
                }
            };

            Canvas.SetZIndex(dynamicBorder, 2);
            dynamicBorder.Child = textBlock;
            parentBorder.Child = dynamicBorder;
        }
        private void UpdateBorderLabel(string newLabelText)
        {
            if (dynamicBorder.Child is TextBlock block)
            {
                block.Text = newLabelText;
            }
        }
    }
}
