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
    internal class PassiveItemResistor : ItemBaseModel
    {
        private double Ohm;
        public double GetOhm { get { return Ohm; } }
        public PassiveItemResistor(string modelImageUri, Point modelCoordinates, int Ohm) : base(modelImageUri, modelCoordinates)
        {
            elementImageUri = modelImageUri;
            locationCoordinates = modelCoordinates;
            IsSelected = false;
            Image = new Image();
            Endpoints = new List<Endpoint>();
            this.Ohm = Ohm;
            InfoBorder = CreateInfoBorder(("Ohm: ", this, Ohm, this.GetType()));

            CreateModel(new List<Endpoint> { new Endpoint(this, new Point(37.5, 0)),
                                             new Endpoint(this, new Point(37.5, 72)) });
        }
        public void SetOhm(double Ohm)
        {
            this.Ohm = Ohm;
            MessageBox.Show($"Ohm on element set as: {Ohm}");
        }
    }
}
