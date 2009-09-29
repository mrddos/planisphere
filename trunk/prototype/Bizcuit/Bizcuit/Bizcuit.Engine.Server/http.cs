/**
 * This software is provided as is.  Do not attempt to use this as your real web server.
 * It surely has bugs in it that would compromise your system.  It is a demonstration only.
 * 
 * That said, it is completely free for modification, distribution, and redistribution.
 * If you make any changes that you think will help the community of users that are interested
 * in this sort of thing, I would love to hear about it.
 * 
 * By default it runs on port 8001.  If you have a server on that port it will very ungracefully
 * fail to start.
 * 
 * Another interesting example based off this would use the async IO APIs instead.  It would also
 * be fairly trivial to extend it with a configuration file to handle virtual hosting.
 * 
 * This program must be assembled with System.Net.dll and System.IO.dll.
 * 
 * History:
 * 8/2000		Wrote original code
 * 7/2001		Updated with keep-alive, changed the port, fixed some bugs, updated to work only with beta 2
 * 
 * Source URL: http://www.sampullara.com/http.cs
 * EXE URL:    http://www.sampullara.com/httpd.exe
 * 
 * (c) 2001 Sam Pullara  sam@sampullara.com
 */

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Bizcuit.Engine;
using Bizcuit.Engine.Server;
using System.Text;
using Bizcuit.Common;

namespace com.sampullara
{
	class HttpProcessor
	{

		private static int threads = 0;
		private Socket s;
		private NetworkStream ns;
		private StreamReader sr;
		private StreamWriter sw;
		private string method;
		private string url;
		private string protocol;
		private Hashtable headers;
		private string request;
		private bool keepAlive = false;
		private int numRequests = 0;
		private bool verbose = HttpServer.verbose;
		private byte[] bytes = new byte[4096];
		private FileInfo docRootFile;

		/**
		 * Each HTTP processor object handles one client.  If Keep-Alive is enabled then this
		 * object will be reused for subsequent requests until the client breaks keep-alive.
		 * This usually happens when it times out.  Because this could easily lead to a DoS
		 * attack, we keep track of the number of open processors and only allow 100 to be
		 * persistent active at any one time.  Additionally, we do not allow more than 500
		 * outstanding requests.
		 */

		public HttpProcessor(string docRoot, Socket s)
		{
			this.s = s;
			docRootFile = new FileInfo(docRoot);
			headers = new Hashtable();
		}


		/**
		 * This is the main method of each thread of HTTP processing.  We pass this method
		 * to the thread constructor when starting a new connection.
		 */
		public void process()
		{
			try
			{
				// Increment the number of current connections
				Interlocked.Increment(ref threads);
				// Bundle up our sockets nice and tight in various streams
				ns = new NetworkStream(s, FileAccess.ReadWrite);
				// It looks like these streams buffer
				sr = new StreamReader(ns);
				sw = new StreamWriter(ns);
				// Parse the request, if that succeeds, read the headers, if that
				// succeeds, then write the given URL to the stream, if possible.
				while (parseRequest())
				{
					if (readHeaders())
					{
						// This makes sure we don't have too many persistent connections and also
						// checks to see if the client can maintain keep-alive, if so then we will
						// keep this http processor around to process again.
						if (threads <= 100 && "Keep-Alive".Equals(headers["Connection"]))
						{
							keepAlive = true;
						}
						// Copy the file to the socket
						writeURL();
						// If keep alive is not active then we want to close down the streams
						// and shutdown the socket
						if (!keepAlive)
						{
							ns.Close();
							s.Shutdown(SocketShutdown.Both);
							break;
						}
					}
				}
			}
			finally
			{
				// Always decrement the number of connections
				Interlocked.Decrement(ref threads);
			}
		}

