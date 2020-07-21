using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateControlsCSharp
{
    internal class CustomPanel : Panel
    {
        public void RequestDestroyHandle()
        {
            base.DestroyHandle();
        }
    }
}
