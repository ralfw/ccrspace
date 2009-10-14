using System;
using System.Threading;
using System.Windows.Forms;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;

namespace Test.ChannelWithSyncContext
{
    public partial class Form1 : Form
    {
        private readonly Port<int> chMakeProgress;
        private readonly Port<int> chReportProgress;


        public Form1()
        {
            InitializeComponent();

            var cfg = new CcrsChannelConfig<int>
                          {
                              MessageHandler = n=>this.textBox1.Text=n.ToString(),
                              HandlerMode = CcrsChannelHandlerModes.InCurrentSyncContext
                          };
            this.chReportProgress = new ChannelFactory().CreateChannel(cfg);

            cfg = new CcrsChannelConfig<int>
            {
                MessageHandler = MakeProgress,
                HandlerMode = CcrsChannelHandlerModes.Parallel
            };
            this.chMakeProgress = new ChannelFactory().CreateChannel(cfg);
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
