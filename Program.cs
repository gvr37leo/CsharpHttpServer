using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer {
    class Program {
        static void Main(string[] args) {

            Server server = new Server(8000);
            server.listen(RequestType.get, ":object", (paramargs, context) => {
                byte[] bytes = Encoding.ASCII.GetBytes($"{paramargs[0]}");
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            });

            server.listen(RequestType.get, ":object/:id", (paramargs, context) => {
                byte[] bytes = Encoding.ASCII.GetBytes($"{paramargs[0]}:{paramargs[1]}");
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            });

            server.listen(RequestType.get, "api/:object/:id", (paramargs, context) => {
                byte[] bytes = Encoding.ASCII.GetBytes($"api:{paramargs[0]}:{paramargs[1]}");
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            });


            server.start();//enters infinite loop
        }
    }
}
