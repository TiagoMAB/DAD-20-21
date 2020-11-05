using Grpc.Core;
using PuppetMaster.Commands;
using PuppetMaster.Exceptions;
using PuppetMaster.MVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster {
    public partial class PuppetMaster : Form {
        const int PORT = 10001;
        private readonly Grpc.Core.Server server;
        private readonly ServerController controller;
        private readonly object lockLog = new Object();
        private readonly List<string> partitionServers = new List<string>();

        public PuppetMaster() {
            InitializeComponent();

            this.server = new Grpc.Core.Server {
                Services = { GStore.Status.BindService(new StatusImpl(this)) },
                Ports = { new ServerPort("0.0.0.0", PORT, ServerCredentials.Insecure) },
            };

            server.Start();

            this.Log("Status service started successfully");

            List<ComboBox> views = new List<ComboBox> {
                crashSelector,
                freezeSelector,
                unfreezeSelector
            };

            this.controller = new ServerController(this, views, serverList);
        }

        private void SelectPMFileClick(object sender, EventArgs e) {
            SelectPMFile.ShowDialog();
        }

        private async void LoadPMFileClick(object sender, EventArgs e) {
            string path = PMFile.Text.Trim();

            if (path.Equals("")) {
                MessageBox.Show("Path cannot be empty", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else if (!File.Exists(path)) {
                MessageBox.Show(String.Format("File \"{0}\" does not exist", path), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try {
                await Task.Run(async () => await Parser.parseScript(this, path).Execute().ConfigureAwait(false));
            } catch (InvalidURLException ex) {
                MessageBox.Show(String.Format("Invalid address '{0}' on command '{1}'", ex.Url, ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (PartitionParameterNumberMismatchException ex) {
                MessageBox.Show(String.Format("Partition command expected {0} server ids, but {1} were given", ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (UnknownCommandException ex) {
                MessageBox.Show(String.Format("Unknown command '{0}' in given file", ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (UnknownServerException ex) {
                MessageBox.Show(String.Format("Unknown server '{0}' in command '{1}'", ex.Id, ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (WrongArgumentNumberException ex) {
                MessageBox.Show(String.Format("Command '{0}' is executed with {1} parameters, but {2} were given", ex.Command, ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void SelectedPMFile(object sender, System.ComponentModel.CancelEventArgs e) {
            PMFile.Text = SelectPMFile.FileName;
        }

        public void AddServer(string name, string URL) {
            this.controller.AddServer(name, URL);
        }

        public void RemoveServer(string name) {
            this.controller.RemoveServer(name);
        }

        public void RemoveConnection(string name) {
            this.controller.RemoveConnection(name);
        }

        public void Log(string entry) {
            string multiple = String.Format("[{0}] {1}", DateTime.Now.ToString(), entry);
            lock (this.lockLog) {
                foreach (string s in Regex.Split(multiple, "\n")) {
                    Logs.Items.Add(s);
                }
            }

            Logs.SelectedIndex = Logs.Items.Count - 1;
        }

        public void AddToCombo(ComboBox box, string item) {
            box.Items.Add(item);
        }

        public void UpdateCombo(ComboBox box, List<string> items) {
            box.Items.Clear();
            box.SelectedIndex = -1;
            box.Items.AddRange(items.ToArray());
        }

        public void AddToList(CheckedListBox box, string item) {
            box.Items.Add(item);
        }

        public void UpdateList(CheckedListBox box, List<string> items) {
            box.Items.Clear();
            box.SelectedIndex = -1;
            box.Items.AddRange(items.ToArray());

            lock (this.partitionServers) {
                this.partitionServers.Clear();
            }
        }

        private async void PM_Closing(object sender, FormClosingEventArgs e) {
            await this.server.ShutdownAsync();
        }

        private async Task CommandHandler(Command command) {
            try {
                await Task.Run(async () => await command.Execute().ConfigureAwait(false));
            } catch (InvalidURLException ex) {
                MessageBox.Show(String.Format("Invalid address '{0}' on command '{1}'", ex.Url, ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (PartitionParameterNumberMismatchException ex) {
                MessageBox.Show(String.Format("Partition command expected {0} server ids, but {1} were given", ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (UnknownServerException ex) {
                MessageBox.Show(String.Format("Unknown server '{0}' in command '{1}'", ex.Id, ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (WrongArgumentNumberException ex) {
                MessageBox.Show(String.Format("Command '{0}' is executed with {1} parameters, but {2} were given", ex.Command, ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private async void CrashBtn_Click(object sender, EventArgs e) {
            if (crashSelector.SelectedIndex == -1) {
                MessageBox.Show("Select a server to crash", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Command command = new Crash(this, crashSelector.Text);
            await CommandHandler(command);

            crashSelector.SelectedIndex = -1;
        }

        private async void FreezeBtn_Click(object sender, EventArgs e) {
            if (freezeSelector.SelectedIndex == -1) {
                MessageBox.Show("Select a server to freeze", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Command command = new Freeze(this, freezeSelector.Text);
            await CommandHandler(command);

            freezeSelector.SelectedIndex = -1;
        }

        private async void UnfreezeBtn_Click(object sender, EventArgs e) {
            if (unfreezeSelector.SelectedIndex == -1) {
                MessageBox.Show("Select a server to unfreeze", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Command command = new Unfreeze(this, unfreezeSelector.Text);
            await CommandHandler(command);

            unfreezeSelector.SelectedIndex = -1;
        }

        private async void StatusBtn_Click(object sender, EventArgs e) {
            Command command = new Commands.Status(this);
            await CommandHandler(command);
        }

        private async void ClientBtn_Click(object sender, EventArgs e) {
            if (clientName.Text == "") {
                MessageBox.Show("Insert a name for the client", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } else if (clientURL.Text == "") {
                MessageBox.Show("Insert a URL for the client", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } else if (clientScript.Text == "") {
                MessageBox.Show("Insert a script for the client to run", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Command command;
            try {
                command = new Client(this, clientName.Text, clientURL.Text, clientScript.Text);
            } catch (InvalidURLException) {
                MessageBox.Show("Invalid URL. Must start with 'http:'", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            await CommandHandler(command);

            clientName.Text = clientURL.Text = clientScript.Text = "";
        }

        private async void ServerBtn_Click(object sender, EventArgs e) {
            if (serverName.Text == "") {
                MessageBox.Show("Insert a name for the server", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } else if (serverURL.Text == "") {
                MessageBox.Show("Insert a URL for the server", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Command command;
            try {
                command = new Commands.Server(this, serverName.Text, serverURL.Text, Decimal.ToInt32(minDelay.Value), Decimal.ToInt32(maxDelay.Value));
            } catch (InvalidURLException) {
                MessageBox.Show("Invalid URL. Must start with 'http:'", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await CommandHandler(command);

            serverName.Text = serverURL.Text = "";
            minDelay.Value = maxDelay.Value = 0;
        }

        private void ServerList_ItemCheck(object sender, ItemCheckEventArgs e) {
            lock (this.partitionServers) {
                if (e.NewValue == CheckState.Checked) {
                    this.partitionServers.Add((string)serverList.Items[e.Index]);
                } else {
                    this.partitionServers.Remove((string)serverList.Items[e.Index]);
                }
            }
        }

        private async void PartitionBtn_Click(object sender, EventArgs e) {
            if (partitionName.Text == "") {
                MessageBox.Show("Insert a name for the partition", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } else if (this.partitionServers.Count == 0) {
                MessageBox.Show("Select at least 1 server to create a partition", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Command command;
            lock (this.partitionServers) {
                command = new Partition(this, this.partitionServers.Count, partitionName.Text, this.partitionServers);
            }

            await CommandHandler(command);

            lock (this.partitionServers) {
                this.partitionServers.Clear();
            }
            
            for(int i = 0; i < serverList.Items.Count; i++) {
                serverList.SetItemCheckState(i, CheckState.Unchecked);
            }

            partitionName.Text = "";
        }
    }
}
