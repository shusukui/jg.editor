using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Skybound.Gecko;
namespace jg.Editor.Library
{
    public partial class WebBrowerGecko : UserControl
    {
        private string _xulrunnerPath = @"D:\xulrunner/";

        /// <summary>
        /// 
        /// </summary>
        public string XulrunnerPath
        {
            get { return _xulrunnerPath; }
            set { _xulrunnerPath = value; }
        }
     
         private bool _IsLoad;

         public bool IsLoad
         {
             get { return _IsLoad; }
             set { _IsLoad = value; }
         }  
        private GeckoWebBrowser _Browser;

        public GeckoWebBrowser Browser
        {
            get { return _Browser; }
            set { _Browser = value; }
        }  
        public WebBrowerGecko()
        {
            InitializeComponent();
            _Browser = new GeckoWebBrowser();
            Skybound.Gecko.Xpcom.Initialize(XulrunnerPath);  // initialize the xulrunner, load profile and set preferences
            _Browser.Parent = this;
           
            _Browser.Dock = System.Windows.Forms.DockStyle.Fill;
            _Browser.HandleCreated += _Browser_HandleCreated;
            _Browser.HandleDestroyed += _Browser_HandleDestroyed;
            this.Controls.Add(_Browser);
            this.Dock = System.Windows.Forms.DockStyle.Fill;
        }
      

          void _Browser_HandleCreated(object sender, EventArgs e)
          {
              IsLoad = true;
          }

          void _Browser_HandleDestroyed(object sender, EventArgs e)
          {
              IsLoad = false;
          }

          public void Navigate(string url)
          {
              _Browser.Navigate(url);
          }
    }
}
