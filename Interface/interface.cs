using System;

namespace RemotingInterface
{
	/// <summary>
	/// this interface contains all the distributed methods
	/// </summary>
	public interface IRemoteString
	{
        void TextMessage(string msg);
        void Login(string username);
        string Logout();
    }
}
