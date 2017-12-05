using System;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace remotServeur
{
	/// <summary>
	/// Description résumée de demarreServeur.
	/// </summary>
	public class Serveur : MarshalByRefObject, RemotingInterface.IRemotChaine
	{
		static void Main()
		{
			// Création d'un nouveau canal pour le transfert des données via un port 
			TcpChannel canal = new TcpChannel(12345);

			// Le canal ainsi défini doit être Enregistré dans l'annuaire
			ChannelServices.RegisterChannel(canal);

			// Démarrage du serveur en écoute sur objet en mode Singleton
			// Publication du type avec l'URI et son mode 
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(Serveur), "Serveur",  WellKnownObjectMode.Singleton);

			Console.WriteLine("Le serveur est bien démarré");
			// pour garder la main sur la console
			Console.ReadLine();	
		}

		// Pour laisser le serveur fonctionner sans time out
		public override object  InitializeLifetimeService()
		{
			return null;
		}
		

		#region Membres de IRemotChaine

		public string Hello()
		{
			// TODO : ajoutez l'implémentation de Serveur.Hello
			return "la chaine se trouvant sur le serveur" ;
		}

		#endregion
	}
}
