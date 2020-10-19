using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PuppetMaster.MVC {
    public class ServerController {
        private delegate void Updater(ComboBox box, List<string> views);
        private delegate void Adder(ComboBox box, string name);
        private readonly PuppetMaster form;
        private readonly List<ComboBox> views;
        private readonly Adder adder;
        private readonly Updater updater;
        private static readonly object mvcLock = new Object();

        public ServerController(PuppetMaster form, List<ComboBox> views) {
            this.form = form;
            this.views = views;
            this.adder = new Adder(this.form.AddToCombo);
            this.updater = new Updater(this.form.UpdateCombo);
        }

        public void AddServer(string name, string URL) {
            ConnectionInfo.AddServer(name, URL);

            lock (mvcLock) {
                foreach (var view in views) {
                    this.form.Invoke(this.adder, view, name);
                }
            }
        }

        public void RemoveServer(string name) {
            List<string> servers = ConnectionInfo.RemoveServer(name);

            lock (mvcLock) {
                foreach (var view in views) {
                    this.form.Invoke(this.updater, view, servers);
                }
            }
        }
    }
}
