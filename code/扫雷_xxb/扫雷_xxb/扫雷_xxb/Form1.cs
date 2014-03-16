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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Button[,] Mines;   //定义一个 二维动态数组 用于显示雷区
        private int XNum = 9;   //初始化累的列数（即为：初级时的行列数）
        private int YNum = 9;   //初始化雷的行数
        public static int zdyXNum;
        public static int zdyYNum;
        private int MineNum = 10;   //初始化雷的总数
        public static int zdyMineNum;
        private int[,] Turn;   //用二维数组赋值：-1 表示这个位置已经翻开；0 表示这个位置没有翻开；1 表示这个位置插上红旗；

        private int CostTime = 0;   //计量所用的时间
        private int StartTime = 0;   //初始化时间
        private int RestMine = 10;   //用于改变等级时载入剩余雷数
        private int MineWidth = 20;   // 控制雷块的大小

        private void button1_Click(object sender, EventArgs e)
        {
            DelAllMines();   //删除所有的雷区控件（很重要，用于不让其改变等级时有参与）
            RestMine = MineNum;
            CostTime = 0;
            label1.Text = CostTime.ToString();
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;
        }

        private void DelAllMines()   //删除所有的
        {
            for (int i = 0; i < XNum; i++)
                for (int j = 0; j < YNum; j++)
                {
                    Button n = new Button();
                    n = (Button)Mines[i,j];
                    this.Controls.Remove(Mines[i, j]);
                }
        }

        private void GamesBegin()
        {
            Turn = new int[XNum, YNum];
            Mines = new Button[XNum, YNum];
            for (int x = 0; x < XNum; x++)
                for (int y = 0; y < YNum; y++)
                {
                    Mines[x, y] = new Button();
                    this.Controls.Add(Mines[x, y]);
                    Mines[x, y].Left = 10 + MineWidth * x;
                    Mines[x, y].Top = 65 + MineWidth * y;
                    Mines[x, y].Width = MineWidth;
                    Mines[x, y].Height = MineWidth;
                    Mines[x, y].Font = new Font("宋体", 10.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(134)));
                    Mines[x, y].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                    Mines[x, y].Name = "Mines" + (x + y * XNum).ToString();
                    Mines[x, y].MouseUp += new MouseEventHandler(bt_MouseUp);
                    Mines[x, y].Visible = true;
                }
            detform();
        }

        private void detform()   //决定form1的整体框架
        {
            button1.Location = new Point( -10 + XNum * MineWidth/2 , -1 );
            button2.Location = new Point( -25 + XNum * MineWidth/2 ,65 + YNum * MineWidth);
            panel1.Size = new Size(30 + MineWidth * XNum, 35);
            label2.Location = new Point(XNum * MineWidth - 30, 7);
            Form1.ActiveForm.Width = 30 + MineWidth * XNum;
            Form1.ActiveForm.Height = 130 + MineWidth * YNum;
        }

        private void GameInit()  //游戏初始化
        {
            RestMine = MineNum;
            label1.Text = RestMine.ToString();
            for (int x = 0; x < XNum; x++)
                for (int y = 0; y < YNum; y++)
                {
                    Mines[x, y].Text = "";
                    Mines[x, y].Visible = true;
                    Mines[x, y].Enabled = true;
                    Mines[x, y].Tag = null;
                    Mines[x, y].BackgroundImage = null;
                    Turn[x, y] = 0;        //刚开始都未插旗、未表示为雷
                }
            LayMines();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CostTime++;
            label2.Text = CostTime.ToString();
        }

        private void bt_MouseUp(object sender, MouseEventArgs e) //这里处理事件方法
        {
            String btName;
            Button bClick = (Button)sender;//将被击的按钮赋给定义的bClick变量
            btName = bClick.Name;//获取按钮的Name
            int n = Convert.ToInt16(btName.Substring(5));
            int x = n % XNum;
            int y = n / XNum;
            //通过按钮Name属性来判断是哪个Button被点击，并执行相应的操作
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (Convert.ToInt16(Mines[x, y].Tag) != 1)
                    {
                        detpicture(GetAroundNum(x, y), x, y);
                        Mines[x, y].Enabled = false;
                        ExpandMines(x, y);
                        if (Victory())
                        {
                            // 判断是否胜利，是则将地图中所有雷标识出来
                            show();
                            MessageBox.Show("胜利!!!", "结束");
                            timer1.Enabled = false;//停止计时
                            // Endsgames();
                        }
                    }
                    else
                    {
                        Mines[x, y].BackgroundImage = Image.FromFile("mine1.bmp");
                        MessageBox.Show("失败!!!", "结束");
                        timer1.Enabled = false;//停止计时
                        // Endsgames();
                    }
                    break;
                case MouseButtons.Right:
                    Mines[x, y].BackgroundImage = Image.FromFile("flag.bmp");
                    if (Turn[x, y] == 1)//表示这个位置插上红旗
                    {
                        Turn[x, y] = 0;//取消红旗,表示这个位置没有翻开
                        RestMine++;
                        Mines[x, y].BackgroundImage = null;
                    }
                    else
                    {
                        Turn[x, y] = 1;//表示这个位置插上红旗
                        RestMine--;
                    }
                    label1.Text = RestMine.ToString();
                    if (Victory())
                    {
                        MessageBox.Show("胜利!!!", "结束");
                        timer1.Enabled = false;//停止计时
                    }
                    break;
            }
        }

        private void LayMines()
        {
            int x, y;
            Random s = new Random();
            //取随机数
            for (int i = 0; i < MineNum; )
            {
                //取随机数
                x = s.Next(XNum);     //取随机数，返回一个小于所指定最大值的非负随机数
                y = s.Next(YNum);
                if (Convert.ToInt16(Mines[x, y].Tag) != 1)
                {
                    //==1时，代表这个位置是地雷
                    Mines[x, y].Tag = 1;//修改属性为雷
                    //Mines[x, y].Text = "u";
                    i++;
                }
            }
            label1.Text = MineNum.ToString();
            CostTime = 0;
            label2.Text = StartTime.ToString();
        }

        private void detpicture(int n, int i, int j)
        {
            switch (n)
            {
                case 1:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("1.PNG");
                        break;
                    }
                case 2:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("2.PNG");
                        break;
                    }
                case 3:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("3.PNG");
                        break;
                    }
                case 4:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("4.PNG");
                        break;
                    }
                case 5:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("5.PNG");
                        break;
                    }
                case 6:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("6.PNG");
                        break;
                    }
                case 7:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("7.PNG");
                        break;
                    }
                case 8:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("8.PNG");
                        break;
                    }
            }
        }

        private int GetAroundNum(int row, int col)
        {
            int i, j;
            int around = 0;
            int minRow = (row == 0) ? 0 : row - 1;
            int maxRow = row + 2;
            int minCol = (col == 0) ? 0 : col - 1;
            int maxCol = col + 2;
            for (i = minRow; i < maxRow; i++)
            {
                for (j = minCol; j < maxCol; j++)     //[row,col]处没有雷
                {
                    if (!IsInMineArea(i, j))//判断是否在扫雷区域
                        continue;
                    if (Convert.ToInt16(Mines[i, j].Tag) == 1) around++;
                }
            }
            return around;
        }

        private void ExpandMines(int row, int col)
        {
            int i, j;
            int minRow = (row == 0) ? 0 : row - 1;
            int maxRow = row + 2;
            int minCol = (col == 0) ? 0 : col - 1;
            int maxCol = col + 2;
            int around = GetAroundNum(row, col);
            //对周围一个雷都没有的空白区域拓展
            if (around == 0)
            {
                Mines[row, col].Enabled = false;
                for (i = minRow; i < maxRow; i++)
                {
                    for (j = minCol; j < maxCol; j++)
                    {
                        //对于周围可以拓展的区域进行的规拓展			
                        if (!IsInMineArea(i, j)) continue;
                        if (!(i == row && j == col) && Mines[i, j].Enabled != false)
                        //&& Convert.ToInt16(Mines[i,j].Tag)!= 1
                        {
                            ExpandMines(i, j);
                        }
                        Mines[i, j].Enabled = false;//周围无雷的区域按钮无效
                        detpicture(GetAroundNum(i, j), i, j);
                    }
                }
            }
        }

        private bool Victory()// 检测是否胜利
        {
            for (int i = 0; i < XNum; i++)
                for (int j = 0; j < YNum; j++)
                {
                    //没翻开且未标示,则未成功
                    if (Mines[i, j].Enabled == true && Turn[i, j] != 1) return false;
                    //不是雷却误标示为雷,则也未成功
                    if (Convert.ToInt16(Mines[i, j].Tag) != 1 && Turn[i, j] == 1) return false;
                }
            return true;
        }

        private void show()//将地图中所有雷标识出来
        {
            for (int i = 0; i < XNum; i++)
                for (int j = 0; j < YNum; j++)
                    if (Convert.ToInt16(Mines[i, j].Tag) == 1)
                    {
                        //==1时，代表这个位置是地雷
                        Mines[i, j].BackgroundImage = Image.FromFile("mine.bmp");
                    }
        }

        private bool IsInMineArea(int row, int col)
        {
            return (row >= 0 && row < XNum && col >= 0 && col < YNum);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            初级ToolStripMenuItem.Checked = true;
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;
            detform();
        }

        private void 初级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            初级ToolStripMenuItem.Checked = true;
            中级ToolStripMenuItem.Checked = false;
            高级ToolStripMenuItem.Checked = false;
          //  自定义ToolStripMenuItem.Checked = false;
            DelAllMines();
            XNum = 9;
            YNum = 9;
            MineNum = 10;
            RestMine = MineNum;
            label1.Text = CostTime.ToString();
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;
            detform();

        }

        private void 中级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            初级ToolStripMenuItem.Checked = false;
            中级ToolStripMenuItem.Checked = true;
            高级ToolStripMenuItem.Checked = false;
           // 自定义ToolStripMenuItem.Checked = false;
            DelAllMines();
            XNum = 16;
            YNum = 16;
            MineNum = 40;
            RestMine = MineNum;
            label1.Text = CostTime.ToString();
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;
            detform();

        }

        private void 高级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            初级ToolStripMenuItem.Checked = false;
            中级ToolStripMenuItem.Checked = false;
            高级ToolStripMenuItem.Checked = true;
            // 自定义ToolStripMenuItem.Checked = false;
            DelAllMines();
            XNum = 30;
            YNum = 16;
            MineNum = 99;
            RestMine = MineNum;
            label1.Text = CostTime.ToString();
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;
            detform();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            show();//将地图中所有雷标识出来
        }

        private void 新游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DelAllMines();   //删除所有的雷区控件（很重要，用于不让其改变等级时有参与）
            RestMine = MineNum;
            CostTime = 0;
            label1.Text = CostTime.ToString();
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        private void 自定义ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            初级ToolStripMenuItem.Checked = false;
            中级ToolStripMenuItem.Checked = false;
            高级ToolStripMenuItem.Checked = false;
            自定义ToolStripMenuItem.Checked = true;
            Form zdy = new Form2();
            zdy.ShowDialog();
            if (zdy.DialogResult == DialogResult.OK)
                zdyGames();
        }

        private void zdyGames()
        {
            DelAllMines();
            XNum = zdyXNum;
            YNum = zdyYNum;
            MineNum = zdyMineNum;
            label1.Text = CostTime.ToString();
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;
        }
    }
}
