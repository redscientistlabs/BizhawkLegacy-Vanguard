using System;
using System.Linq;
using System.Windows.Forms;

using BizHawk.Client.Common;
using BizHawk.Emulation.Cores.ColecoVision;

namespace BizHawk.Client.EmuHawk
{
	public partial class ColecoControllerSettings : Form
	{
		private ColecoVision.ColecoSyncSettings _syncSettings;

		public ColecoControllerSettings()
		{
			InitializeComponent();
		}

		private void ColecoControllerSettings_Load(object sender, EventArgs e)
		{
			_syncSettings = ((ColecoVision)Global.Emulator).GetSyncSettings().Clone();

			var possibleControllers = ColecoVisionControllerDeck.ValidControllerTypes.Select(t => t.Key);

			foreach (var val in possibleControllers)
			{
				Port1ComboBox.Items.Add(val);
				Port2ComboBox.Items.Add(val);
			}

			Port1ComboBox.SelectedItem = _syncSettings.Port1;
			Port2ComboBox.SelectedItem = _syncSettings.Port2;
		}

		private void OkBtn_Click(object sender, EventArgs e)
		{
			bool changed =
				_syncSettings.Port1 != Port1ComboBox.SelectedItem.ToString()
				|| _syncSettings.Port2 != Port2ComboBox.SelectedItem.ToString();

			if (changed)
			{
				_syncSettings.Port1 = Port1ComboBox.SelectedItem.ToString();
				_syncSettings.Port2 = Port2ComboBox.SelectedItem.ToString();

				GlobalWin.MainForm.PutCoreSyncSettings(_syncSettings);
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			GlobalWin.OSD.AddMessage("Controller settings aborted");
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
