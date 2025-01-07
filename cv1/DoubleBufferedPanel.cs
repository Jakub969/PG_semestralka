using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cv1
{
    public class DoubleBufferPanel : Panel
    {
        public DoubleBufferPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | //Do not erase the background, reduce flicker
                 ControlStyles.OptimizedDoubleBuffer | //Double buffering
                 ControlStyles.UserPaint, //Use a custom redraw event to reduce flicker
                 true);
        }
    }
}
