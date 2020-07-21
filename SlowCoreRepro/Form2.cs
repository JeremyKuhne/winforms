using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace CreateControlsCSharp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Text = $"WinForms PerfTest - {FrameworkAndOsInfo.Get45PlusFromRegistry()} on {FrameworkAndOsInfo.GetOsName()}";
        }

        private CustomPanel CreatePanelDynamic()
        {
            return new CustomPanel()
            {
                Top = 12,
                Left = 12,
                Height = 426,
                Width = 200,
                Visible = true
            };
        }

        private TextBox CreateTextBoxDynamic()
        {
            return new TextBox()
            {
                Text = "12354",
                Width = 100,
                Height = 20,
                Left = 0
            };
        }

        private long LaunchPerfTest(bool requestDestroyHandle)
        {
            var sw = Stopwatch.StartNew();
            for (int outerLoop = 0; outerLoop < 100; outerLoop++)
            {
                int top = 0;

                CustomPanel pnlCtrl = this.CreatePanelDynamic();
                Controls.Add(pnlCtrl);

                for (int innerLoop = 0; innerLoop < 50; innerLoop++)
                {
                    TextBox newTextBox = CreateTextBoxDynamic();
                    newTextBox.Top = top;
                    pnlCtrl.Controls.Add(newTextBox);
                    top += 20;
                }

                if (requestDestroyHandle)
                {
                    // ?
                    pnlCtrl.RequestDestroyHandle();
                }

                pnlCtrl.Controls.Clear();
                Controls.Remove(pnlCtrl);
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private void BtnStartPerformanceTest_Click(object sender, System.EventArgs e)
        {
            bool destroyPanelHandle = chkDestroyPanelHandle.Checked;

            try
            {
                btnStartPerformanceTest.Enabled = false;
                Cursor = Cursors.WaitCursor;
                pBarRunsDone.Value = 0;
                pBarRunsDone.Maximum = (int)nupRuns.Value;
                
                var resultTable = new List<long>();
                var result = 0L;

                for (var count = 0; count < pBarRunsDone.Maximum; count++)
                {
                    if (chkSuspenLayout.Checked)
                        this.SuspendDrawing();

                    result = LaunchPerfTest(destroyPanelHandle);
                    resultTable.Add(result);
                    dgvResults.Rows.Add($"{count + 1}", $"{result / 1000f:#,##0.000} sec.");
                    pBarRunsDone.Value = count + 1;
                    if (chkSuspenLayout.Checked)
                        this.ResumeDrawing();

                    Application.DoEvents();
                }

                lblAverageRunTime.Text = $"Average run time:{resultTable.Average() / 1000f:#,##0.000}";
            }

            catch (System.Exception)
            {
            }

            finally
            {
                btnStartPerformanceTest.Enabled = true;
                Cursor = Cursors.Arrow;
            }
        }
    }
}
