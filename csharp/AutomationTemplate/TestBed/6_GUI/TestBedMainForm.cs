using System;
using System.Windows.Forms;
using AutomationTemplate._0_System;
using TestBed._3_Control;

namespace TestBed
{
    public class TestBedMainForm : Form
    {
        private readonly TextBox logBox;
        private readonly TestBedEquipmentProfile profile;

        public TestBedMainForm(TestBedEquipmentProfile profile)
        {
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile));
            Text = "TestBed - Equipment UI";
            Width = 900;
            Height = 600;

            var btnOpenCommonUi = new Button
            {
                Text = "Open Common BT UI",
                Left = 12,
                Top = 12,
                Width = 180
            };
            btnOpenCommonUi.Click += (s, e) =>
            {
                var common = new AutomationTemplate.MainForm();
                common.Show(this);
                AppendLog("Opened common BT UI.");
            };

            var btnMoveSafePos = new Button
            {
                Text = $"Handler MoveSafePos({this.profile.Ui.SafePositionIndex})",
                Left = 210,
                Top = 12,
                Width = 180
            };
            btnMoveSafePos.Click += (s, e) =>
            {
                try
                {
                    var control = MSystem.GetInstance().Resolve<CHandler>();
                    control.MoveSafePos(this.profile.Ui.SafePositionIndex);
                    AppendLog($"CHandler.MoveSafePos({this.profile.Ui.SafePositionIndex}) executed.");
                }
                catch (Exception ex)
                {
                    AppendLog("MoveSafePos failed: " + ex.Message);
                }
            };

            logBox = new TextBox
            {
                Left = 12,
                Top = 52,
                Width = 860,
                Height = 500,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            Controls.Add(btnOpenCommonUi);
            Controls.Add(btnMoveSafePos);
            Controls.Add(logBox);
        }

        private void AppendLog(string message)
        {
            logBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }
    }
}
