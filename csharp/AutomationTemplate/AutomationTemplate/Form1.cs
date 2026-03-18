using AutomationTemplate._0_System;
using AutomationTemplate._5_Utility.BtRuntime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace AutomationTemplate
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource btCts;
        private BehaviorTree currentTree;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MSystem.GetInstance().Initialize();
            AppendLog("System initialized.");

            // default BT path suggestion (user can load another file)
            var defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "bt-tree.json");
            if (File.Exists(defaultPath))
                txtBtPath.Text = defaultPath;
            UpdateStatus("Idle");

        }

        private void btnLoadBt_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "BT JSON (*.json)|*.json|All files (*.*)|*.*";
                dialog.Title = "BT JSON 파일 선택";
                dialog.FileName = "bt-tree.json";
                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                txtBtPath.Text = dialog.FileName;
            }

            LoadBtFromPath(txtBtPath.Text);
        }

        private void btnStartBt_Click(object sender, EventArgs e)
        {
            if (btCts != null)
            {
                AppendLog("BT is already running.");
                return;
            }

            if (currentTree == null)
            {
                if (!LoadBtFromPath(txtBtPath.Text))
                    return;
            }

            btCts = new CancellationTokenSource();
            UpdateStatus("Running");

            var context = new BtContext(
                blackboard: new BtBlackboard(),
                unitRegistry: new LegacyUnitRegistry(),
                cancellationToken: btCts.Token);

            context.Trace += (s, ev) =>
            {
                if (ev.EventType == "tickResult" || ev.EventType == "timeout")
                    AppendLog($"[{ev.TimestampUtc:O}] {ev.EventType} node={ev.NodeId} type={ev.NodeType} status={ev.Status}");
            };

            // PoC default value if not provided by schema
            context.Blackboard.Set("positionIdx", 2);

            Task.Run(async () =>
            {
                try
                {
                    AppendLog($"Start BT: {currentTree.Definition.TreeId} v{currentTree.Definition.Version}");
                    var result = await currentTree.RunToCompletionAsync(context, TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
                    AppendLog("BT finished: " + result);
                    SafeUi(() => UpdateStatus("Done: " + result));
                }
                catch (OperationCanceledException)
                {
                    AppendLog("BT canceled.");
                    SafeUi(() => UpdateStatus("Canceled"));
                }
                catch (Exception ex)
                {
                    AppendLog("BT error: " + ex.Message);
                    SafeUi(() => UpdateStatus("Error"));
                }
                finally
                {
                    var cts = btCts;
                    btCts = null;
                    cts?.Dispose();
                }
            });
        }

        private void btnStopBt_Click(object sender, EventArgs e)
        {
            if (btCts == null)
            {
                AppendLog("BT is not running.");
                return;
            }

            btCts.Cancel();
        }

        private bool LoadBtFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                AppendLog("BT path is empty.");
                return false;
            }

            if (!File.Exists(path))
            {
                AppendLog("BT file not found: " + path);
                return false;
            }

            try
            {
                var def = BtDefinition.LoadFromFile(path);
                currentTree = BehaviorTree.Build(def);
                AppendLog($"Loaded BT: {def.TreeId} v{def.Version} root={def.RootNodeId}");
                UpdateStatus("Loaded");
                return true;
            }
            catch (Exception ex)
            {
                AppendLog("Failed to load BT: " + ex.Message);
                UpdateStatus("Load failed");
                currentTree = null;
                return false;
            }
        }

        private void UpdateStatus(string status)
        {
            lblBtStatus.Text = "BT 상태: " + status;
        }

        private void AppendLog(string message)
        {
            SafeUi(() =>
            {
                txtLog.AppendText(message + Environment.NewLine);
            });
        }

        private void SafeUi(Action action)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }
    }
}
