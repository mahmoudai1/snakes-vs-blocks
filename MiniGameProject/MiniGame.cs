using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniGameProject
{
    public partial class MiniGame : Form
    {
        Bitmap off;

        Timer tt = new Timer();

        Random RRnb;            // Randomize of number of blocks
        Random RRxb;            // Randomize of x-axis blocks
        Random RRqc;            // Randomize of color quantity
        Random RRl;             // Randomize of draw line or not (1, 0)
        Random RRlh;            // Randomize of the height of the line
        Random RRsf;            // Randomize of put snake food or not(1, 0)
        Random RRqsf;           // Randomize of snake food quantity

        int SnakeQty = 10;
        int ctTimer = 0;
        int selectedIndex = 0;
        int BlockHit = -1;
        int BlockNumber = -1;
        int TotalSnakeQty = 10;
        int max = 10;
        int gameSpeed = 28;
        int selectedColorIndex = -1;

        bool RightClicked = false;
        bool LeftClicked = false;
        bool MotionOn = false;
        bool isHit = false;
        bool isLost = false;
        bool StartPage = true;
        
        
        class ChooseColor
        {
            public int X;
            public int Y;
            public int W;
            public int H;
            public Color clr;
        }

        class Snake
        {
            public int X;
            public int Y;
            public int W;
            public int H;
            public Color clr;
        }

        class Block
        {
            public int X;
            public int Y;
            public int W;
            public int H;
            public int qty;
            public Color clr;
            public List<Line> LLine = new List<Line>();
        }

        class Line
        {
            public int X;
            public int Y;
            public int W;
            public int H;
        }

        class SnakeFood
        {
            public int X;
            public int Y;
            public int W;
            public int H;
            public int qty;
            public Color clr;
        }

        List<Snake> LS = new List<Snake>();
        List<Block> LB = new List<Block>();
        List<SnakeFood> LSF = new List<SnakeFood>();
        List<ChooseColor> LCC = new List<ChooseColor>();

        public MiniGame()
        {
            InitializeComponent();

            this.Paint += new PaintEventHandler(MiniGame_Paint);
            this.MouseDown += new MouseEventHandler(MiniGame_MouseDown);
            this.KeyDown += new KeyEventHandler(MiniGame_KeyDown);
            this.KeyUp += new KeyEventHandler(MiniGame_KeyUp);
            this.Text = "MiniGame - Snakes vs Blocks";

            tt.Tick += Tt_Tick;
            tt.Start();
        }

        private void MiniGame_MouseDown(object sender, MouseEventArgs e)
        {
            if(StartPage)
            {
                for(int i = 0; i < LCC.Count; i++)
                {
                    if(e.X >= LCC[i].X && e.X <= LCC[i].X + LCC[i].W
                        && e.Y >= LCC[i].Y && e.Y <= LCC[i].Y + LCC[i].H)
                    {
                        selectedColorIndex = i;
                        StartPage = false;
                        CreateSnake();
                        ctTimer = 0;
                        break;
                    }
                }
            }
        }

        private void MiniGame_Load(object sender, EventArgs e)
        {
            off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
        }

        private void MiniGame_Paint(object sender, PaintEventArgs e)
        {
            DrawDouble(e.Graphics);
        }

        private void MiniGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (!StartPage)
            {
                if (e.KeyCode == Keys.Right && !MotionOn && LS[0].X < this.ClientSize.Width - 150)
                {
                    LeftClicked = false;
                    selectedIndex = 0;
                    if (RightisOk())
                    {
                        RightClicked = true;
                    }
                    else
                    {
                        RightClicked = false;
                    }
                    ArrangeSnake();
                }
                if (LS[0].X > 100)
                {
                    if (e.KeyCode == Keys.Left && !MotionOn)
                    {
                        RightClicked = false;
                        selectedIndex = 0;
                        if (LeftisOk())
                        {
                            LeftClicked = true;
                        }
                        else
                        {
                            LeftClicked = false;
                        }
                        ArrangeSnake();
                    }
                }
            }
        }

        private void MiniGame_KeyUp(object sender, KeyEventArgs e)
        {
           
        }

        private void Tt_Tick(object sender, EventArgs e)
        {
            /*RR4 = new Random();
            int vRR4;
            for (; ; )
            {
                vRR4 = RR4.Next(29, 51);
                if (vRR4 == 30 || vRR4 == 40 || vRR4 == 50)
                {
                    break;
                }
            }*/
            if (!StartPage)
            {
               

                if (!isHit)
                {
                    MoveBlocks();

                    if (ctTimer % 21 == 0)
                    {
                        CreateBlocks();
                    }
                    ctTimer++;
                }

               

                checkHeadHit();
                checkFoodTaken();

                if (isHit)
                {
                    DecreaseBlockQty();
                    DecreaseSnakeQty();
                    ChangeBlockColor();
                    ArrangeSnake();
                }

                if (RightClicked)
                {
                    RightMotion();
                    selectedIndex++;
                    //MotionOn = true;
                    if (selectedIndex >= 6 || selectedIndex >= SnakeQty)
                    {
                        ArrangeSnake();
                        selectedIndex = 0;
                        RightClicked = false;
                        MotionOn = false;
                    }
                }
                else if (LeftClicked)
                {
                    LeftMotion();
                    selectedIndex++;
                    //MotionOn = true;
                    if (selectedIndex >= 6 || selectedIndex >= SnakeQty)
                    {
                        ArrangeSnake();
                        selectedIndex = 0;
                        LeftClicked = false;
                        MotionOn = false;
                    }
                }

            }
            else
            {
                CreateStartPage();
            }

            DrawDouble(this.CreateGraphics());
        }

        void CreateSnake()
        {
            int ay = this.ClientSize.Height - 300;
            for (int i = 0; i < SnakeQty; i++)
            {
                Snake pnn = new Snake();
                pnn.X = 530;
                pnn.Y = ay;
                pnn.W = 40;
                pnn.H = 40;
                pnn.clr = LCC[selectedColorIndex].clr;
                LS.Add(pnn);
                ay += pnn.H;
            }
        }

        void CreateBlocks()
        {
            RRnb = new Random();
            RRxb = new Random();
            RRqc = new Random();
            RRsf = new Random();
            RRl = new Random();
            RRlh = new Random();
            RRqsf = new Random();

            int vRRnb = RRnb.Next(3, 9);
            for (int i = 0; i < vRRnb; i++)
            {
                Block pnn = new Block();
                
                int vRRx = 0;

                for (; ; )
                {
                    vRRx = RRxb.Next(0, this.ClientSize.Width - 100);
                    if (CollideBlocks(vRRx))
                    {
                        break;
                    }
                }

               
                pnn.X = vRRx;
                pnn.Y = -100;
                pnn.W = 100;
                pnn.H = 100;

                
                if(RRsf.Next(0, 50) % 6 == 0)
                {
                    SnakeFood pnn2 = new SnakeFood();
                    pnn2.X = pnn.X + (pnn.W / 2) - 20;
                    pnn2.Y = pnn.Y + pnn.H + 50;
                    pnn2.W = 40;
                    pnn2.H = 40;
                    pnn2.clr = LS[0].clr;
                    pnn2.qty = RRqsf.Next(1, 8);
                    LSF.Add(pnn2);
                }

                
                pnn.qty = RRqc.Next(1, 45);

                if(pnn.qty < 15)
                {
                    pnn.clr = Color.FromArgb(2 * pnn.qty, 2 * pnn.qty, 255 - (2 * pnn.qty));
                }
                else if(pnn.qty < 30 && pnn.qty >= 15)
                {
                    pnn.clr = Color.FromArgb(0, 255 - (4 * pnn.qty), 0);
                }
                else
                {
                    pnn.clr = Color.FromArgb(255 - (4 * pnn.qty), 1 * pnn.qty, 1 * pnn.qty);
                }

                
                LB.Add(pnn);
                BlockNumber++;


                int ax = pnn.X;
                if (RRl.Next(0, 30) % 3 == 0)
                {
                    for (int j = 0; j < RRl.Next(1, 2); j++)
                    {
                        Line pnn2 = new Line();
                        pnn2.X = ax;
                        pnn2.Y = pnn.Y + pnn.H;
                        pnn2.W = 5;
                        pnn2.H = RRlh.Next(200, 300);
                        LB[BlockNumber].LLine.Add(pnn2);
                        ax = pnn.X + pnn.W - 5;
                    }
                }
            }
        }

        bool CollideBlocks(int vRR2)
        {
            if (vRR2 % 100 == 0)
            {
                for (int i = 0; i < LB.Count; i++)
                {
                    if(vRR2 == LB[i].X && LB[i].Y == -100)
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        void MoveBlocks()
        {
            for(int i = 0; i < LB.Count; i++)
            {
                LB[i].Y += gameSpeed;
                for(int j = 0; j < LB[i].LLine.Count; j++)
                {
                    LB[i].LLine[j].Y += gameSpeed;
                }
            }

            for (int j = 0; j < LSF.Count; j++)
            {
                LSF[j].Y += gameSpeed;
            }

            for (int i = 0; i < LB.Count; i++)
            {
                if (LB[i].Y > LS[0].Y)
                {
                    LB.Remove(LB[i]);
                    BlockNumber--;
                    for (int j = 0; j < LSF.Count; j++)
                    {
                        if (LSF[j].Y - 50 > LS[0].Y)
                        {
                            LSF.Remove(LSF[j]);
                        }
                    }

                }
            }

           


        }

        void RightMotion()
        {
            if (selectedIndex >= 0)
            {
                if (selectedIndex == 0)
                {
                    LS[selectedIndex].X += 100;
                }
                else
                {
                    LS[selectedIndex].X = LS[0].X;
                }
            }
        }

        void LeftMotion()
        {
            if (selectedIndex >= 0)
            {
                if (selectedIndex == 0)
                {
                    LS[selectedIndex].X -= 100;
                }
                else
                {
                    LS[selectedIndex].X = LS[0].X;
                }
            }
        }

        void checkHeadHit()
        {
            for (int i = 0; i < LB.Count; i++)
            {
                if (LS[0].Y <= LB[i].Y + LB[i].H && LS[0].Y > LB[i].Y && LS[0].X > LB[i].X && LS[0].X < LB[i].X + LB[i].W)
                {
                    isHit = true;
                    BlockHit = i;
                    break;
                }
                else
                {
                    isHit = false;
                    
                }
            }
        }

        void checkFoodTaken()
        {
            for (int i = 0; i < LSF.Count; i++)
            {
                if (LS[0].Y <= LSF[i].Y + LSF[i].H && LS[0].Y > LSF[i].Y && LS[0].X >= LSF[i].X && LS[0].X + LS[0].W <= LSF[i].X + LSF[i].W)
                {
                    SnakeQty += LSF[i].qty;
                    TotalSnakeQty = SnakeQty;
                    if(TotalSnakeQty > max)
                    {
                        max = TotalSnakeQty;
                    }
                    int ay = LS[LS.Count - 1].Y + 40;
                    for(int j = 0; j < LSF[i].qty; j++)
                    {
                        Snake pnn = new Snake();
                        pnn.X = LS[0].X;
                        pnn.W = 40;
                        pnn.H = 40;
                        pnn.Y = ay;
                        pnn.clr = LCC[selectedColorIndex].clr;
                        LS.Add(pnn);
                        ay += 40;
                    }
                    LSF.Remove(LSF[i]);
                    break;
                }
            }
        }

        bool RightisOk()
        {
            for (int i = 0; i < LS.Count; i++)
            {
                for (int j = 0; j < LB.Count; j++)
                {
                    if (LS[i].X + 100 > LB[j].X && LS[i].X + 100 < LB[j].X + LB[j].W &&
                        LS[i].Y > LB[j].Y && LS[i].Y < LB[j].Y + LB[j].H - gameSpeed)
                    {
                        return false;
                    }
                }

                for (int j = 0; j < LB.Count; j++)
                {
                    for(int k = 0; k < LB[j].LLine.Count; k++)
                    {
                        if (LS[i].X + 70 >= LB[j].LLine[k].X && LS[i].X + 70 <= LB[j].LLine[k].X + LB[j].LLine[k].W &&
                        LS[i].Y >= LB[j].LLine[k].Y && LS[i].Y <= LB[j].LLine[k].Y + LB[j].LLine[k].H)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        bool LeftisOk()
        {
            for (int i = 0; i < LS.Count; i++)
            {
                for (int j = 0; j < LB.Count; j++)
                {
                    if (LS[i].X - 100 > LB[j].X && LS[i].X - 100 < LB[j].X + LB[j].W &&
                        LS[i].Y > LB[j].Y && LS[i].Y < LB[j].Y + LB[j].H - gameSpeed)
                    {
                        return false;
                    }
                }

                for (int j = 0; j < LB.Count; j++)
                {
                    for (int k = 0; k < LB[j].LLine.Count; k++)
                    {
                        if (LS[i].X - 25 >= LB[j].LLine[k].X && LS[i].X - 25 <= LB[j].LLine[k].X + LB[j].LLine[k].W &&
                        LS[i].Y >= LB[j].LLine[k].Y && LS[i].Y <= LB[j].LLine[k].Y + LB[j].LLine[k].H)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        void DecreaseBlockQty()
        {
            LB[BlockHit].qty--;
            if(LB[BlockHit].qty == 0)
            {
                LB.Remove(LB[BlockHit]);
                BlockNumber--;
            }
        }
        
        void DecreaseSnakeQty()
        {
            LS.RemoveAt(LS.Count-1);
            SnakeQty--;
            if(SnakeQty == 0)
            {
                StartPage = true;
                isLost = true;
                TotalSnakeQty = max;
                SnakeQty = 10;

                 SnakeQty = 10;
                 ctTimer = 0;
                 selectedIndex = 0;
                 BlockHit = -1;
                 BlockNumber = -1;
                 gameSpeed = 28;

                 RightClicked = false;
                 LeftClicked = false;
                 MotionOn = false;
                 isHit = false;
                 selectedColorIndex = -1;

                LCC.Clear();
                LSF.Clear();
                LB.Clear();
                LS.Clear();
            }
        }

        void ChangeBlockColor()
        {
            if (BlockHit > 0)
            {
                if (LB[BlockHit].qty < 15 && LB[BlockHit].qty > 0)
                {
                    LB[BlockHit].clr = Color.FromArgb(2 * LB[BlockHit].qty, 2 * LB[BlockHit].qty, 255 - (2 * LB[BlockHit].qty));
                }
                else if (LB[BlockHit].qty < 30 && LB[BlockHit].qty >= 15)
                {
                    LB[BlockHit].clr = Color.FromArgb(0, 255 - (4 * LB[BlockHit].qty), 0);
                }
                else
                {
                    LB[BlockHit].clr = Color.FromArgb(255 - (4 * LB[BlockHit].qty), 1 * LB[BlockHit].qty, 1 * LB[BlockHit].qty);
                }
            }
        }

        void ArrangeSnake()
        {
            for(int i = selectedIndex; i < LS.Count; i++)
            {
                LS[i].X = LS[0].X;
            }
        }

        void CreateStartPage()
        {
            int ax = 50;
            int ay = 300;
            string[] clrsArray = {"Yellow" , "Chartreuse", "Green", "SpringGreen", "Cyan", "Azure", "LightBlue",  "Blue", "BlueViolet", "Violet",  "Magenta", "MistyRose", "Orange", "Brown", "Red"};
           
            for (int i = 0; i < 15; i++)
            {
                ChooseColor pnn = new ChooseColor();
                pnn.X = ax;
                pnn.Y = ay;
                pnn.W = 60;
                pnn.H = 60;
                pnn.clr = Color.FromName(clrsArray[i]);
                LCC.Add(pnn);
                ax += 160;
                if(ax >= this.ClientSize.Width - 30)
                {
                    ax = 50;
                    ay += 200;
                }
            }
            
        }

        void DrawScene(Graphics g)
        {
            g.Clear(Color.Black);

            if (!StartPage)
            {
                for (int i = 0; i < LS.Count; i++)
                {
                    SolidBrush SnakeColor = new SolidBrush(LS[i].clr);
                    g.FillEllipse(SnakeColor, LS[i].X, LS[i].Y, LS[i].W, LS[i].H);
                    g.DrawString("⬆", new Font("Arial", 20, FontStyle.Bold), Brushes.Black, LS[0].X - 15, LS[0].Y - 10);
                    
                }

                for (int i = 0; i < LB.Count; i++)
                {
                    SolidBrush BlockColor = new SolidBrush(LB[i].clr);
                    g.FillRectangle(BlockColor, LB[i].X, LB[i].Y, LB[i].W, LB[i].H);
                    g.DrawRectangle(Pens.White, LB[i].X, LB[i].Y, LB[i].W, LB[i].H);

                    g.DrawString(LB[i].qty + "", new Font("Arial", 14, FontStyle.Bold), Brushes.White, LB[i].X + (LB[i].W / 4), LB[i].Y + (LB[i].H / 4));

                    for (int j = 0; j < LB[i].LLine.Count; j++)
                    {
                        g.FillRectangle(Brushes.White, LB[i].LLine[j].X, LB[i].LLine[j].Y, LB[i].LLine[j].W, LB[i].LLine[j].H);
                    }
                }


                for (int i = 0; i < LSF.Count; i++)
                {
                    SolidBrush SnakeFoodColor = new SolidBrush(LSF[i].clr);
                    g.FillEllipse(SnakeFoodColor, LSF[i].X, LSF[i].Y, LSF[i].W, LSF[i].H);

                    g.DrawString(LSF[i].qty + "", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, LSF[i].X + 6, LSF[i].Y + 3);
                }

                SolidBrush SnakeColorScore = new SolidBrush(LS[0].clr);
                g.DrawString(SnakeQty + "", new Font("Arial", 14, FontStyle.Bold), SnakeColorScore, 15, this.ClientSize.Height - 50);
                g.DrawString(max + "", new Font("Arial", 14, FontStyle.Bold), Brushes.White, this.ClientSize.Width - 70, this.ClientSize.Height - 50);
            }
            else
            {
                g.DrawString("Snakes vs Blocks", new Font("Gadugi", 26, FontStyle.Bold), Brushes.White, 120, 30);
                g.DrawString("Select your color to play", new Font("Gadugi", 13, FontStyle.Regular), Brushes.White, 190, 140);

                for(int i = 0; i < LCC.Count; i++)
                {
                    SolidBrush SnakeStartColor = new SolidBrush(LCC[i].clr);
                    g.FillEllipse(SnakeStartColor, LCC[i].X, LCC[i].Y, LCC[i].W, LCC[i].H);
                }
            }

        }

        void DrawDouble(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }

    }
}
