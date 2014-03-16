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
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath; // bin路径 
            string name;
            name = textBox1.Text;
            if (name == "") 
            {
                this.Close(); 
                return;
            }
            string  mystr = "";//mr = "名字\t\t成绩\r\n",
            path = System.Windows.Forms.Application.StartupPath; // bin路径 
            FileStream fs = File.OpenRead(path + "\\扫雷英雄榜.txt");
            StreamReader sr = new StreamReader(fs);
            fs.Seek(0, SeekOrigin.Begin);
            while (sr.Peek() > -1)
            {
                mystr += sr.ReadLine() + "\t\t";
                mystr += sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            mystr += name + "\r\n" + 扫雷_xxb.Form1.CostTime.ToString() + "\r\n";
            FileStream fsa = File.OpenWrite(path + "\\扫雷英雄榜.txt");
            StreamWriter sw = new StreamWriter(fsa);
            sw.WriteLine(mystr);
            sw.Close();
            fsa.Close();
            this.Close();
        }


    }
}
