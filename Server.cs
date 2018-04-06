using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

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

        public void listen(RequestType requesType,string path, Action<List<string>, HttpListenerContext> callback) {
            PathfinderDictionary[requesType].register(path, (args) => {
                callback(args,context);
            });
        }

        public void start() {
            
            listener.Start();
            while (true) {
                context = listener.GetContext();
                context.Response.StatusCode = 200;
                context.Request.Url.AbsolutePath.Remove(0, 1);
                string filepath = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}/public{context.Request.Url.AbsolutePath}";
                if (File.Exists(filepath)) {
                    string file = File.ReadAllText(filepath);
                    byte[] bytes = Encoding.ASCII.GetBytes(file);
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                } else {
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
                    PathfinderDictionary[requestType].trigger(context.Request.Url.AbsolutePath.Remove(0, 1));
                }
                context.Response.Close();
            }
        }
    }

    class Request {
        public Dictionary<string,string> headers;
        public RequestType requestType;
        public string body;
    }

    class Response {
        HttpListenerContext ctxt;
    }

    enum RequestType {get,post,put,delete}
}
