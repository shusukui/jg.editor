using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Diagnostics;
using System.Runtime.InteropServices;

using System.Net;
using System.Net.Sockets;
namespace jg.PCPlayer
{
    public class Program
    {

        [STAThread]
        static void Main(string[] args)
        {

            if (args.Length == 0) return;
            jg.PCPlayerLibrary.Entrance entrance = new jg.PCPlayerLibrary.Entrance(args[0]);

            App app = new App();
            app.InitializeComponent();
            entrance.windowPreview = new jg.PCPlayerLibrary.MainWindow();
            entrance.windowPreview.TittleName = entrance.CourseName;
            entrance.windowPreview.ShowDialog();
        }
    }
}