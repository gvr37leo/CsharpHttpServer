using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpServer {
    class PathFinder {

        private List<PathRegistration> pathRegistrations = new List<PathRegistration>();


        public PathFinder() {

        }

        public void trigger(string path) {
            foreach (PathRegistration pathRegistration in pathRegistrations) {
                Match match = pathRegistration.regex.Match(path);
                if (match.Success) {
                    pathRegistration.callback(match);
                    return;
                }
            }
        }

        public void register(Regex regex, Action<Match> callback){
            pathRegistrations.Add(new PathRegistration(regex, callback));
        }

        class PathRegistration {
            public Action<Match> callback;

            public Regex regex;

            public PathRegistration(Regex regex, Action<Match> callback) {
                this.regex = regex;
                this.callback = callback;
            }
        }
    }

    
}
