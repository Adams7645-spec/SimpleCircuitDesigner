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
    internal class PassiveItemInductor : ItemBaseModel
    {
        private int Henry;
        public PassiveItemInductor(string modelImageUri, Point modelCoordinates, int Henry) : base(modelImageUri, modelCoordinates)
        {
            elementImageUri = modelImageUri;
            locationCoordinates = modelCoordinates;
            IsSelected = false;
            Image = new Image();
            Endpoints = new List<Endpoint>();
            this.Henry = Henry;

            CreateModel(new List<Endpoint> { new Endpoint(this, new Point(36.9, 0)),
                                             new Endpoint(this, new Point(36.9, 72)) });
        }
        public void SetHenry(int Henry)
        {
            this.Henry = Henry;
        }
    }
}
