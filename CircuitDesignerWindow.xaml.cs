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
using System.Xml.Linq;

namespace SimpleCircuitDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool IsWiringModeEntered = false;
        private bool IsSimulationEntered = false;
        private static bool IsItemDeletionModeEntered = false;
        private bool IsItemPanelCollapsed = false;
        private static bool isSelectionAvailable = true;
        private List<ItemBaseModel> Models = new List<ItemBaseModel>();
        private List<Endpoint> Endpoints = new List<Endpoint>();
        private static Dictionary<int, ItemBaseModel> selectedItems = new Dictionary<int, ItemBaseModel>();
        private Circuit circuit;
        public static bool IsDeletionModeEntered { get { return IsItemDeletionModeEntered; } }
        public static bool IsSelectionAvailable { get { return isSelectionAvailable; } }
        public static bool WiringMode { get { return IsWiringModeEntered; } }
        public static Canvas? MainItemCanvas { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            ItemField.Focusable = true;
            ItemField.Focus();

            MainItemCanvas = ItemField;
        }
        private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            IsItemDeletionModeEntered = !IsItemDeletionModeEntered;
            if (IsItemDeletionModeEntered)
                this.Cursor = Cursors.Cross;
            else
                this.Cursor = Cursors.Arrow;
        }
        private void Button_EnterSimulation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Button_EnterSimulation.Margin = new Thickness(0, 5, 0, 0);
        }
        private void Button_CollapseItemPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BottomGrid.Margin = new Thickness(-272, 20, 15, 15);

        }
        private void EssentialDesignElement_Wire_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeWiringMode();
        }
        private void Button_EnterSimulation_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Button_EnterSimulation.Margin = new Thickness(0, 0, 0, 0);
            IsSimulationEntered = ChangeButtonVisualCondition(IsSimulationEntered, Button_EnterSimulation, 
                "/ImageSource/PauseButton.png",
                "/ImageSource/PlayButton.png", 
                       new Thickness(10));

            if (IsSimulationEntered)
            {
                SimulationStatPanel.Visibility = Visibility.Visible;
                SetCircuitParams();
                CircuitCalculate();
            }
            else
                SimulationStatPanel.Visibility = Visibility.Hidden;
        }
        private void Button_CollapseItemPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsItemPanelCollapsed = ChangeButtonVisualCondition(IsItemPanelCollapsed, Button_CollapseItemPanel,
                                   "/ImageSource/Right-arrow.png",
                                   "/ImageSource/left-arrow.png", 
                                   new Thickness(2.5, 7.5, 7.5, 7.5), new Thickness(7.5, 7.5, 2.5, 7.5));
            CollapseFrameworkElement(new Thickness(0, 0, 300, 0), new Thickness(15), ItemBorder, IsItemPanelCollapsed);

            if (IsItemPanelCollapsed)
            {
                Button_CollapseItemPanel.SetValue(Grid.ColumnProperty, 1);
                BottomGrid.Margin = new Thickness(-272, 15, 15, 15);
            }
            else
            {
                Button_CollapseItemPanel.SetValue(Grid.ColumnProperty, 3);
                BottomGrid.Margin = new Thickness(-272, 15, 15, 15);
            }
        }
        private void WireCreateButton_Click(object sender, RoutedEventArgs e)
        {
            Endpoint firstSelectedEndpoint;
            Endpoint secondSelectedEndpoint;

            if (selectedItems.Values.ElementAtOrDefault(0) == null || selectedItems.Values.ElementAtOrDefault(1) == null) 
                return;

            if (IsAnyEndpointEmpty(selectedItems.Values.ElementAtOrDefault(0)) && 
                IsAnyEndpointEmpty(selectedItems.Values.ElementAtOrDefault(1)))
            {
                firstSelectedEndpoint = GetEmptyEndpoint(selectedItems.Values.ElementAtOrDefault(0));
                secondSelectedEndpoint = GetEmptyEndpoint(selectedItems.Values.ElementAtOrDefault(1));

                if (firstSelectedEndpoint != null && secondSelectedEndpoint != null)
                {
                    if (!firstSelectedEndpoint.IsConnected)
                    {
                        firstSelectedEndpoint?.ConnectCall(secondSelectedEndpoint);
                    }
                }
            }
        }
        private void WireDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var firstSelectedEndpoint = selectedItems.Values.ElementAtOrDefault(0)?.Endpoints?[0];

            if (firstSelectedEndpoint != null)
                if (firstSelectedEndpoint.IsConnected)
                    firstSelectedEndpoint?.Disconnect();
        }
        private void EssentialDesignElement_GND_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Models.Add(new EssentialItemGND("pack://application:,,,/SimpleCircuitDesigner;component/ImageSource/GND.png", new Point(600, 300)));
        }
        private void EssentialDesignElement_NameNode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Models.Add(new EssentialNameNode("pack://application:,,,/SimpleCircuitDesigner;component/ImageSource/Transparent.png", new Point(600, 300)));
        }
        private void DCSourcesDesigner_VoltageSource_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Models.Add(new DCSourcesVoltageSource("pack://application:,,,/SimpleCircuitDesigner;component/ImageSource/VoltageSource.png", new Point(600, 300), 1));
        }
        private void DCSourcesDesigner_CurrentSource_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Models.Add(new DCSourcesCurrentSource("pack://application:,,,/SimpleCircuitDesigner;component/ImageSource/CurrentSource.png", new Point(600, 300), 1));
        }
        private void PassiveElements_Resistor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Models.Add(new PassiveItemResistor("pack://application:,,,/SimpleCircuitDesigner;component/ImageSource/Resistor.png", new Point(600, 300), 1));
        }
        private void PassiveElements_Capasitor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Models.Add(new PassiveItemCapasitor("pack://application:,,,/SimpleCircuitDesigner;component/ImageSource/Capasitor.png", new Point(600, 300), 1));
        }
        private void PassiveElements_Inductor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Models.Add(new PassiveItemInductor("pack://application:,,,/SimpleCircuitDesigner;component/ImageSource/Inductor.png", new Point(600, 300), 1));
        }
        private void Switches_SPSTGate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Models.Add(new SwitchesSPSTGate("pack://application:,,,/SimpleCircuitDesigner;component/ImageSource/SPSTGate.png", new Point(600, 300)));
        }
        private void Switches_SPDTGate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Models.Add(new SwitchesSPDTGate("pack://application:,,,/SimpleCircuitDesigner;component/ImageSource/SPDTGate.png", new Point(600, 300)));
        }
        private void Switches_SPSTRelay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Models.Add(new SwitchesSPSTRelay("pack://application:,,,/SimpleCircuitDesigner;component/ImageSource/SPSTRelay.png", new Point(600, 300)));
        }
        private void Button_CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        //                                        //
        //          Auxiliary Handlers            //
        //                                        //

        private bool ChangeButtonVisualCondition(bool condition, ToggleButton button, string trueImage, string falseImage, Thickness ImageMargin)
        {
            if (condition != true)
            {
                condition = true;
                SetButtonImage(trueImage, button, ImageMargin);
            }
            else
            {
                condition = false;
                SetButtonImage(falseImage, button, ImageMargin);
            }
            return condition;
        }
        private bool ChangeButtonVisualCondition(bool condition, ToggleButton button, string trueImage, string falseImage, Thickness ImageMargin, Thickness ImageAditionalMargin)
        {
            if (condition != true)
            {
                condition = true;
                SetButtonImage(trueImage, button, ImageAditionalMargin);
            }
            else
            {
                condition = false;
                SetButtonImage(falseImage, button, ImageMargin);
            }
            return condition;
        }
        private void SetButtonImage(string uri, ToggleButton button, Thickness ImageMargin)
        {
            var PauseImage = new Image();
            var BitmapImage = new BitmapImage(new Uri(uri, UriKind.Relative));
            PauseImage.Source = BitmapImage;
            PauseImage.Margin = ImageMargin;
            button.Content = PauseImage;
        }
        private void CollapseFrameworkElement(Thickness CollapseMargin, Thickness DefMargin, FrameworkElement targerBorder, bool condition)
        {
            if (condition != true)
            {
                targerBorder.Margin = DefMargin;
            }
            else
            {
                targerBorder.Margin = CollapseMargin;
            }
        }
        private void ChangeWiringMode()
        {
            try
            {
                if (!IsWiringModeEntered)
                {
                    EssentialDesignElement_Wire.Background = new SolidColorBrush(Colors.DarkGray);
                    foreach (var model in Models)
                    {
                        model.BaseElement.Cursor = Cursors.Hand;
                        foreach (var endpoint in model.Endpoints)
                        {
                            Endpoints.Add(endpoint);
                        }
                    }
                    foreach (var endpoint in Endpoints)
                    {
                        if (!endpoint.IsConnected)
                            endpoint.ChangeAccentColor(Brushes.Red);
                    }
                    WiringPanel.Visibility = Visibility.Visible;
                    IsWiringModeEntered = true;
                }
                else
                {
                    foreach (var endpoint in Endpoints)
                    {
                        endpoint.ChangeAccentColor(Brushes.DimGray);
                    }
                    foreach (var model in Models)
                    {
                        model.DropSelection();
                        model.BaseElement.Cursor = Cursors.ScrollAll;
                    }
                    Endpoints.Clear();
                    ResetSelection();
                    WiringPanel.Visibility = Visibility.Hidden;
                    EssentialDesignElement_Wire.Background = new SolidColorBrush(Colors.LightGray);
                    IsWiringModeEntered = false;
                }
            }
            catch
            {
                throw;
            }
        }
        public static void SelectItem(object item)
        {
            try
            {
                var itemBaseModel = (ItemBaseModel)item;
                int hashCode = itemBaseModel.GetHashCode();

                if (IsItemSelected(itemBaseModel))
                {
                    RemoveItemFromSelection(hashCode);
                }
                else
                {
                    AddItemToSelection(hashCode, itemBaseModel);
                }
                UpdateSelectionAvailability();
            }
            catch
            {
                throw;
            }
        }
        public static void ResetSelection()
        {
            selectedItems.Clear();
            UpdateSelectionAvailability();
        }
        private static void UpdateSelectionAvailability()
        {
            isSelectionAvailable = selectedItems.Count < 2;
        }
        private static bool IsItemSelected(ItemBaseModel item)
        {
            return selectedItems.ContainsKey(item.GetHashCode());
        }
        private static void RemoveItemFromSelection(int hashCode)
        {
            if (selectedItems.ContainsKey(hashCode))
            {
                selectedItems.Remove(hashCode);
            }
        }
        private static void AddItemToSelection(int hashCode, ItemBaseModel item)
        {
            if (selectedItems.Count < 2)
            {
                selectedItems[hashCode] = item;
            }
        }
        private static bool IsAnyEndpointEmpty(ItemBaseModel item)
        {
            foreach (var endpoint in item.Endpoints)
            {
                if (!endpoint.IsConnected)
                    return true;
            }
            return false;
        }
        private static Endpoint GetEmptyEndpoint(ItemBaseModel item)
        {
            foreach (var endpoint in item.Endpoints)
            {
                if (!endpoint.IsConnected)
                    return endpoint;
            }
            return null;
        }
        private void SetCircuitParams()
        {
            double resistanse = 0;
            double voltage = 0;
            double current = 0;
            double inductance = 0;
            double capacitance = 0;

            foreach (var model in Models)
            {
                var IsModelViable = true;

                foreach (var point in model.Endpoints)
                {
                    if (!point.IsConnected)
                    {
                        IsModelViable = false;
                    }
                }

                if (IsModelViable)
                {
                    switch (model.GetType().Name)
                    {
                        case "DCSourcesCurrentSource":
                            var tempCurrentSource = (DCSourcesCurrentSource)model;
                            current += tempCurrentSource.GetAmperage;
                            break;
                        case "DCSourcesVoltageSource":
                            var tempVoltageSource = (DCSourcesVoltageSource)model;
                            voltage += tempVoltageSource.getVoltage;
                            break;
                        case "PassiveItemCapasitor":
                            var tempCapasitor = (PassiveItemCapasitor)model;
                            capacitance += tempCapasitor.GetFarad;
                            break;
                        case "PassiveItemInductor":
                            var tempInductor = (PassiveItemInductor)model;
                            inductance += tempInductor.GetHenry;
                            break;
                        case "PassiveItemResistor":
                            var tempResistor = (PassiveItemResistor)model;
                            resistanse += tempResistor.GetOhm;
                            break;
                        case "SwitchesSPSTRelay":
                            var tempRelay = (PassiveItemResistor)model;
                            resistanse += tempRelay.GetOhm;
                            break;
                    }
                }
            }

            circuit = new Circuit(resistanse, voltage, current, inductance, capacitance);
        }
        private void CircuitCalculate()
        {
            circuit.CalculateCurrent();
            circuit.CalculatePower();
            circuit.CalculateResistance();
            circuit.CalculateVoltage();
            circuit.CalculateImpedance();
            circuit.CalculateFrequency();

            CurrentLabel.Content = Math.Round(circuit.Current, 3);
            EnergyLabel.Content = Math.Round(circuit.CalculateEnergy(0.2), 3);
            PowerLabel.Content = Math.Round(circuit.Power, 3);
            ResistanceLabel.Content = Math.Round(circuit.Resistance, 3);
            VoltageLabel.Content = Math.Round(circuit.Voltage, 3);
            InductanceLabel.Content = Math.Round(circuit.Inductance, 3);
            CapacitanceLabel.Content = Math.Round(circuit.Capacitance, 3);
            ImpedanceLabel.Content = Math.Round(circuit.CalculateImpedance(), 3);

        }
    }
}
