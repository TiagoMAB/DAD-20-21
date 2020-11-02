using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PuppetMaster.MVC {
    public class ServerController {
        private delegate void UpdaterCombo(ComboBox box, List<string> views);
        private delegate void AdderCombo(ComboBox box, string name);
        private delegate void UpdaterList(CheckedListBox box, List<string> views);
        private delegate void AdderList(CheckedListBox box, string name);
        private readonly PuppetMaster form;
        private readonly List<ComboBox> views;
        private readonly CheckedListBox listView;
        private readonly AdderCombo adderCombo;
        private readonly UpdaterCombo updaterCombo;
        private readonly AdderList adderList;
        private readonly UpdaterList updaterList;
        private static readonly object mvcLock = new Object();

        public ServerController(PuppetMaster form, List<ComboBox> views, CheckedListBox listView) {
            this.form = form;
            this.views = views;
            this.listView = listView;
            this.adderCombo = new AdderCombo(this.form.AddToCombo);
            this.updaterCombo = new UpdaterCombo(this.form.UpdateCombo);
            this.adderList = new AdderList(this.form.AddToList);
            this.updaterList = new UpdaterList(this.form.UpdateList);
        }

        public void AddServer(string name, string URL) {
            ConnectionInfo.AddServer(name, URL);

            lock (mvcLock) {
                foreach (var view in views) {
                    this.form.Invoke(this.adderCombo, view, name);
                }

                this.form.Invoke(this.adderList, this.listView, name);
            }
        }

        public void RemoveServer(string name) {
            List<string> servers = ConnectionInfo.RemoveServer(name);

            lock (mvcLock) {
                foreach (var view in views) {
                    this.form.Invoke(this.updaterCombo, view, servers);
                }

                this.form.Invoke(this.updaterList, this.listView, servers);
            }
        }
    }
}
