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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool IsWiringModeEntered = false;
        private bool IsSimulationEntered = false;
        private bool IsItemPanelCollapsed = false;
        private static bool isSelectionAvailable = true;
        private string BaseImageFolderUri = @"E:\Programm\SimpleCircuitDesigner\ImageSource\";
        private List<ItemBaseModel> Models = new List<ItemBaseModel>();
        private List<Endpoint> Endpoints = new List<Endpoint>();
        private static ItemBaseModel[] selectedPair = new ItemBaseModel[2];

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
                       BaseImageFolderUri, "PauseButton.png", "PlayButton.png", 
                       new Thickness(10));
        }

        private void Button_CollapseItemPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsItemPanelCollapsed = ChangeButtonVisualCondition(IsItemPanelCollapsed, Button_CollapseItemPanel,
                                   BaseImageFolderUri, "Right-arrow.png", "left-arrow.png", 
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
        private void EssentialDesignElement_GND_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = new EssentialItemGND("E:/Programm/SimpleCircuitDesigner/ImageSource/GND.png",
                                            "GndElement",
                                            new Point(600, 300),
                                            AuxiliaryEnums.GNDDisplayType.Signal);
            item.CreateModel();
            Models.Add(item);
        }

        private void Button_CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        //                                        //
        //          Auxiliary Handlers            //
        //                                        //

        private bool ChangeButtonVisualCondition(bool condition, ToggleButton button, string folderUri, string trueImage, string falseImage, Thickness ImageMargin)
        {
            if (condition != true)
            {
                condition = true;
                SetButtonImage(folderUri + trueImage, button, ImageMargin);
            }
            else
            {
                condition = false;
                SetButtonImage(folderUri + falseImage, button, ImageMargin);
            }
            return condition;
        }
        private bool ChangeButtonVisualCondition(bool condition, ToggleButton button, string folderUri, string trueImage, string falseImage, Thickness ImageMargin, Thickness ImageAditionalMargin)
        {
            if (condition != true)
            {
                condition = true;
                SetButtonImage(folderUri + trueImage, button, ImageAditionalMargin);
            }
            else
            {
                condition = false;
                SetButtonImage(folderUri + falseImage, button, ImageMargin);
            }
            return condition;
        }
        private void SetButtonImage(string uri, ToggleButton button, Thickness ImageMargin)
        {
            var PauseImage = new Image();
            var BitmapImage = new BitmapImage(new Uri(uri));
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
                    IsWiringModeEntered = true;
                }
                else
                {
                    foreach (var endpoint in Endpoints)
                    {
                        endpoint.ChangeAccentColor(Brushes.DimGray);
                    }
                    foreach (var item in Models)
                    {
                        item.DropSelection();
                    }
                    Endpoints.Clear();
                    ResetSelection();
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
                if (IsItemSelected((ItemBaseModel)item))
                {
                    RemoveItemFromSelection((ItemBaseModel)item);
                }
                else if (!IsItemSelected((ItemBaseModel)item))
                {
                    AddItemToSelection((ItemBaseModel)item);
                }
                UpdateSelectionAvailability();
            }
            catch
            {
                throw;
            }
        }
        public void ResetSelection()
        {
            selectedPair[0] = null;
            selectedPair[1] = null;
            UpdateSelectionAvailability();
        }
        private static void UpdateSelectionAvailability()
        {
            isSelectionAvailable = selectedPair[0] == null || selectedPair[1] == null;
            //MessageBox.Show($"Item1: {selectedPair[0]},\nitem2: {selectedPair[1]}, \nAvailability: {isSelectionAvailable}", "Status");
        }
        private static bool IsItemSelected(ItemBaseModel item)
        {
            return selectedPair[0] == item || selectedPair[1] == item;
        }
        private static void RemoveItemFromSelection(ItemBaseModel item)
        {
            if (selectedPair[0] == item)
                selectedPair = new ItemBaseModel[2] { null, selectedPair[1] };
            else if (selectedPair[1] == item)
                selectedPair = new ItemBaseModel[2] { selectedPair[0], null };
        }
        private static void AddItemToSelection(ItemBaseModel item)
        {
            if (selectedPair[0] == null)
                selectedPair = new ItemBaseModel[2] { item, selectedPair[1]};
            else if (selectedPair[1] == null)
                selectedPair = new ItemBaseModel[2] { selectedPair[0], item };
        }
    }
}
