using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.AppContext
{
    class DrawingServiceSingleton
    {
        private static DrawingServiceSingleton instance = null;
        private static readonly object padlock = new object();

        public DrawingService DrawingService { get; set; }

        public DrawingServiceSingleton()
        {
            DrawingService = new DrawingService();
        }

        public static DrawingServiceSingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DrawingServiceSingleton();
                    }
                    return instance;
                }
            }
        }
    }
}
