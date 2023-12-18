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
    internal class SwitchesSPSTRelay : ItemBaseModel
    {
        public SwitchesSPSTRelay(string modelImageUri, Point modelCoordinates) : base(modelImageUri, modelCoordinates)
        {
            elementImageUri = modelImageUri;
            locationCoordinates = modelCoordinates;
            IsSelected = false;
            Image = new Image();
            Endpoints = new List<Endpoint>();

            CreateModel(new List<Endpoint> { new Endpoint(this, new Point(15.4, 3)),
                                             new Endpoint(this, new Point(15.4, 70)),
                                             new Endpoint(this, new Point(46.3, 3)),
                                             new Endpoint(this, new Point(46.3, 70)) });
        }
    }
}
