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
    internal class Circuit
    {
        public double Voltage { get; set; }
        public double Resistance { get; set; }
        public double Current { get; set; }
        public double Power { get; private set; }
        public double Energy { get; private set; }
        public double Impedance { get; private set; }
        public double Inductance { get; set; }
        public double Capacitance { get; set; }
        public double Frequency { get; set; }

        public Circuit(double resistance, double voltage, double current, double inductance, double capacitance)
        {
            Resistance = resistance;
            Voltage = voltage;
            Current = current;
            Inductance = inductance;
            Capacitance = capacitance;
        }

        public void CalculateVoltage()
        {
            Voltage = Current * Resistance;
        }

        public void CalculateCurrent()
        {
            Current = Voltage / Resistance;
        }

        public void CalculateResistance()
        {
            Resistance = Voltage / Current;
        }

        public void CalculatePower()
        {
            Power = Voltage * Current;
        }

        public double CalculateEnergy(double timeInHours)
        {
            Energy = Power * timeInHours;
            return Energy;
        }

        public double CalculateImpedance()
        {
            Impedance = 1 / (2 * Math.PI * Frequency * Resistance);
            return Impedance;
        }

        public double CalculateFrequency()
        {
            Frequency = 1 / (2 * Math.PI * Math.Sqrt(Capacitance * Inductance));
            return Frequency;
        }
        public void SetCircuitParameters(double resistance, double voltage, double current, double inductance, double capacitance)
        {
            Resistance = resistance;
            Voltage = voltage;
            Current = current;
            Inductance = inductance;
            Capacitance = capacitance;
        }
    }
}
