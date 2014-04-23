using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SuperPuTTY.Manager;

namespace SuperPuTTY.Scp
{
    public partial class PscpBrowserPanel : ToolWindowDocument
    {
        FileTransferPresenter fileTransferPresenter;
        IBrowserPresenter localBrowserPresenter;
        IBrowserPresenter remoteBrowserPresenter;

        public PscpBrowserPanel()
        {
            InitializeComponent();
        }

        public PscpBrowserPanel(SessionData session, PscpOptions options) :
            this(session, options, Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
        { }

        public PscpBrowserPanel(SessionData session, PscpOptions options, string localStartingDir) : this()
        {
            var fileTransferSession = (SessionData) session.Clone();

            // if the session doesn't contain a host, a port, or an username
            // try to get them from the putty profile
            if ((string.IsNullOrEmpty(fileTransferSession.Username) || string.IsNullOrEmpty(fileTransferSession.Host) || fileTransferSession.Port == 0)
                && !string.IsNullOrEmpty(fileTransferSession.PuttySession))
            {
                var puttyProfile = PuttyDataHelper.GetSessionData(fileTransferSession.PuttySession);

                fileTransferSession.Username = string.IsNullOrEmpty(fileTransferSession.Username)
                    ? puttyProfile.Username
                    : fileTransferSession.Username;

                fileTransferSession.Host = string.IsNullOrEmpty(fileTransferSession.Host)
                    ? puttyProfile.Host
                    : fileTransferSession.Host;

                fileTransferSession.Port = (fileTransferSession.Port == 0)
                    ? puttyProfile.Port
                    : fileTransferSession.Port;
            }

            this.Name = fileTransferSession.SessionName;
            this.TabText = fileTransferSession.SessionName;

            this.fileTransferPresenter = new FileTransferPresenter(options);
            this.localBrowserPresenter = new BrowserPresenter(
                "Local", new LocalBrowserModel(), fileTransferSession, fileTransferPresenter);
            this.remoteBrowserPresenter = new BrowserPresenter(
                "Remote", new RemoteBrowserModel(options), fileTransferSession, fileTransferPresenter);

            this.browserViewLocal.Initialize(this.localBrowserPresenter, new BrowserFileInfo(new DirectoryInfo(localStartingDir)));
            this.browserViewRemote.Initialize(this.remoteBrowserPresenter, RemoteBrowserModel.NewDirectory(ScpUtils.GetHomeDirectory(fileTransferSession)));
            this.fileTransferView.Initialize(this.fileTransferPresenter);
        }
    }
}
