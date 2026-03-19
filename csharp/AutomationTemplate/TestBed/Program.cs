using System;
using System.Windows.Forms;
using AutomationTemplate._0_System;

namespace TestBed
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var profile = TestBedEquipmentProfile.LoadFromConfigDirectory(AppDomain.CurrentDomain.BaseDirectory);
            MSystem.GetInstance().Initialize(new TestBedEquipmentModule(profile));
            Application.Run(new TestBedMainForm(profile));
        }
    }
}
