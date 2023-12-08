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
        private bool IsSimulationEntered = false;
        private bool IsItemPanelCollapsed = false;
        private string BaseImageFolderUri = @"E:\Programm\SimpleCircuitDesigner\ImageSource\";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_EnterSimulation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Button_EnterSimulation.Margin = new Thickness(0, 5, 0, 0);
        }

        private void Button_CollapseItemPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BottomGrid.Margin = new Thickness(-272, 20, 15, 15);
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
    }
}
