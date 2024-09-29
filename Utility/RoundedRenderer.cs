using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedRenderer : ToolStripProfessionalRenderer
{
    private readonly Color _bgColor = Color.FromArgb(24, 26, 27);
    private readonly Color _hoverColor = Color.FromArgb(38, 41, 43);

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        // Set high-quality rendering for smooth edges
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        
        // Fill the background area
        using Brush brush = new SolidBrush(_bgColor);
        e.Graphics.FillRectangle(brush, e.AffectedBounds);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        if (e.Item.Selected)
        {
            // Draw the background for selected menu items with rounded corners
            var itemBounds = new Rectangle(Point.Empty, e.Item.Size);
            itemBounds.Y -= 2; // Move the background rectangle up by 2 pixels
            itemBounds.Height += 2; // Increase height to maintain full coverage
            using Brush brush = new SolidBrush(_hoverColor);
            e.Graphics.FillRectangle(brush, itemBounds); // Fill with a solid color
        }
        else
        {
            base.OnRenderMenuItemBackground(e); // Default for non-selected
        }
    }
}