		public bool parseRequest()
		{
			// The number of requests handled by this persistent connection
			numRequests++;
			
			// We should use this, instead of socket...Healer
			//HttpListener listener = new HttpListener();
			
			
			// Here is where we ensure that we are not overloaded
			if (threads > 500)
			{
				writeError(502, "Server temporarily overloaded");
				return false;
			}
			// FIXME: This could conceivably used to DoS us if we never finish reading the
			// line and they never hang up.  We could set the socket options to limit
			// the amount of time before reading a request.
			try
			{
				request = null;
				request = sr.ReadLine();
			}
			catch (IOException)
			{
			}
			// If the request line is null, then the other end has hung up on us.  A well
			// behaved client will do this after 15-60 seconds of inactivity.
			if (request == null)
			{
				if (verbose)
				{
					Console.WriteLine("Keep-alive broken after " + numRequests + " requests");
				}
				return false;
			}
			// HTTP request lines are of the form:
			// [METHOD] [Encoded URL] HTTP/1.?
			string[] tokens = request.Split(new char[] { ' ' });
			if (tokens.Length != 3)
			{
				writeError(400, "Bad request");
				return false;
			}
			// We currently only handle GET requests
			method = tokens[0];
			if (!method.Equals("GET"))
			{
				writeError(501, method + " not implemented");
				return false;
			}
			url = tokens[1];
			// Only accept valid urls
			if (!url.StartsWith("/"))
			{
				writeError(400, "Bad URL");
				return false;
			}
			// Decode all encoded parts of the URL using the built in URI processing class
			int i = 0;
			while ((i = url.IndexOf("%", i)) != -1)
			{
				url = url.Substring(0, i) + Uri.HexUnescape(url, ref i) + url.Substring(i);
			}
			// Lets just make sure we are using HTTP, thats about all I care about
			protocol = tokens[2];
			if (!protocol.StartsWith("HTTP/"))
			{
				writeError(400, "Bad protocol: " + protocol);
			}
			return true;
		}

		public bool readHeaders()
		{
			string line;
			string name = null;
			// The headers end with either a socket close (!) or an empty line
			while ((line = sr.ReadLine()) != null && line != "")
			{
				// If the value begins with a space or a hard tab then this
				// is an extension of the value of the previous header and
				// should be appended
				if (name != null && Char.IsWhiteSpace(line[0]))
				{
					headers[name] += line;
					continue;
				}
				// Headers consist of [NAME]: [VALUE] + possible extension lines
				int firstColon = line.IndexOf(":");
				if (firstColon != -1)
				{
					name = line.Substring(0, firstColon);
					String value = line.Substring(firstColon + 1).Trim();
					if (verbose) Console.WriteLine(name + ": " + value);
					headers[name] = value;
				}
				else
				{
					writeError(400, "Bad header: " + line);
					return false;
				}
			}
			return line != null;
		}

		/**
		 * We need to make sure that the url that we are trying to treat as a file
		 * lies below the document root of the http server so that people can't grab
		 * random files off your computer while this is running.
		 */
		public void writeURL()
		{
			try
			{
				// Replace the forward slashes with back-slashes to make a file name
				string filename = url.Replace('/', '\\');
				
				// Add by Healer.kx
				if (url.StartsWith("/bizcuit/"))
				{
					if (url.StartsWith("/bizcuit/actions/"))
					{
						// Goes for bizcuit actions.
						int p = url.IndexOf('?', 17);
						if (p > 0)
						{
							string actionCommand = url.Substring(17, p - 17);

							// TODO: use action command to call actions
							// TODO: 
							

							IBizActionRequest req = new BizActionRequest();
							IBizActionResponse resp = new BizActionResponse();

							req.ActionCommnad = actionCommand;
							HttpServer.GetActionEngine().ProcessAction(req, resp);

							string content = resp.Content;
							
							byte[] bs = Encoding.Default.GetBytes(content);
							ns.Write(bs, 0, bs.Length);

							return;
						}

					}
				}
				
				// Construct a filename from the doc root and the filename
				FileInfo file = new FileInfo(docRootFile + filename);
				// Make sure they aren't trying in funny business by checking that the
				// resulting canonical name of the file has the doc root as a subset.
				filename = file.FullName;
				if (!filename.StartsWith(docRootFile.FullName))
				{
					writeForbidden();
				}
				else
				{
					// Open the file
					FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
					// Write the content length and the success header to the stream
					long left = file.Length;
					writeSuccess(left);
					// Copy the contents of the file to the stream, ensure that we never write
					// more than the content length we specified.  Just in case the file somehow
					// changes out from under us, although I don't know if that is possible.
					BufferedStream bs = new BufferedStream(fs);
					int read;
					while (left > 0 && (read = bs.Read(bytes, 0, (int)Math.Min(left, bytes.Length))) != 0)
					{
						ns.Write(bytes, 0, read);
						left -= read;
					}
					ns.Flush();
					bs.Close();
				}
			}
			catch (FileNotFoundException)
			{
				writeFailure();
			}
		}

