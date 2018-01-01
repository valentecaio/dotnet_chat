# .NET chat

That's a windows messenger, written in C# using .NET Remoting library, which conforms to RPC protocol.
The client has a WPF user interface, while the Server is a console app.
Dear to RPC, both client and server subprojets share a Interface dynamic library (dll), that's also a subproject.

## Requirements

By now, this project only runs on windows.
The projet was done using Microsoft Visual Studio 2017, so you need a similar tool to build the components.

## Usage

To execute a released one, you can simply go to Release subdir and download it.
Firstly, you need to open a server and launch it (see command table below).
Then, you can launch two clients and connect them to the server by choosing usernames and clicking on Connect buttons.
Now you may be able to send messages.

### Server functions

| Command | Function |
| ------ | ------ |
| START | start a server on port 12345 |
| STOP | stop a running server |
| LIST | list connected users |
| QUIT | stop server and close server manager |
| HELP | shows a help message that explains all commands |

### Client functions

* Connect to a server on given host and port
* Disconnect from server
* See online users
* Test server availability
* Send messages (always broadcast)


## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D
