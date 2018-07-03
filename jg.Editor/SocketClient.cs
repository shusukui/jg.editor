using System;
using JG.Library.Net.TcpCSFramework;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
namespace jg.Editor
{
    public delegate void NetRecvDataEvent(string RegCode);
    public class SocketClient
    {
        public event NetRecvDataEvent RecvData;

        public event NetEvent ConnectedServer;
        public event NetEvent DisConnectedServer;
        public event NetEvent ReceivedDatagram;
        public event NetEvent ConnectFailed;
        TcpCli cli;

        public string pwd = "";
        string Command = "";
        public const string RESOLVER = "##";

        private int ConnectCount = 0;

        private string localIP;
        private int localPort;

        public bool IsConnected
        {
            get { return cli.IsConnected; }
        }

        public SocketClient()
        {
            Console.WriteLine("正在尝试连接到服务器。");
            cli = new TcpCli(new Coder(Coder.EncodingMothord.UTF8));
            cli.ReceivedDatagram += new NetEvent(cli_ReceivedDatagram);
            cli.ConnectedServer += new NetEvent(cli_ConnectedServer);
            cli.DisConnectedServer += new NetEvent(cli_DisConnectedServer);
            cli.ConnectFailed += cli_ConnectFailed;
            cli.Resovlver = new DatagramResolver(RESOLVER);
        }

        void cli_ConnectFailed(object sender, NetEventArgs e)
        {
            if (ConnectFailed != null) ConnectFailed(sender, e);
        }
        public void Connected(string localIP, int localPort)
        {
            this.localIP = localIP;
            this.localPort = localPort;
            cli.Connect(localIP, localPort);           
        }
        public void WriteData(string line)
        {
            Console.WriteLine(line);
            Console.Write(">");
        }

        public void Send(string Message)
        {
            string cmd = Message;

            #region send
            cli.Send(cmd);
            #endregion


            //IsOver = false;

        }

        private void cli_DisConnectedServer(object sender, NetEventArgs e)
        {
            if (DisConnectedServer != null) DisConnectedServer(sender, e);
        }

        private void cli_ConnectedServer(object sender, NetEventArgs e)
        {
            if (ConnectedServer != null) ConnectedServer(sender, e);
            Console.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "已成功连接到服务器。");
        }

        private void cli_ReceivedDatagram(object sender, NetEventArgs e)
        {
            if (ReceivedDatagram != null) ReceivedDatagram(sender, e);
            Console.WriteLine(e.Client.Datagram);
        }
    }
}