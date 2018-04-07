using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpServer {
    class Program {
        static void Main(string[] args) {

            Server server = new Server(8000);

            server.listen(RequestType.get, new Regex("^/api/(.+)/(.+)$"), (match,context) => {
                
            });

            server.listen(RequestType.get, new Regex("^/api/(.+)$"), (match, context) => {

            });

            server.serveStatic("./public");


            server.start();//enters infinite loop
        }
    }
}
