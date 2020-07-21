// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace WinFormsPerf
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            button1.Paint += Button1_Paint;
        }

        private void Button1_Paint(object sender, PaintEventArgs e)
        {
            Action action = () => Close();
            BeginInvoke(action);
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        protected override void OnShown(EventArgs e)
        {
        }
    }
}
