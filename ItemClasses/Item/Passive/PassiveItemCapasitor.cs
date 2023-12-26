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
    internal class PassiveItemCapasitor : ItemBaseModel
    {
        private double Farad;
        public double GetFarad { get => Farad; }
        public PassiveItemCapasitor(string modelImageUri, Point modelCoordinates, int Farad) : base(modelImageUri, modelCoordinates)
        {
            elementImageUri = modelImageUri;
            locationCoordinates = modelCoordinates;
            IsSelected = false;
            Image = new Image();
            Endpoints = new List<Endpoint>();
            this.Farad = Farad;
            InfoBorder = CreateInfoBorder(("Farad: ", this, Farad, this.GetType()));

            CreateModel(new List<Endpoint> { new Endpoint(this, new Point(36.6, 0)),
                                             new Endpoint(this, new Point(36.6, 72)) });
        }
        public void SetFarad(double Farad)
        {
            this.Farad = Farad;
            MessageBox.Show($"Farad on element set as: {Farad}");
        }
    }
}