		/**
		 * These write out the various HTTP responses that are possible with this
		 * very simple web server.
		 */

		public void writeSuccess(long length)
		{
			writeResult(200, "OK", length);
		}

		public void writeFailure()
		{
			writeError(404, "File not found");
		}

		public void writeForbidden()
		{
			writeError(403, "Forbidden");
		}

		public void writeError(int status, string message)
		{
			string output = "<h1>HTTP/1.0 " + status + " " + message + "</h1>";
			writeResult(status, message, (long)output.Length);
			sw.Write(output);
			sw.Flush();
		}

		public void writeResult(int status, string message, long length)
		{
			if (verbose) Console.WriteLine(request + " " + status + " " + numRequests);
			sw.Write("HTTP/1.0 " + status + " " + message + "\r\n");
			sw.Write("Content-Length: " + length + "\r\n");
			if (keepAlive)
			{
				sw.Write("Connection: Keep-Alive\r\n");
			}
			else
			{
				sw.Write("Connection: close\r\n");
			}
			sw.Write("\r\n");
			sw.Flush();
		}
	}

	public class HttpServer
	{

		// ============================================================
		// Data

		public static bool verbose = false;
		private int port;
		private string docRoot;

		private static BizActionEngine actionEngine = new BizActionEngine();

		public static BizActionEngine GetActionEngine()
		{
			return actionEngine;
		}

		// ============================================================
		// Constructor

		public HttpServer(string docRoot, int port)
		{
			this.docRoot = docRoot;
			this.port = port;

			// Added by Healer.kx
			
			actionEngine.Initialize();

		}

		// ============================================================
		// Listener

		public void listen()
		{
			// Create a new server socket, set up all the endpoints, bind the socket and then listen
			Socket listener = new Socket(0, SocketType.Stream, ProtocolType.Tcp);
			IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
			IPEndPoint endpoint = new IPEndPoint(ipaddress, port);
			listener.Bind(endpoint);
			listener.Blocking = true;
			listener.Listen(-1);
			Console.WriteLine("Http server listening on port " + port);
			while (true)
			{
				try
				{
					// Accept a new connection from the net, blocking till one comes in
					Socket s = listener.Accept();
					// Create a new processor for this request
					HttpProcessor processor = new HttpProcessor(docRoot, s);
					// Dispatch that processor in its own thread
					Thread thread = new Thread(new ThreadStart(processor.process));
					thread.Start();
				}
				catch (NullReferenceException)
				{
					// Don't even ask me why they throw this exception when this happens
					Console.WriteLine("Accept failed.  Another process might be bound to port " + port);
				}
			}
		}

		// ============================================================
		// Main

		private static void usage()
		{
			Console.WriteLine("Usage: httpd [-port port] [-docroot path] [-verbose] [/?]");
		}


		// Process all the command line parameters, create the listener, and dispatch it. We
		// could have just started listening in the main method, but its a little cleaner like this.
		public static int Main(String[] args)
		{
			HttpServer httpServer;
			int port = 8001;
			string docRoot = "web/root/";
			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "/?":
						usage();
						return 1;
					case "-port":
						if (i < args.Length - 1)
						{
							port = int.Parse(args[++i]);
						}
						else
						{
							usage();
						}
						break;
					case "-docroot":
						if (i < args.Length - 1)
						{
							docRoot = args[++i];
						}
						else
						{
							usage();
						}
						break;
					case "-verbose":
						verbose = true;
						break;
					default:
						usage();
						return 1;
				}
			}
			httpServer = new HttpServer(docRoot, port);
			Thread thread = new Thread(new ThreadStart(httpServer.listen));
			thread.Start();
			return 0;
		}
	}
}

