using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Forms;

using VortexGUI;

namespace VortexGUI
{
	class Window
	{
		//Close console window
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeConsole();

		[STAThread]
		public static void Main ()
		{
			FreeConsole ();
			Application.EnableVisualStyles();

			// ---- Icon ---- //
			var iconpath = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("icon");
			Icon icon = new Icon (iconpath);

			// ---- GUI ---- //

			//window
			Form form = new Form();
			form.Text = "VORTEX";
			form.Width = 512;
			form.Height = 350;
			form.MaximizeBox = false;
			form.MinimizeBox = false;
			form.FormBorderStyle = FormBorderStyle.FixedSingle;
			form.Icon = icon;

			//pack name
			TextBox namefield = new TextBox ();
			namefield.Text = "Name";
			namefield.Width = 128;
			namefield.Height = 20;
			namefield.TabIndex = 1;
			namefield.Name = "namefield";

			//start button
			Button startbutton = new Button ();
			startbutton.Text = "Create Pack";
			startbutton.Width = 112;
			startbutton.Height = 20;
			startbutton.Location = new Point (128, 0);
			startbutton.TabIndex = 5;
			startbutton.Click += new EventHandler (startbutton_Click);

			//description
			TextBox descriptionfield = new TextBox ();
			descriptionfield.Text = "Description";
			descriptionfield.Multiline = true;
			descriptionfield.Height = 80;
			descriptionfield.Width = 240;
			descriptionfield.Location = new Point (0, 20);
			descriptionfield.TabIndex = 2;
			descriptionfield.Name = "descriptionfield";

			//log
			TextBox outputfield = new TextBox ();
			outputfield.Multiline = true;
			outputfield.Width = 240;
			outputfield.Height = 192;
			outputfield.Location = new Point (0, 152);
			outputfield.ReadOnly = true;
			outputfield.Name = "outputfield";

			var versionlist = new VersionList ();

			//minecraft version
			ComboBox versionbox = new ComboBox ();
			versionbox.Width = 240;
			versionbox.Height = 32;
			versionbox.Location = new Point (0, 100);
			versionbox.DropDownWidth = 256;
			versionbox.DropDownStyle = ComboBoxStyle.DropDownList;
			versionbox.Name = "versionbox";
			foreach (var v in versionlist.versions) {
				versionbox.Items.Add (v);
			}
			versionbox.SelectedIndex = 0;

			//.minecraft subfolder
			TextBox subfolder = new TextBox ();
			subfolder.Height = 20;
			subfolder.Width = 144;
			subfolder.Location = new Point (96, 120);
			subfolder.Name = "subfolder";

			Label subfolder_label = new Label ();
			subfolder_label.Text = "Profile subfolder:";
			subfolder_label.Location = new Point (0, 124);

			//vertical separator
			Label vert_sep = new Label ();
			vert_sep.AutoSize = false;
			vert_sep.Height = 305;
			vert_sep.Width = 2;
			vert_sep.BorderStyle = BorderStyle.Fixed3D;
			vert_sep.Location = new Point (243, 5);

			//pack.png selection
			Label preview_heading = new Label();
			preview_heading.Text = "Select pack thumbnail";
			preview_heading.Location = new Point (306, 16);
			preview_heading.Width = 128;
			preview_heading.TextAlign = ContentAlignment.MiddleCenter;

			PictureBox preview_thumbnail = new PictureBox();
			preview_thumbnail.Name = "preview_thumbnail";
			preview_thumbnail.Width = 128;
			preview_thumbnail.Height = 128;
			preview_thumbnail.Location = new Point(306, 48);
			preview_thumbnail.BorderStyle = BorderStyle.FixedSingle;
			preview_thumbnail.SizeMode = PictureBoxSizeMode.StretchImage;

			ContextMenu preview_context = new ContextMenu ();
			MenuItem load_image = new MenuItem ();
			load_image.Text = "Load image";
			load_image.Click += new EventHandler (load_image_Click);

			MenuItem clear_image = new MenuItem ();
			clear_image.Text = "Clear image";
			clear_image.Click += new EventHandler (clear_image_Click);

			preview_context.MenuItems.Add (load_image);
			preview_context.MenuItems.Add (clear_image);
			preview_thumbnail.ContextMenu = preview_context;

			TextBox preview_source = new TextBox ();
			preview_source.Name = "preview_source";
			preview_source.Width = 192;
			preview_source.Location = new Point (274, 192);
			preview_source.TextChanged += new EventHandler (preview_source_Change);
			preview_source.MouseDoubleClick += new MouseEventHandler (preview_source_Click);

			//tooltip
			ToolTip tooltip1 = new ToolTip ();
			tooltip1.AutoPopDelay = 7500;
			tooltip1.InitialDelay = 1500;
			tooltip1.ReshowDelay = 500;
			tooltip1.ShowAlways = true;

			tooltip1.SetToolTip (namefield, "Enter the name of your resource pack\n(Required)");
			tooltip1.SetToolTip (descriptionfield, "Enter a description\n(Optional)");
			tooltip1.SetToolTip (versionbox, "Select which Minecraft version RPCMC will use");
			tooltip1.SetToolTip (subfolder, "Advanced:\nIf your launcher profiles use subfolders, enter the desired folder here\n(Optional)");
			tooltip1.SetToolTip (preview_thumbnail, "Select a thumbnail image\n(Optional)");
			tooltip1.SetToolTip (preview_source, "Type to select image\nDouble-click to open file window");


			form.Controls.AddRange (new Control[] {
				namefield,
				startbutton,
				descriptionfield,
				outputfield,
				versionbox,
				subfolder,
				subfolder_label,
				vert_sep,
				preview_heading,
				preview_thumbnail,
				preview_source
			});

			form.Show();

			// ---- Run ---- //
			Application.Run(form);
		}



