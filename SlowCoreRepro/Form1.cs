using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Slow471Repro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Text += DotNetVersionHelper.Get45PlusFromRegistry();
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            TestEventSource.Log.TestStart();
            
            SuspendLayout();

            var parentPanel = new TableLayoutPanel {AutoSize = false};
            //parentPanel.SuspendLayout();
            if (chkSuspendDrawing.Checked)
                SuspendDrawing(parentPanel);

            for (int i = 0; i < (int) numericGroupBoxes.Value; i++)
            {
                var panel = new Panel {Name = "panel" + i, AutoSize = true};
                for (int j = 0; j < (int)numericContent.Value; j++)
                {
                    var label = new Label
                    {
                        Name = "Label" + j,
                        AutoSize = true,
                        Text = "Hello World",
                        Location = new Point(10, j * 70 + 10)
                    };

                    var cb = new ComboBox
                    {
                        Name = "comboBox" + i,
                        FormattingEnabled = true,
                        AutoSize = true,
                        DataSource = new List<string> { "Hello World" },
                        Location = new Point(10, j * 70 + 30)
                    };

                    var tb = new TextBox
                    {
                        Name = "TextBox" + i,
                        AutoSize = true,
                        Text = "TextBoxContent" + i,
                        Location = new Point(10, j * 70 + 50)
                    };

                    panel.Controls.Add(label);
                    panel.Controls.Add(cb);
                    panel.Controls.Add(tb);
                }

                var column = i % 4;
                var row = i / 4;
                parentPanel.Controls.Add(panel, column, row);
            }
            firstTabPage.Controls.Add(parentPanel);
            parentPanel.AutoSize = true;
            //parentPanel.ResumeLayout();
            ResumeLayout();
            if (chkSuspendDrawing.Checked)
                ResumeDrawing(parentPanel);

            TestEventSource.Log.TestStop();
            sw.Stop();
            MessageBox.Show($"Elapsed {sw.Elapsed.TotalMilliseconds} ms", "Time", MessageBoxButtons.OK);
        }

        private void ShowPerfResultsButton_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();

            //foreach (var item in Application.PerformanceResults)
            //{
            //    string itemText = $"{item.Value.Name}\n Count: {item.Value.SampleCount} - Total: {item.Value.TotalExecutionTime} - Avr: {item.Value.AverageExecutionTime}";
            //    sb.Append($"{itemText}\n\n");
            //}
            MessageBox.Show(sb.ToString(),"Result");
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }
    }
}
