using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CcrSpaces.Api;
using CcrSpaces.Api.Extensions;

namespace Test.ChannelWithSyncContext
{
    public partial class Form1 : Form
    {
        private readonly ICcrsDuplexChannel<int, int> chProgress;


        public Form1()
        {
            InitializeComponent();

            this.chProgress = new CcrsRequestMultiResponseChannel<int, int>(MakeProgress);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.chProgress.Post(50, n => this.textBox1.Text = n.ToString(), true);
        }


        private void MakeProgress(int n, ICcrsSimplexChannel<int> showProgress)
        {
            for(int i=0; i<n; i++)
            {
                showProgress.Post(i);
                Thread.Sleep(100);
            }
        }

    }
}
