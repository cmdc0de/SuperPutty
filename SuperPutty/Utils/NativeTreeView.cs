using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SuperPutty.Utils
{
    public class NativeTreeView : System.Windows.Forms.TreeView
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName,
                                                string pszSubIdList);

        protected override void CreateHandle()
        {
            base.CreateHandle();

            SetWindowTheme(this.Handle, "explorer", null);
        }
    }
}
