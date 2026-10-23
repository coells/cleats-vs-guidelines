using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace CleatsVsGuidelines
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class GuidelinesListener : IWpfTextViewCreationListener
    {
        private const string LayerName = "CleatsVsGuidelines";
        private const double StrokeThickness = 0.2;
        private static readonly int[] Columns = { 80, 120 };

        [Export]
        [Name(LayerName)]
        [Order(Before = PredefinedAdornmentLayers.Text)]
        public AdornmentLayerDefinition Layer = null;

        public void TextViewCreated(IWpfTextView view)
        {
            var layer = view.GetAdornmentLayer(LayerName);
            var lines = Columns
                .Select(_ => new Line { Stroke = Brushes.Gray, StrokeThickness = StrokeThickness, IsHitTestVisible = false })
                .ToArray();

            foreach (var line in lines)
            {
                layer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, null, null, line, null);
            }

            view.LayoutChanged += (_, _) =>
            {
                for (int i = 0; i < Columns.Length; i++)
                {
                    double x = view.FormattedLineSource.BaseIndentation + Columns[i] * view.FormattedLineSource.ColumnWidth;
                    lines[i].X1 = x;
                    lines[i].X2 = x;
                    lines[i].Y1 = view.ViewportTop;
                    lines[i].Y2 = view.ViewportBottom;
                }
            };
        }
    }
}
