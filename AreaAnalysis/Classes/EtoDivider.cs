using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaAnalysis.Classes
{
    public class EtoDivider : Eto.Forms.Drawable
    {
        private Eto.Drawing.Color m_color;

        public Eto.Drawing.Color Color
        {
            get { return m_color; }
            set
            {
                if (m_color == value)
                    return;
                m_color = value;
                Invalidate();
            }
        }

        public Eto.Forms.Orientation Orientation => Width < Height
            ? Eto.Forms.Orientation.Vertical
            : Eto.Forms.Orientation.Horizontal;

        public EtoDivider()
        {
            m_color = Eto.Drawing.Colors.LightGrey;
            Size = new Eto.Drawing.Size(3, 3);
        }

        protected override void OnSizeChanged(System.EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

        protected override void OnLoadComplete(System.EventArgs e)
        {
            base.OnLoadComplete(e);
            Invalidate();
        }

        protected override void OnPaint(Eto.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            var middle = new Eto.Drawing.PointF(Size / 2);
            e.Graphics.FillRectangle(
                Color,
                Orientation == Eto.Forms.Orientation.Horizontal
                    ? new Eto.Drawing.RectangleF(0f, middle.Y, ClientSize.Width, 1)
                    : new Eto.Drawing.RectangleF(middle.Y, 0f, 1, ClientSize.Height));
        }
    }
}
