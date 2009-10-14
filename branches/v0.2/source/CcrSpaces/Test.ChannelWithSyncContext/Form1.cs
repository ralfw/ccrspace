using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;

namespace Test.ChannelWithSyncContext
{
    public partial class Form1 : Form
    {
        private Port<int> chMakeProgress;
        private Port<int> chReportProgress;


        public Form1()
        {
            InitializeComponent();

            var cfg = new CcrsChannelConfig<int>
                          {
                              MessageHandler = n=>this.textBox1.Text=n.ToString(),
                              HandlerMode = CcrsChannelHandlerModes.InCurrentSyncContext
                          };
            this.chReportProgress = new ChannelFactory().CreateChannel<int>(cfg);

            cfg = new CcrsChannelConfig<int>
            {
                MessageHandler = MakeProgress,
                HandlerMode = CcrsChannelHandlerModes.Parallel
            };
            this.chMakeProgress = new ChannelFactory().CreateChannel<int>(cfg);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.chMakeProgress.Post(50);
        }


        private void MakeProgress(int n)
        {
            for(int i=0; i<n; i++)
            {
                this.chReportProgress.Post(i);
                Thread.Sleep(100);
            }
        }
    }
}
