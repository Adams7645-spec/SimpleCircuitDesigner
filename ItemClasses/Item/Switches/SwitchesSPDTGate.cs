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
    internal class SwitchesSPDTGate : ItemBaseModel
    {
        public SwitchesSPDTGate(string modelImageUri, Point modelCoordinates) : base(modelImageUri, modelCoordinates)
        {
            elementImageUri = modelImageUri;
            locationCoordinates = modelCoordinates;
            IsSelected = false;
            Image = new Image();
            Endpoints = new List<Endpoint>();

            CreateModel(new List<Endpoint> { new Endpoint(this, new Point(0,42.7)),
                                             new Endpoint(this, new Point(74, 42.7)),
                                             new Endpoint(this, new Point(74, 26.4)) });
        }
    }
}
