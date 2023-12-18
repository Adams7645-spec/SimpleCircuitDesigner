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
    internal class DCSourcesCurrentSource : ItemBaseModel
    {
        private int Amperage;
        public DCSourcesCurrentSource(string modelImageUri, Point modelCoordinates, int Amperage) : base(modelImageUri, modelCoordinates)
        {
            elementImageUri = modelImageUri;
            locationCoordinates = modelCoordinates;
            IsSelected = false;
            Image = new Image();
            this.Amperage = Amperage;
            Endpoints = new List<Endpoint>();

            CreateModel(new List<Endpoint> { new Endpoint(this, new Point(36.4, 0)),
                                             new Endpoint(this, new Point(36.4, 72)) });
        }
        public void SetAmperage(int Amperage)
        {
            this.Amperage = Amperage;
        }
    }
}