using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace 扫雷_xxb
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            string path1, mr1 = "名字\t\t成绩\r\n", mystr1 = "";
            path1 = System.Windows.Forms.Application.StartupPath; 
            FileStream fs1 = File.OpenRead(path1 + "\\扫雷英雄榜.txt");
            StreamReader sr1 = new StreamReader(fs1);
            fs1.Seek(0, SeekOrigin.Begin);
            while (sr1.Peek() > -1)
            {
                mystr1 += sr1.ReadLine() + "\t\t";
                mystr1 += sr1.ReadLine() + "\r\n";
            }
            sr1.Close();
            fs1.Close();
            textBox1.Text = mr1 + mystr1;
        }
    }
}
