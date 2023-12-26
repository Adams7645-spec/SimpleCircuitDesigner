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
    internal class DCSourcesVoltageSource : ItemBaseModel
    {
        private double Voltage;
        public double getVoltage { get { return Voltage; } }
        public DCSourcesVoltageSource(string modelImageUri, Point modelCoordinates, int Voltage) : base(modelImageUri, modelCoordinates)
        {
            elementImageUri = modelImageUri;
            locationCoordinates = modelCoordinates;
            IsSelected = false;
            Image = new Image();
            this.Voltage = Voltage;
            Endpoints = new List<Endpoint>();
            InfoBorder = CreateInfoBorder(("Voltage: ", this, Voltage, this.GetType()));

            CreateModel(new List<Endpoint> { new Endpoint(this, new Point(36.8, 0)),
                                             new Endpoint(this, new Point(36.8, 72)) });
        }
        public void SetVoltage(double Voltage)
        {
            this.Voltage = Voltage;
            MessageBox.Show($"Voltage on element set as: {Voltage}");
        }
    }
}
