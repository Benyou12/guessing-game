using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.AppContext
{

        public sealed class AppContextSingleton
        {
            private static AppContextSingleton instance = null;
            private static readonly object padlock = new object();

            public AppContext AppContext;

            public AppContextSingleton()
            {
                AppContext = new AppContext();
            }

            public static AppContextSingleton Instance
            {
                get
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new AppContextSingleton();
                        }
                        return instance;
                    }
                }
            }
        }
}
