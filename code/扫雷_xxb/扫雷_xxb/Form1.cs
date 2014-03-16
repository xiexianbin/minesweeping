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
        public static int zdyXNum;   //用于自定义中的列数
        public static int zdyYNum;   //用于自定义中的行数
        private int MineNum = 10;   //初始化雷的总数
        public static int zdyMineNum;   //用于记录自定义中的雷数
        private int[,] Turn;   //用二维数组赋值：-1 表示这个位置已经翻开；0 表示这个位置没有翻开；1 表示这个位置插上红旗；

        public static int CostTime = 0;   //计量所用的时间
        private int StartTime = 0;   //初始化时间
        private int RestMine = 10;   //用于改变等级时载入剩余雷数
        private int MineWidth = 20;   // 控制雷块的大小

        private void button1_Click(object sender, EventArgs e)   //这是一个开始按钮，单击即开始游戏
        {
            button1.Image = Image.FromFile("face.bmp");   //用于控制开始按钮的图标，开始时位笑脸
            DelAllMines();   //删除所有的雷区控件（很重要，用于不让其改变等级时有参与）
            RestMine = MineNum;   //用于记录雷数，开始时剩余雷数为总雷数
            CostTime = 0;   //用于记录从开始到现在游戏用时，开始为0
            label1.Text = CostTime.ToString();   //label1窗体中用于显示所用的时间
            GamesBegin();   //开始游戏
            GameInit();   //游戏初始化
            timer1.Enabled = true;   //触发计时器
        }

        private void DelAllMines()   //删除所有的雷区
        {
            for (int i = 0; i < XNum; i++)
                for (int j = 0; j < YNum; j++)   //二维数组逐个删除
                {
                    Button n = new Button();   //定义一个新的button
                    n = (Button)Mines[i,j];   //强制类型转换
                    this.Controls.Remove(Mines[i, j]);   //删除所指雷区
                }
        }

        private void GamesBegin()   //用于开始游戏，主要是定义雷区
        {
            Turn = new int[XNum, YNum];   //定义新的二维数组
            Mines = new Button[XNum, YNum];   //定义按钮
            for (int x = 0; x < XNum; x++)
                for (int y = 0; y < YNum; y++)   //通过二维数组，逐个定义初始化button按钮
                {
                    Mines[x, y] = new Button();
                    this.Controls.Add(Mines[x, y]);   //增加新按钮
                    Mines[x, y].Left = 10 + MineWidth * x;   //定义雷区开始在Form窗体中的左边界
                    Mines[x, y].Top = 65 + MineWidth * y;   //定义雷区开始在Form窗体中的上边界
                    Mines[x, y].Width = MineWidth;   //定义雷块的宽度
                    Mines[x, y].Height = MineWidth;   //定义雷块的高度
                    Mines[x, y].Font = new Font("宋体", 10.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(134)));   //定义字体
                    Mines[x, y].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;   //定义backgroundimageLayout
                    Mines[x, y].Name = "Mines" + (x + y * XNum).ToString();   //定义雷区的名字
                    Mines[x, y].MouseUp += new MouseEventHandler(bt_MouseUp);   //定义单击事件
                    Mines[x, y].Visible = true;   //控制Mines按钮的可见
                }
            detform();
        }

        private void Load_Mine()   //这个与上面的基本相同，主要是了解决在开始时单击人一个按钮均会胜利的问题，一下不做解释。
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
        }

        private void detform()   //决定form1的整体框架
        {
            button1.Location = new Point( -10 + XNum * MineWidth/2 , -1 );   //控制form1中的button1按钮开始的位置
            button2.Location = new Point(-25 + XNum * MineWidth / 2, 65 + YNum * MineWidth);   ////控制form1中的button2按钮开始的位置
            panel1.Size = new Size(30 + MineWidth * XNum, 35);   //控制panel1的大小
            label2.Location = new Point(XNum * MineWidth - 30, 7);   //控制label2的位置
            Form1.ActiveForm.Width = 30 + MineWidth * XNum;   //用于控制form窗体的宽度
            Form1.ActiveForm.Height = 130 + MineWidth * YNum;   //用于控制form窗体的高度
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

        private void timer1_Tick(object sender, EventArgs e)   //定义计时器
        {
            CostTime++;
            label2.Text = CostTime.ToString();
        }

        private void bt_MouseUp(object sender, MouseEventArgs e)    //这里处理事件方法
        {
            String btName;
            Button bClick = (Button)sender;   //将被击的按钮赋给定义的bClick变量
            btName = bClick.Name;   //获取按钮的Name
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
                            MessageBox.Show("恭喜您，胜利啦!!!", "游戏结束");
                            Form v = new Form4();
                            v.ShowDialog();
                            timer1.Enabled = false;//停止计时
                            // Endsgames();
                        }
                    }
                    else
                    {
                        Mines[x, y].BackgroundImage = Image.FromFile("mine1.bmp");
                        button1.Image = Image.FromFile("face1.jpg");
                        MessageBox.Show("真的很遗憾，您失败啦!!!", "游戏结束");                       
                        timer1.Enabled = false;//停止计时
                        Endall();
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
                        MessageBox.Show("恭喜您，胜利啦!!!", "游戏结束");
                        Form vic = new Form4();
                        vic.ShowDialog();
                        timer1.Enabled = false;//停止计时
                    }
                    break;
            }
        }

        private void Endall()
        {
            for (int i = 0; i < XNum; i++)
                for (int j = 0; j < YNum; j++)
                {
                    Mines[i, j].Enabled = false;
                }
        }

        private void LayMines()   //布雷
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

        private void detpicture(int n, int i, int j)   //用于调用不同的图片显示所单击按钮周围所剩的雷数
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

        private int GetAroundNum(int row, int col)   //用于获取所单击按钮周围8个雷块中所剩的雷数
        {
            int i, j;
            int around = 0;   //定义所生的雷数，开始为0
            int minRow = (row == 0) ? 0 : row - 1;
            int maxRow = row + 2;
            int minCol = (col == 0) ? 0 : col - 1;
            int maxCol = col + 2;
            for (i = minRow; i < maxRow; i++)
            {
                for (j = minCol; j < maxCol; j++)     //[row,col]处没有雷
                {
                    if (!IsInMineArea(i, j))   //判断是否在扫雷区域
                        continue;
                    if (Convert.ToInt16(Mines[i, j].Tag) == 1) around++;
                }
            }
            return around;   //返回所生的雷数
        }

        private void ExpandMines(int row, int col)
        {
            int i, j;
            int minRow = (row == 0) ? 0 : row - 1;
            int maxRow = row + 2;
            int minCol = (col == 0) ? 0 : col - 1;
            int maxCol = col + 2;
            int around = GetAroundNum(row, col);   //对周围一个雷都没有的空白区域拓展
            if (around == 0)
            {
                Mines[row, col].Enabled = false;
                for (i = minRow; i < maxRow; i++)
                {
                    for (j = minCol; j < maxCol; j++)
                    {
                        //对于周围可以拓展的区域进行的规拓展			
                        if (!IsInMineArea(i, j)) continue;
                        if (!(i == row && j == col) && Mines[i, j].Enabled != false)   //&& Convert.ToInt16(Mines[i,j].Tag)!= 1
                        {
                            ExpandMines(i, j);
                        }
                        Mines[i, j].Enabled = false;   //周围无雷的区域按钮无效
                        detpicture(GetAroundNum(i, j), i, j);
                    }
                }
            }
        }

        private bool Victory()   // 检测是否胜利
        {
            for (int i = 0; i < XNum; i++)
                for (int j = 0; j < YNum; j++)
                {
                    //没翻开且未标示,则未成功
                    if (Mines[i, j].Enabled == true && Turn[i, j] != 1) 
                        return false;
                    //不是雷却误标示为雷,则也未成功
                    if (Convert.ToInt16(Mines[i, j].Tag) != 1 && Turn[i, j] == 1) 
                        return false;
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

        private bool IsInMineArea(int row, int col)   //判断是否已出雷区
        {
            return (row >= 0 && row < XNum && col >= 0 && col < YNum);   //返回true or false
        }

        private void Form1_Load(object sender, EventArgs e)   //Form1_Load，用于基本开始界面的设置
        {
            string path;   //定义字符串，用于存放“扫雷英雄榜.txt”的位置
            path = System.Windows.Forms.Application.StartupPath;    // bin路径
            if (!File.Exists(path + "\\扫雷英雄榜.txt"))   //if语句，主要是通过检测是否有扫雷英雄榜.txt，如果没有。。。
                File.Create(path + "\\扫雷英雄榜.txt");    //创建扫雷英雄榜.txt
            初级ToolStripMenuItem.Checked = true;   //使菜单中的初级可用
            Load_Mine();   //用于解决开始时单击任意按钮均胜利的问题
            GameInit();    //游戏初始化
            timer1.Enabled = true;   //开启时钟计时
        }

        private void 初级ToolStripMenuItem_Click(object sender, EventArgs e)    //初级按钮的定义
        {
            button1.Image = Image.FromFile("face.bmp");   //开始时，让button1按钮的Image设为face.bmp
            初级ToolStripMenuItem.Checked = true;   //初级按钮可用
            中级ToolStripMenuItem.Checked = false;   //中级按钮不可用
            高级ToolStripMenuItem.Checked = false;   //高级按钮不可用
            自定义ToolStripMenuItem.Checked = false;   //自定义按钮不可用
            DelAllMines();   //删除残余雷片
            XNum = 9;   //定义雷区的列数
            YNum = 9;   //定义雷区的行数
            MineNum = 10;   //定义总雷数
            RestMine = MineNum;   //计数剩余的雷数
            label1.Text = CostTime.ToString();   //用于在开始界面显示所用的时间
            GamesBegin();   //开始游戏
            GameInit();   //游戏初始化
            timer1.Enabled = true;   //触发计时器
            detform();   //控制Form窗体

        }

        private void 中级ToolStripMenuItem_Click(object sender, EventArgs e)    //中级按钮的定义
        {
            button1.Image = Image.FromFile("face.bmp");   //开始时，让button1按钮的Image设为face.bmp
            初级ToolStripMenuItem.Checked = false;   //初级按钮不可用
            中级ToolStripMenuItem.Checked = true;   //中级按钮可用
            高级ToolStripMenuItem.Checked = false;   //高级按钮不可用
            自定义ToolStripMenuItem.Checked = false;   //自定义按钮不可用
            DelAllMines();   //删除残余雷片
            XNum = 16;   //定义雷区的列数
            YNum = 16;   //定义雷区的行数
            MineNum = 40;   //定义总雷数
            RestMine = MineNum;   //计数剩余的雷数
            label1.Text = CostTime.ToString();   //用于在开始界面显示所用的时间
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;   //触发计时器
            detform();   //控制Form窗体

        }

        private void 高级ToolStripMenuItem_Click(object sender, EventArgs e)    //高级按钮的定义
        {
            button1.Image = Image.FromFile("face.bmp");   //开始时，让button1按钮的Image设为face.bmp
            初级ToolStripMenuItem.Checked = false;   //初级按钮不可用
            中级ToolStripMenuItem.Checked = false;   //中级按钮不可用
            高级ToolStripMenuItem.Checked = true;   //高级按钮可用
            自定义ToolStripMenuItem.Checked = false;   //自定义按钮不可用
            DelAllMines();   //删除残余雷片
            XNum = 30;   //定义雷区的列数
            YNum = 16;   //定义雷区的行数
            MineNum = 99;   //定义总雷数
            RestMine = MineNum;   //计数剩余的雷数
            label1.Text = CostTime.ToString();   //用于在开始界面显示所用的时间
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;   //触发计时器
            detform();   //控制Form窗体
        }

        private void button2_Click(object sender, EventArgs e)   //显示所有的雷数
        {
            show();   //将地图中所有雷标识出来
        }

        private void 新游戏ToolStripMenuItem_Click(object sender, EventArgs e)    //新游戏按钮的定义
        {
            button1.Image = Image.FromFile("face.bmp");   //开始时，让button1按钮的Image设为face.bmp
            DelAllMines();   //删除所有的雷区控件（很重要，用于不让其改变等级时有参与）
            RestMine = MineNum;   //计数剩余的雷数
            CostTime = 0;   //计数所用时间
            label1.Text = CostTime.ToString();   //用于在开始界面显示所用的时间
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;   //触发计时器
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)   //定义退出
        {
            Application.Exit();   //单击退出
        }

        private void 自定义ToolStripMenuItem_Click(object sender, EventArgs e)    //自定义按钮的定义
        {
            button1.Image = Image.FromFile("face.bmp");   //开始时，让button1按钮的Image设为face.bmp
            初级ToolStripMenuItem.Checked = false;   //初级按钮不可用
            中级ToolStripMenuItem.Checked = false;   //中级按钮不可用
            高级ToolStripMenuItem.Checked = false;   //高级按钮不可用
            自定义ToolStripMenuItem.Checked = true;   //自定义按钮可用
            Form zdy = new Form2();   //通过定义调用新的Form窗体
            zdy.ShowDialog();   //显示自定义窗体
            if (zdy.DialogResult == DialogResult.OK)   //如果在自定义窗体中单击 确定
                zdyGames();   //开始自定义游戏
        }

        private void zdyGames()   //自定义游戏
        {
            DelAllMines();   //删除残余雷片
            XNum = zdyXNum;   //定义雷区的列数
            YNum = zdyYNum;   //定义雷区的行数
            MineNum = zdyMineNum;   //定义总雷数
            label1.Text = CostTime.ToString();   //用于在开始界面显示所用的时间
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;   //触发计时器
        }

        private void 关于扫雷ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("游戏简介：\r\n\r\n------------------------------------------\r\n\r\n扫雷是单人计算机游戏,游戏的用户界面十分友好！\r\n\r\n------------------------------------------\r\n\r\n当游戏方格组成的战场构成开始时，计算机将固定的地雷数随机的放到战场的方格。\r\n\r\n------------------------------------------\r\n\r\n此游戏还允许玩家指定雷区中的地雷数,让您尽情玩到底！！\r\n\r\n------------------------------------------\r\n\r\n    据有关调查显示：此游戏具有开发智力少儿的功能！！  ", " 说 明：");   //显示游戏简介
        }

        private void 关于游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("关于版权：\r\n\r\n本游戏是防Micosoft软件扫雷游戏 由\r\n\r\n    郑州轻工业学院 \r\n    计算机与通信工程学院\r\n    计算机科学与技术10-01班\r\n    谢先斌\r\n\r\n改编用于C#作业使用！特此声明！\r\n\r\n  辅导老师：李灿林\r\n\r\n声明：因版权问题敬请访问个人网站： http://www.xiexianbin.co.co \r\n\r\n                            谢谢合作！！\r\n\r\n    如有雷同，纯属巧合", "版权声明：");   //显示游戏版本及版权说明
        }

        private void 关于扫雷ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("游戏说明：\r\n\r\n  ▲在“游戏”菜单上，单击“开局”;\r\n  ▲要启动计时器，请单击游戏区中的任何方块;\r\n\r\n注意:\r\n\r\n  ▲ 单击就可以揭开方块,如果揭开的是地雷，您便输掉游戏;\r\n  ▲ 如果方块上出现数字，则表示在它周围的八个方块中共有多少颗地雷(0-8)如有都可将用不同的图片标示;\r\n  ▲ 要标记您认为可能有地雷的方块，请右键单击它;\r\n  ▲ 游戏区包括菜单、雷区、地雷计数器、计时器和开始按钮，请按游戏规则游戏！\r\n  ▲本游戏的英雄榜是记录游戏者的成绩使用，因能力有限仅支持初中高级的游戏记录。并设置了重置功能，方便玩家。\r\n\r\n   谢谢您对本游戏的支持 小谢 祝 您游戏愉快！", "游戏说明：");   //游戏的使用说明，简介游戏使用问题
        }

        private void 扫雷英雄榜ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form herof = new Form3();   //定义新的窗体，通过关联调用
            herof.Show();   //显示扫雷英雄棒的窗体
        }
    }
}
