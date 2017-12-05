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
	/// Description résumée de Form1.
	/// </summary>
	public class Client : System.Windows.Forms.Form
	{
			
		private System.Windows.Forms.TextBox zone;
		private System.Windows.Forms.Button bt;
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private RemotingInterface.IRemotChaine LeRemot ;


		public Client()
		{
			InitializeComponent();

			// création d'un canal recepteur TCP
			TcpChannel canal = new TcpChannel();

			// enregistrement du canal
			ChannelServices.RegisterChannel(canal);

			// l'ojet LeRemot  récupére ici la référence de l'objet du serveur
			// on donne l'URI (serveur, port, classe du serveur)  et le nom de l'interface
			LeRemot = (RemotingInterface.IRemotChaine)Activator.GetObject(
				typeof (RemotingInterface.IRemotChaine), "tcp://localhost:12345/Serveur");
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
            this.zone.Location = new System.Drawing.Point(24, 24);
            this.zone.Name = "zone";
            this.zone.Size = new System.Drawing.Size(440, 31);
            this.zone.TabIndex = 0;
            // 
            // bt
            // 
            this.bt.Location = new System.Drawing.Point(184, 72);
            this.bt.Name = "bt";
            this.bt.Size = new System.Drawing.Size(104, 40);
            this.bt.TabIndex = 1;
            this.bt.Text = "Lancer";
            this.bt.Click += new System.EventHandler(this.bt_Click);
            // 
            // Client
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(488, 142);
            this.Controls.Add(this.bt);
            this.Controls.Add(this.zone);
            this.Name = "Client";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Point d'entrée principal de l'application.
		/// </summary>
		[STAThread]
		/*static void Main() 
		{
			Application.Run(new Form1());
		}
          */

		private void bt_Click(object sender, System.EventArgs e)
		{
			// on lance la méthode remote
			zone.Text = LeRemot.Hello() ;
		}
	}
}
