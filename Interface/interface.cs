using System;

namespace RemotingInterface
{
	/// <summary>
	/// this interface contains all the distributed methods
	/// </summary>
	public interface IRemoteString
	{
        string TextMessage();
        string Login();
        string Logout();
    }
}
