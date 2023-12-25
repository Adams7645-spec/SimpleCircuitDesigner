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
    class EssentialItemGND : ItemBaseModel
    {
        private bool IsGrounded = false;
        public EssentialItemGND(string DeffElementImageUri,
                                Point ElementCoordinates) : base(DeffElementImageUri, ElementCoordinates)
        {
            IsGrounded = true;
            elementImageUri = DeffElementImageUri;
            locationCoordinates = ElementCoordinates;
            IsSelected = false;
            Image = new Image();
            Endpoints = new List<Endpoint>();
            InfoBorder = CreateInfoBorder(("Grounded: ", this, IsGrounded, this.GetType()));

            CreateModel(new List<Endpoint> { new Endpoint(this, new Point(36.7, 4)) });
        }
        public void SetGrouded(bool isGrounded)
        {
            IsGrounded = isGrounded;
            MessageBox.Show($"Is grounded set as: {isGrounded}");
        }
    }
}