		public static void preview_source_Change(object sender, EventArgs eventArgs)
		{
			TextBox preview_source = sender as TextBox;
			Form form = preview_source.FindForm () as Form;
			PictureBox preview_thumbnail = form.Controls ["preview_thumbnail"] as PictureBox;
			preview_thumbnail.ImageLocation = preview_source.Text;
		}

		public static void preview_source_Click(object sender, EventArgs eventArgs)
		{
			TextBox preview_source = sender as TextBox;
			Form form = preview_source.FindForm () as Form;
			PictureBox preview_thumbnail = form.Controls ["preview_thumbnail"] as PictureBox;

			OpenFileDialog select_image = new OpenFileDialog ();
			select_image.Title = "Select pack thumbnail";
			select_image.InitialDirectory = System.Environment.GetEnvironmentVariable ("AppData") + @"\.minecraft";

			if (select_image.ShowDialog () == DialogResult.OK) {
				preview_thumbnail.ImageLocation = select_image.FileName;
			}
		}

		public static void load_image_Click(object sender, EventArgs eventArgs)
		{
			MenuItem load_image = sender as MenuItem;
			var context = load_image.Parent as ContextMenu;
			PictureBox preview_thumbnail = context.SourceControl as PictureBox;

			OpenFileDialog select_image = new OpenFileDialog ();
			select_image.Title = "Select pack thumbnail";
			select_image.InitialDirectory = System.Environment.GetEnvironmentVariable ("AppData") + @"\.minecraft";

			if (select_image.ShowDialog () == DialogResult.OK) {
				preview_thumbnail.ImageLocation = select_image.FileName;
				Form form = preview_thumbnail.FindForm () as Form;
				TextBox preview_source = form.Controls ["preview_source"] as TextBox;
				preview_source.Text = select_image.FileName;
			}
		}

		public static void clear_image_Click(object sender, EventArgs eventArgs)
		{
			MenuItem clear_image = sender as MenuItem;
			var context = clear_image.Parent as ContextMenu;
			PictureBox preview_thumbnail = context.SourceControl as PictureBox;
			preview_thumbnail.Image = null;

			Form form = preview_thumbnail.FindForm () as Form;
			TextBox preview_source = form.Controls ["preview_source"] as TextBox;
			preview_source.Text = "";
		}

		public static void startbutton_Click(object sender, EventArgs eventArgs)
		{
			// ---- Get all necessary GUI items ---- //
			Button button = sender as Button;
			Form form = button.FindForm () as Form;
			TextBox outputfield = form.Controls ["outputfield"] as TextBox;
			TextBox namefield = form.Controls ["namefield"] as TextBox;
			TextBox descriptionfield = form.Controls ["descriptionfield"] as TextBox;
			ComboBox packformat = form.Controls ["packformat"]as ComboBox;
			ComboBox versionbox = form.Controls ["versionbox"] as ComboBox;
			TextBox subfolder = form.Controls ["subfolder"] as TextBox;
			TextBox preview_source = form.Controls ["preview_source"] as TextBox;

			// ---- pack_format, name, description, version ---- //

			var name = namefield.Text;
			var description = descriptionfield.Text;

			if (description == "Description") {
				description = "";
			}

			var jarversion = "";
			try {
				jarversion = versionbox.SelectedItem.ToString();
				jarversion = jarversion + @"\" + jarversion;
			} catch (NullReferenceException) {
				jarversion = @"1.8\1.8";
			}

			//determine pack format depending on selected version
			var version_regex = new Regex (@"1\.(\d{1,2})");
			var version_match = version_regex.Match (jarversion);
			var version_format = version_match.Groups [1].Value;
			var version_format_number = Int32.Parse (version_format);

			var pack_format = "3";
			var version = "1.11.x +";

			if (version_format_number < 9) {
				pack_format = "1";
				version = "1.6.x - 1.8.x";
			} else if (version_format_number >= 9 && version_format_number <= 10 ) {
				pack_format = "2";
				version = "1.9.x - 1.10.x";
			}


			// ---- Remove invalid chars from name ---- //
			string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

			foreach (char c in invalid)
			{
				name = name.Replace(c.ToString(), "");
			}

			// ---- summary ---- //
			output (Environment.NewLine + Environment.NewLine + "-- Summary --" + Environment.NewLine + "Name: " + name + Environment.NewLine + "Description: " + Environment.NewLine + description + Environment.NewLine + "Version: " + version, outputfield);

			// ---- Create pack ---- //
			new MCRPC (name, description, pack_format, outputfield, jarversion, subfolder.Text, preview_source.Text);

		}

		// ---- Output function, nothing more than a substitute for Control.Text += text ---- //
		public static void output(string text, TextBox output)
		{
			output.Text += text;
		}
	}
}
