using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;


namespace Client
{
	/// <summary>
	/// </summary>
	public class Client : System.Windows.Forms.Form
	{
			
		private System.Windows.Forms.TextBox zone;
		private System.Windows.Forms.Button bt;
		private System.ComponentModel.Container components = null;

		private RemotingInterface.IRemoteString strRemote ;


		public Client()
		{
			InitializeComponent();

			// create a receptor TCP channel
			TcpChannel canal = new TcpChannel();

			// register channnel
			ChannelServices.RegisterChannel(canal);

			// get server objet reference
			// needs: server URL (tcp://<server ip>:<server port>/<server class>) and interface name
			this.strRemote = (RemotingInterface.IRemoteString)Activator.GetObject(
				typeof (RemotingInterface.IRemoteString), "tcp://localhost:12345/Server");
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Code généré par le Concepteur Windows Form
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
            this.zone = new System.Windows.Forms.TextBox();
            this.bt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // zone
            // 
            this.zone.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zone.Location = new System.Drawing.Point(12, 418);
            this.zone.Multiline = true;
            this.zone.Name = "zone";
            this.zone.Size = new System.Drawing.Size(440, 57);
            this.zone.TabIndex = 0;
            // 
            // bt
            // 
            this.bt.Location = new System.Drawing.Point(458, 418);
            this.bt.Name = "bt";
            this.bt.Size = new System.Drawing.Size(68, 57);
            this.bt.TabIndex = 1;
            this.bt.Text = "Send";
            this.bt.Click += new System.EventHandler(this.bt_Click);
            // 
            // Client
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(845, 487);
            this.Controls.Add(this.bt);
            this.Controls.Add(this.zone);
            this.Name = "Client";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
        
		[STAThread]
		/*static void Main() 
		{
			Application.Run(new Form1());
		}
          */

		private void bt_Click(object sender, System.EventArgs e)
		{
			// launch remote method
			zone.Text = this.strRemote.Hello() ;
		}
	}
}
