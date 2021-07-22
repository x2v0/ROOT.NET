// $Id: Form1.cs 4927 2017-01-18 16:10:16Z onuchin $

using System;
using System.Windows.Forms;


using ROOT;


namespace Hsimple
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : Form
   {
      private Button button1;
      private TCanvas fCanvas;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{

			InitializeComponent();
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
         this.button1 = new System.Windows.Forms.Button();
         this.fCanvas = new ROOT.TCanvas();
         this.SuspendLayout();
         // 
         // button1
         // 
         this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.button1.Location = new System.Drawing.Point(512, 469);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(75, 23);
         this.button1.TabIndex = 0;
         this.button1.Text = "Run";
         this.button1.UseVisualStyleBackColor = true;
         this.button1.Click += new System.EventHandler(this.OnClick);
         // 
         // fCanvas
         // 
         this.fCanvas.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
         this.fCanvas.Location = new System.Drawing.Point(34, 12);
         this.fCanvas.Name = "fCanvas";
         this.fCanvas.Size = new System.Drawing.Size(574, 407);
         this.fCanvas.TabIndex = 1;
         // 
         // Form1
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(649, 504);
         this.Controls.Add(this.fCanvas);
         this.Controls.Add(this.button1);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "Form1";
         this.Text = "hsimple example";
         this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
         Application.Run(new Form1());
		}
   
//// another way to run ROOT compiled code
//      #region Interop
//
//      [DllImport("hsimple.NET.dll", EntryPoint="hsimple", SetLastError=true)]
//      public static extern void hsimple();
//
//      #endregion Interop

      private void OnClick(object sender, EventArgs e)
      {

         // another way to run ROOT compied code
         // hsimple();

         var macro = "hsimple.C" + "(\"" + fCanvas.Name + "\")";
         TROOT.Macro(macro);
      }
	}
}
