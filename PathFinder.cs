using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer {
    class PathFinder {

        private List<PathRegistration> pathRegistrations = new List<PathRegistration>();


        public PathFinder() {

        }

        public void trigger(string path) {
            string[] fragments = path.Split('/');

            foreach (var pathRegistration in pathRegistrations) {
                if (pathRegistration.pathFragments.Count != fragments.Length) {
                    continue;
                }

                List<string> hitVariables = new List<string>();
                for (var i = 0; i < pathRegistration.pathFragments.Count; i++) {
                    var registeredFragment = pathRegistration.pathFragments[i];
                    if (registeredFragment[0] == ':') {
                        hitVariables.Add(fragments[i]);
                    } else if (registeredFragment != fragments[i]) {
                        goto continueOuterloop;
                    } else {
                        continue;
                    }
                }
                pathRegistration.callback(hitVariables);
                break;
                continueOuterloop:;
            }
        }

        public void register(string path, Action<List<string>> callback){
            pathRegistrations.Add(new PathRegistration(path.Split('/').ToList(), callback));
        }

        class PathRegistration {
            public Action<List<string>> callback;

            public List<string> pathFragments = new List<string>();

            public PathRegistration(List<string> path, Action<List<string>> callback) {
                this.pathFragments = path;
                this.callback = callback;
            }
        }
    }

    
}
