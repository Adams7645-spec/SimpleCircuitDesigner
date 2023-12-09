using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimpleCircuitDesigner.ItemClasses.Item
{
    internal class EssentialItemWire : ItemBaseModel
    {
        public Line? Line { get; private set; }
        private string modelName;
        private string modelImageUri;
        private Point modelCoordinates;
        private Point start;
        private Point end;
        public EssentialItemWire(string modelName, string modelImageUri, Point modelCoordinates, Point start, Point end) : base(modelName, modelImageUri, modelCoordinates)
        {
            this.modelName = modelName;
            this.modelImageUri = modelImageUri;
            this.modelCoordinates = modelCoordinates;
            this.start = start;
            this.end = end;
        }
        public override void DestructModel()
        {
            throw new NotImplementedException();
        }

        public override void CreateModel()
        {
            throw new NotImplementedException();
        }
    }
}
