using System;

namespace RemotingInterface
{
	/// <summary>
	/// this interface contains all the distributed methods
	/// </summary>
	public interface IRemoteString
	{
        void TextMessage(string msg);
        string Login();
        string Logout();
    }
}
