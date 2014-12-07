using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QuickGenerator.UI.form
{

	public delegate void uiChange();

	class ObserverChange : IDisposable
	{
		List<Control> ControlsUnderObservation;
		private bool isActive;
		public event uiChange UIChange;

		public ObserverChange(Control ctr)
		{
			if (ctr == null)
				throw new Exception("Control is null");

			ControlsUnderObservation = new List<Control>();

			if (AddHandler(ctr))
				ControlsUnderObservation.Add(ctr);
		}

		private Boolean AddHandler(Control ctr)
		{
			TextBox textBox = ctr as TextBox;

			if (textBox != null)
			{
				textBox.TextChanged += new EventHandler(ObserverChange_TextChanged);
			}

			CheckBox checkBox = ctr as CheckBox;
			if (checkBox != null)
			{
				checkBox.CheckedChanged += new EventHandler(ObserverChange_CheckedChanged);

			}

			RadioButton radioButton = ctr as RadioButton;

			if (radioButton != null)
			{
				radioButton.CheckedChanged += new EventHandler(ObserverChange_CheckedChanged);

			}

			return false;

		}


		private void RemoveHandle(Control ctr)
		{
			TextBox textBox = ctr as TextBox;

			if (textBox != null)
			{
				textBox.TextChanged -= new EventHandler(ObserverChange_TextChanged);
			}

			CheckBox checkBox = ctr as CheckBox;
			if (checkBox != null)
			{
				checkBox.CheckedChanged -= new EventHandler(ObserverChange_CheckedChanged);

			}

			RadioButton radioButton = ctr as RadioButton;

			if (radioButton != null)
			{
				radioButton.CheckedChanged -= new EventHandler(ObserverChange_CheckedChanged);

			}


		}

		public void RemoveAllHandle()
		{
			if (isActive == false) return;

			foreach (Control item in ControlsUnderObservation)
			{
				RemoveHandle(item);
			}

			isActive = false;
		}

		/// <summary>
		/// Active listen
		/// </summary>
		public void AddAllHandle()
		{
			if (isActive) return;

			foreach (Control item in ControlsUnderObservation)
			{
				AddHandler(item);
			}

			isActive = true;
		}

		void ObserverChange_CheckedChanged(object sender, EventArgs e)
		{
			Notify();
		}

		void ObserverChange_TextChanged(object sender, EventArgs e)
		{
			Notify();
		}


		private void Notify()
		{
			RemoveAllHandle();
			if (UIChange != null)
				UIChange();
		}

		/// <summary>
		/// Put in observation the control if change
		/// </summary>
		/// <param name="ctr"></param>
		public void PutUnderObservation(Control ctr)
		{
			if (AddHandler(ctr))
				ControlsUnderObservation.Add(ctr);


		}




		#region IDisposable Membri di

		public void Dispose()
		{

			foreach (Control item in ControlsUnderObservation)
			{

				RemoveHandle(item);

			}

			ControlsUnderObservation.Clear();
			ControlsUnderObservation = null;
		}

		#endregion
	}
}
