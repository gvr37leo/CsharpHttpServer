using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

namespace HttpServer {
    class Server {

        int port;
        HttpListener listener;
        //List<Action<Request, Response>> listeners;
        Dictionary<RequestType, PathFinder> PathfinderDictionary;
        HttpListenerContext context = null;

        public Server(int port) {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Prefixes.Add($"http://127.0.0.1:{port}/");
            Console.WriteLine($"listening on {port}");

            PathfinderDictionary = new Dictionary<RequestType, PathFinder>();
            PathfinderDictionary.Add(RequestType.get,new PathFinder());
            PathfinderDictionary.Add(RequestType.post, new PathFinder());
            PathfinderDictionary.Add(RequestType.put, new PathFinder());
            PathfinderDictionary.Add(RequestType.delete, new PathFinder());

        }

        public void listen(RequestType requesType,Regex regex, Action<Match, HttpListenerContext> callback) {
            PathfinderDictionary[requesType].register(regex, (match) => {
                callback(match,context);
            });
        }

        public void serveStatic(string folder) {
            listen(RequestType.get, new Regex($"^/(.+)"), (match, context) => {
                string filepath = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}{folder}/{match.Groups[1].Value}";
                if (File.Exists(filepath)) {
                    string file = File.ReadAllText(filepath);
                    byte[] bytes = Encoding.ASCII.GetBytes(file);
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
            });
        }

        public void start() {
            listener.Start();
            while (true) {
                context = listener.GetContext();
                
                RequestType requestType = 0;
                switch (context.Request.HttpMethod) {
                    case "GET":
                        requestType = RequestType.get;
                        break;
                    case "POST":
                        requestType = RequestType.post;
                        break;
                    case "PUT":
                        requestType = RequestType.put;
                        break;
                    case "DELETE":
                        requestType = RequestType.delete;
                        break;
                }
                PathfinderDictionary[requestType].trigger(context.Request.Url.AbsolutePath);
                context.Response.Close();
            }
        }
    }

    enum RequestType {get,post,put,delete}
}
