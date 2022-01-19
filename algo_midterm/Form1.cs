/* $LAN=C#$ */
/*
	Name: Form1.cs
	Copyright: Copyright © 2021
	Author:簡志軒
    Student ID: M103040069
    Class: 資工碩一
	Date: 2021/12/28
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;

namespace algo_midterm
{
    public partial class Form1 : Form
    {
        List<PointF>  plist = new List<PointF>();
        List<float> filedata = new List<float>();
        Voronoi voronoi=new Voronoi();
        Bitmap bitmap;
        int  counter = 0,status=0;
        ManualResetEvent autoawait = new ManualResetEvent(false);
        
        //ThreadStart tstart = new ThreadStart(a);

        public Form1()
        {
            InitializeComponent();

            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);


        }
        
        //點擊畫布
        private void addpoint(object sender, MouseEventArgs e)
        {
            PointF p = new PointF(e.X, e.Y);
            //AddPointF(p);
            DrawPointF(p.X,p.Y, Color.DarkRed);
            plist.Add(p);
            //voronoi.plist.Add(p);
            listBox1.Items.Add("(" + e.X + ", " + e.Y + ")");
        }
        //畫點
        public void DrawPointF(float X, float Y, Color color)
        {
            Graphics graphic = Graphics.FromImage(bitmap);
            Brush brush = new SolidBrush(color);
            RectangleF r = new RectangleF(X -2, Y -2, 5, 5);
            graphic.SmoothingMode = SmoothingMode.AntiAlias;
            graphic.FillEllipse(brush, r);
            pictureBox1.Image = bitmap;
            /*if (plist.Count() >=2)
            {
                DrawLine(plist[(int)plist.Count()-2], plist[(int)plist.Count-1],Color.DarkBlue);
            }*/
        }
        //畫線
        public void DrawLine(PointF A, PointF B, Color color)
        {
            //bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics graphic = Graphics.FromImage(bitmap);
            Pen pen = new Pen(color);
            graphic.SmoothingMode = SmoothingMode.AntiAlias;
            graphic.DrawLine(pen, A, B);
            pictureBox1.Image = bitmap;
        }
        //畫出plist所有點
        private void drawlistpoint(List<PointF> plist)
        {
            foreach(PointF p in plist)
            {
                DrawPointF(p.X, p.Y, Color.DarkRed);
               // listBox1.Items.Add("(" + p.X + ", " + p.Y + ")");
            }
        }
        //清除畫布、plist
        private void Clear()
        {

            Graphics graphic = Graphics.FromImage(bitmap);
            graphic.Clear(pictureBox1.BackColor);
            pictureBox1.Image = bitmap;
            plist.Clear();
            voronoi.plist.Clear();
            voronoi.elist.Clear();
            voronoi.hyperplane.Clear();
            voronoi.convexhull = new Convexhull();
            listBox1.Items.Clear();
        }
        private void Clearbitmap()
        {
            
            Graphics graphic = Graphics.FromImage(bitmap);
            graphic.Clear(pictureBox1.BackColor);
            pictureBox1.Image = bitmap;
         
        }
        //清除按鈕c
        private void clearpaint(object sender, EventArgs e)
        {

            
            //Clear();
            if(status == 2)
            {
                status = 0;
                autoawait.Set();

            }
            else
            {
                Clear();
            }
            
           

        }
        //手動輸入點
        private void typepoint(object sender, EventArgs e)
        {

            if (Regex.IsMatch(xaxis.Text, @"^[-+]?[0-9]+(\.[0-9]{1,3})?$") && Regex.IsMatch(yaxis.Text, @"^[-+]?[0-9]+(\.[0-9]{1,3})?$"))
            {
                PointF p = new PointF();
                p.X = Convert.ToSingle(xaxis.Text);
                p.Y = Convert.ToSingle(yaxis.Text);
                DrawPointF(p.X, p.Y, Color.DarkRed);
                listBox1.Items.Add("(" + p.X + ", " + p.Y + ")");
                plist.Add(p);
            }
            else
            {
                MessageBox.Show("請輸入小數後三位內的浮點數", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }
        //檔案的下一個case
        private void nextclick(object sender, EventArgs e)
        {
            
            if (filedata.Count != 0)
            {
                Clear();
                readdata();
            }
        }

        //按step
        private void stepclick(object sender, EventArgs e)
        {                 
            if (status == 0)
            {
                status = 2;
                Thread t = new Thread(() => test());
                t.Start();

            }
            else
            {
                status = 2;
                autoawait.Set();
            }
            
        }
        //按run
        private  void runclick(object sender, EventArgs e)
        {
            
            status = 0;
            autoawait.Set();
            if (status != 2)
            {
                
                test();
            }
            
            
        }
        public void test() {

            voronoi.plist = plist;
            //砍掉重複的點
            voronoi.killreapet();
            voronoi.sortplist();
            if (voronoi.plist.Count == 2)
            {
                voronoi.elist = twopoint(voronoi);
                for (int i = 0; i < voronoi.elist.Count; i++)
                {
                    DrawLine(voronoi.elist[i].A, voronoi.elist[i].B, Color.Green);
                }
            }
            else if (voronoi.plist.Count > 2)
            {
                //threepoint(voronoi);
                voronoi = dividevoronoi(voronoi);
                //voronoi.convexhull.convexlist = voronoi.plist;
                //voronoi.convexhull.sortconvexhull();
                Clearbitmap();
                drawlistpoint(plist);
                for (int i = 0; i < voronoi.elist.Count; i++)
                {
                    DrawLine(voronoi.elist[i].A, voronoi.elist[i].B, Color.Green);
                }
                /*for (int i = 0; i < voronoi.convexhull.convexlist.Count(); i++)
                {
                    DrawLine(voronoi.convexhull.convexlist[i], voronoi.convexhull.convexlist[(i + 1) % voronoi.convexhull.convexlist.Count], Color.DarkOrange);
                }*/
                /*foreach (Edge e in voronoi.hyperplane)
                {
                    DrawLine(e.A, e.B, Color.Blue);
                }*/
            }
            else
            {

            }
            
            status = 0;
            
        }
        #region 工具
        //外積
        public bool cross(PointF a, PointF b,PointF c)
        {
            PointF p1 = new PointF(a.X - c.X, a.Y - c.Y);
            PointF p2 = new PointF(b.X - c.X, b.Y - c.Y);
            float temp = (p1.X * p2.Y) - (p1.Y * p2.X);
            if (temp > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //順時針排三點
        public List<PointF> sortthreepoint(List<PointF> plist)
        {
            PointF center = new PointF(0, 0);
            for(int i = 0; i < plist.Count; i++)
            {
                center.X = plist[i].X + center.X;
                center.Y = plist[i].Y + center.Y;
            }
            center.X /= plist.Count;
            center.Y /= plist.Count;
            for(int i = 0; i < plist.Count - 1; i++)
            {
                for (int j = 0; j < plist.Count - i - 1; j++)
                {
                    if (!cross(plist[j], plist[j + 1],center))
                    {
                        PointF tmp = new PointF();
                        tmp = plist[j + 1];
                        plist[j + 1] = plist[j];
                        plist[j] = tmp;
                    }
                }
            }
            return plist;

        }
        //根據克拉默法則找外心
        public PointF circumcenter(PointF a,PointF b,PointF c)
        {
            PointF ans = new PointF();
            double x1 = a.X, x2 = b.X, x3 = c.X, y1 = a.Y, y2 = b.Y, y3 = c.Y;
            double A1 = 2 * (x2 - x1);
            double B1 = 2 * (y2 - y1);
            double C1 = Math.Pow(x2,2) + Math.Pow(y2, 2) - Math.Pow(x1, 2) - Math.Pow(y1, 2);
            double A2 = 2 * (x3 - x2);
            double B2 = 2 * (y3 - y2);
            double C2 = Math.Pow(x3, 2) + Math.Pow(y3, 2) - Math.Pow(x2, 2) - Math.Pow(y2, 2);
            ans.X = (float)(((C1 * B2) - (C2 * B1)) / ((A1 * B2) - (A2 * B1)));
            ans.Y = (float)(((A1 * C2) - (A2 * C1)) / ((A1 * B2) - (A2 * B1)));
            return ans;

        }
        //判斷切線是否穿過convexhull
        public bool orientation(PointF a, PointF b, PointF c)
         {
            PointF p1 = new PointF(b.X - a.X, b.Y - a.Y);
            PointF p2 = new PointF(c.X - b.X, c.Y - b.Y);
            float temp = (p1.X * p2.Y) - (p1.Y * p2.X);
            if (temp > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
         }
        #endregion

        #region 最小voronoi
        //兩個點vonoroi
        public List<Edge> twopoint(Voronoi voronoi)
        {
            if(voronoi.plist[0].X==voronoi.plist[1].X && voronoi.plist[0].Y == voronoi.plist[1].Y)
            {
                int i=0;

            }
            else
            {

                Edge e = new Edge(voronoi.plist[0], voronoi.plist[1]);
                //e.perpendicular();
                //e.midpoint.X -= e.xvector * 600;
                //e.midpoint.Y -= e.yvector * 600;
                PointF nagativeside =new PointF( e.midpoint.X - e.xvector * 600, e.midpoint.Y - e.yvector * 600);
                //DrawLine(e.midpoint, e.perpendicularpoint, Color.DarkBlue);
                Edge e2 = new Edge(nagativeside, e.perpendicularpoint);
                if (voronoi.plist[0].Y >= voronoi.plist[1].Y)
                {
                    e2.up = voronoi.plist[0];
                    e2.down = voronoi.plist[1];
                }
                else
                {
                    e2.up = voronoi.plist[1];
                    e2.down = voronoi.plist[0]; ;
                }
                voronoi.elist.Add(e2);
                
                //DrawPointF(e.midpoint.X, e.midpoint.Y, Color.DarkBlue);
                //DrawPointF(e.perpendicularpoint.X, e.perpendicularpoint.Y, Color.DarkBlue);
            }
            return voronoi.elist;
        }
        //3個點voronoi
        public List<Edge> threepoint(Voronoi voronoi)
        {   

            if(voronoi.plist[0]== voronoi.plist[1]&& voronoi.plist[1]== voronoi.plist[2])
            {

            }
            else if (voronoi.onthesameline(voronoi))
            {
                Edge e1 = new Edge(voronoi.plist[0], voronoi.plist[1]);
                Edge e2 = new Edge(voronoi.plist[1], voronoi.plist[2]);
                //e1.perpendicular();
                e1.midpoint.X -= e1.xvector * 600;
                e1.midpoint.Y -= e1.yvector * 600;
                e1 = new Edge(e1.midpoint, e1.perpendicularpoint);
                if (voronoi.plist[0].Y >= voronoi.plist[1].Y)
                {
                    e1.up = voronoi.plist[0];
                    e1.down = voronoi.plist[1];
                }
                else
                {
                    e1.up = voronoi.plist[1];
                    e1.down = voronoi.plist[0]; ;
                }
                //e2.perpendicular();
                e2.midpoint.X -= e2.xvector * 600;
                e2.midpoint.Y -= e2.yvector * 600;
                e2 = new Edge(e2.midpoint, e2.perpendicularpoint);
                if (voronoi.plist[1].Y >= voronoi.plist[2].Y)
                {
                    e1.up = voronoi.plist[1];
                    e1.down = voronoi.plist[2];
                }
                else
                {
                    e1.up = voronoi.plist[2];
                    e1.down = voronoi.plist[1]; ;
                }
                DrawLine(e1.A, e1.B, Color.DarkBlue);
                DrawLine(e2.A, e2.B, Color.DarkBlue);
                voronoi.elist.Add(e1);
                voronoi.elist.Add(e2);

            }
            else
            {
                voronoi.plist = sortthreepoint(voronoi.plist);
                PointF center = circumcenter(voronoi.plist[0], voronoi.plist[1], voronoi.plist[2]);
                for(int i = 0; i < voronoi.plist.Count; i++)
                {
                    Edge e = new Edge(voronoi.plist[i], voronoi.plist[(i + 1) % 3]);
                    //e.perpendicular();
                    Edge e1=new Edge(center, e.perpendicularpoint);
                    if(voronoi.plist[i].Y>= voronoi.plist[(i + 1) % 3].Y)
                    {
                        e1.up = voronoi.plist[i];
                        e1.down = voronoi.plist[(i + 1) % 3];
                    }
                    else
                    {
                        e1.up = voronoi.plist[(i + 1) % 3];
                        e1.down = voronoi.plist[i];
                    }
                    voronoi.elist.Add(e1);
                    //DrawLine(center, e.perpendicularpoint, Color.DarkBlue);

                }

            }

            return voronoi.elist;
        }

        #endregion
        #region merge and devide
        //mergeconvexhull
        public Convexhull mergeconvexhull(List<PointF> plist1, List<PointF> plist2)
        {   //看哪個convexhull在左邊
            
            if (plist1[0].X > plist2[0].X)
            {
                List<PointF> temp = plist1;
                plist1 = plist2;
                plist2 = temp;
            }
            float left = 99999, right = -99999;
            int indexleft = 0,indexright = 0;
            //找左邊convexhull最右邊的點
            for (int i = 0; i < plist1.Count; i++)
            {
                if (plist1[i].X > right)
                {
                    right = plist1[i].X;
                    indexleft = i;
                }
                
            }
            //找右邊convexhull最左邊的點
            for (int i = 0; i < plist2.Count; i++)
            {
                if (plist2[i].X<left)
                {
                    left = plist2[i].X;
                    indexright = i;
                }

            }
            //移動兩個多邊形中間的線
            int upperboundleft = indexleft,upperboundright=indexright;          
            while (orientation(plist1[upperboundleft], plist2[upperboundright], plist2[(upperboundright+  1) % plist2.Count]))
            {
                upperboundright = (upperboundright+ 1) % plist2.Count;
            }
            while (!orientation(plist2[upperboundright], plist1[upperboundleft], plist1[(upperboundleft - 1+ plist1.Count) % plist1.Count]))
            {
                upperboundleft = (upperboundleft - 1+plist1.Count) % plist1.Count;
            }
            while (orientation(plist1[upperboundleft], plist2[upperboundright], plist2[(upperboundright + 1) % plist2.Count]))
            {
                upperboundright = (upperboundright + 1) % plist2.Count;
            }

            int lowerboundleft = indexleft, lowerboundright = indexright;
            int k = 0;
            while (!orientation(plist1[lowerboundleft], plist2[lowerboundright], plist2[(lowerboundright - 1+ plist2.Count) % plist2.Count]))
            {
                lowerboundright = (lowerboundright - 1+ plist2.Count) % plist2.Count;
            }

            while (orientation(plist2[lowerboundright], plist1[lowerboundleft], plist1[(lowerboundleft+1) % plist1.Count]))
            {
                
                lowerboundleft = (lowerboundleft+ plist1.Count + 1) % plist1.Count;
            }
            while (!orientation(plist1[lowerboundleft], plist2[lowerboundright], plist2[(lowerboundright - 1 + plist2.Count) % plist2.Count]))
            {
                k++;
                if (k == plist1.Count)
                {
                    break;
                }
                lowerboundright = (lowerboundright - 1 + plist2.Count) % plist2.Count;
            }
            Convexhull convexhull = new Convexhull();
            //合併並排序點
            List<PointF> mergelist = new List<PointF>(100);
            mergelist.Clear();
            convexhull.uptangentline = new Edge(plist1[upperboundleft], plist2[upperboundright]);
            convexhull.downtangentline = new Edge(plist1[lowerboundleft], plist2[lowerboundright]);
            for(int i = upperboundleft; i != (lowerboundleft ) % plist1.Count; i = (i - 1+ plist1.Count) % plist1.Count)
            {
                mergelist.Add(plist1[i]);
            }
            if (plist1.Count != 2)
            {
                mergelist.Add(plist1[lowerboundleft]);
            }
            
            if (plist1.Count == 2)
            {
                mergelist.Clear();
                mergelist.Add(plist1[upperboundleft]);
                mergelist.Add(plist1[lowerboundleft]);
            }
            if (plist2.Count >= 3)
            {
                for (int i = lowerboundright; i != (upperboundright ) % plist2.Count; i = (i - 1+ plist2.Count) % plist2.Count)
                {
                    mergelist.Add(plist2[i]);
                }
            }
            
            
            if (plist2.Count != 2)
            {
                mergelist.Add(plist2[upperboundright]);
            }
            if (plist2.Count == 2)
            {   
                mergelist.Add(plist2[lowerboundright]);
                mergelist.Add(plist2[upperboundright]);
                
            }

            convexhull.convexlist = mergelist;
            convexhull.subconvexlist1 = sortconvexhsublist(plist1);          
            convexhull.subconvexlist2 = sortconvexhsublist(plist2);
            convexhull.sortconvexhull();
            return convexhull;


        }
        //dividevonoroi
        public Voronoi dividevoronoi(Voronoi voronoi)
        {   
            
            if (voronoi.plist.Count > 3)
            {   
                
                Voronoi voronoi1 = new Voronoi(), voronoi2 = new Voronoi();
                if (voronoi.plist.Count % 2 == 0) { 
                    voronoi1.plist = voronoi.plist.GetRange(0,(voronoi.plist.Count/2));
                    voronoi2.plist = voronoi.plist.GetRange(voronoi.plist.Count / 2, voronoi.plist.Count/2);
                }
                else
                {
                    voronoi1.plist = voronoi.plist.GetRange(0, (voronoi.plist.Count / 2+1));
                    voronoi2.plist = voronoi.plist.GetRange(voronoi.plist.Count / 2+1, (voronoi.plist.Count / 2));
                }
                voronoi1 = dividevoronoi(voronoi1);
                

                if (status == 2)
                {   
                    for (int i = 0; i < voronoi1.elist.Count; i++)
                    {
                        DrawLine(voronoi1.elist[i].A, voronoi1.elist[i].B, Color.Brown);
                    }
                    for (int i = 0; i < voronoi1.convexhull.convexlist.Count(); i++)
                    {
                        DrawLine(voronoi1.convexhull.convexlist[i], voronoi1.convexhull.convexlist[(i + 1) % voronoi1.convexhull.convexlist.Count], Color.DarkOrange);
                    }

                    autoawait.WaitOne();
                    autoawait.Reset();
                }
                voronoi2 =dividevoronoi(voronoi2);

                if (status == 2)
                {   

                    for (int i = 0; i < voronoi2.elist.Count; i++)
                    {
                        DrawLine(voronoi2.elist[i].A, voronoi2.elist[i].B, Color.Green);
                    }
                    for (int i = 0; i < voronoi2.convexhull.convexlist.Count(); i++)
                    {
                        DrawLine(voronoi2.convexhull.convexlist[i], voronoi2.convexhull.convexlist[(i + 1) % voronoi2.convexhull.convexlist.Count], Color.DarkOrange);
                    }
                    autoawait.WaitOne();
                    autoawait.Reset();
                }
                 
                Clearbitmap();
                for (int i = 0; i < voronoi1.elist.Count; i++)
                {
                    DrawLine(voronoi1.elist[i].A, voronoi1.elist[i].B, Color.Green);
                }
                for (int i = 0; i < voronoi2.elist.Count; i++)
                {
                    DrawLine(voronoi2.elist[i].A, voronoi2.elist[i].B, Color.Green);
                }
                voronoi =mergevoronoi(voronoi1, voronoi2);
                drawlistpoint(plist);

                if (status == 2)
                {
                    
                    
                    for (int i = 0; i < voronoi.convexhull.convexlist.Count(); i++)
                    {
                        DrawLine(voronoi.convexhull.convexlist[i], voronoi.convexhull.convexlist[(i + 1) % voronoi.convexhull.convexlist.Count], Color.DarkOrange);
                    }
                    foreach (Edge e in voronoi.hyperplane)
                    {
                        DrawLine(e.A, e.B, Color.Blue);
                    }
                    autoawait.WaitOne();
                    autoawait.Reset();


                    Clearbitmap();
                    drawlistpoint(plist);
                    /*for (int i = 0; i < voronoi.elist.Count; i++)
                    {
                        DrawLine(voronoi.elist[i].A, voronoi.elist[i].B, Color.Green);
                    }
                    for (int i = 0; i < voronoi.convexhull.convexlist.Count(); i++)
                    {
                        DrawLine(voronoi.convexhull.convexlist[i], voronoi.convexhull.convexlist[(i + 1) % voronoi.convexhull.convexlist.Count], Color.DarkOrange);
                    }*/
                    
                    /*autoawait.WaitOne();
                    autoawait.Reset();*/
                }
                if (status == 2)
                {
                    
                }


            }
            else
            {
                if (voronoi.plist.Count == 2)
                {
                    
                    voronoi.elist = twopoint(voronoi);
                    voronoi.convexhull.convexlist = voronoi.plist;
                    voronoi.convexhull.sortconvexhull();

                }
                else if (voronoi.plist.Count ==3)
                {
                    voronoi.elist= threepoint(voronoi);
                    voronoi.convexhull.convexlist = voronoi.plist;
                    voronoi.convexhull.sortconvexhull();
                }
                else
                {
                    voronoi.convexhull.convexlist = voronoi.plist;
                }
            }
            return voronoi;
        }
        //mergevoronoi
        public Voronoi mergevoronoi(Voronoi voronoi1,Voronoi voronoi2)
        {
            Voronoi voronoi=new Voronoi();
            voronoi.convexhull = mergeconvexhull(voronoi1.convexhull.convexlist, voronoi2.convexhull.convexlist);
            /*voronoi.subplist1 = voronoi1.plist;
            voronoi.subplist2 = voronoi2.plist;*/
            voronoi.plist.AddRange(voronoi1.plist);voronoi.plist.AddRange(voronoi2.plist);          
            voronoi.subelistleft=voronoi1.elist;
            voronoi.subelistright=voronoi2.elist;
            voronoi.convexhull.subconvexlist1 = sortconvexhsublist(voronoi.convexhull.subconvexlist1);
            voronoi.convexhull.subconvexlist2 = sortconvexhsublist(voronoi.convexhull.subconvexlist2);
            voronoi = findhyperplane(voronoi);
            voronoi.elist.AddRange(voronoi.subelistleft); voronoi.elist.AddRange(voronoi.subelistright);
            voronoi.elist.AddRange(voronoi.hyperplane);
            voronoi.subelistleft=voronoi1.elist;
            voronoi.subelistright=voronoi2.elist;



            return voronoi;
        }
        #endregion
        //找hyperplane
        public Voronoi findhyperplane(Voronoi voronoi)
        {
            
            List<Edge> hyperplane = new List<Edge>();
            List<int> alredyleft = new List<int>();
            List<int> alredyright = new List<int>();
            Edge uptangent = voronoi.convexhull.uptangentline;
            Edge downtangent = voronoi.convexhull.downtangentline;
            PointF startpoint = new PointF();
      
            PointF currentpoint = new PointF(600, 600);
            int uptengentrightindex=0, uptengentleftindex=0;
            for (int i = 0; i < voronoi.convexhull.subconvexlist1.Count; i++)
            {
                if (voronoi.convexhull.uptangentline.A.Equals(voronoi.convexhull.subconvexlist1[i])){
                    uptengentleftindex = i;
                }
            }
            for (int i = 0; i < voronoi.convexhull.subconvexlist2.Count; i++)
            {
                if (voronoi.convexhull.uptangentline.B.Equals(voronoi.convexhull.subconvexlist2[i])){
                    uptengentrightindex = i;
                }
            }
            PointF rightscan = uptangent.B;
            PointF leftscan = uptangent.A;
            voronoi.subelistleft=sortelist(voronoi.subelistleft);
            voronoi.subelistright=sortelist(voronoi.subelistright);
            int flag = -1;
            startpoint.X=uptangent.midpoint.X+ (((1000 - uptangent.midpoint.Y) / uptangent.yvector) * uptangent.xvector);
            //startpoint.Y = uptangent.midpoint.Y;
            startpoint.Y = 1000;
            int whichedge=0;
            currentpoint = startpoint;
            PointF start = currentpoint;
            Edge currentline = voronoi.convexhull.uptangentline;
            while (currentpoint.Y>-100)
            {
                int doubleintersect = 0;
                int leftedge = 0, rightedge = 0;
                start.Y = -900;
                flag = 0;
                for (int i=0;i < voronoi.subelistleft.Count;i++)
                {
                    Edge e1 = voronoi.subelistleft[i];
                    double d1 = (double)GetIntersection(e1.A, e1.B, currentpoint, currentline.perpendicularpoint).X;
                    double d2 = (double)GetIntersection(e1.A, e1.B, currentpoint, currentline.perpendicularpoint).Y;
                    if(d1== 99999&& d2==99999)
                    {
                        continue;
                    }
                    d1 = Math.Round(d1, 6, MidpointRounding.ToEven);
                    d2 = Math.Round(d2, 6, MidpointRounding.ToEven);
                    
                    //PointF t = new PointF((float)d1, (float)d2);
                    PointF t= getInterHPoint(e1, new Edge(currentpoint, currentline.perpendicularpoint));
                    if (t.Y > start.Y && t.Y < currentpoint.Y &&(t.X<e1.B.X&&t.X>e1.A.X))
                    {
                        flag = 1;
                        start = t;
                        whichedge = i;
                        leftedge = i;
                    }
                }
                for (int i = 0; i < voronoi.subelistright.Count; i++)
                {
                    Edge e1 = voronoi.subelistright[i];
                    double d1 = (double)GetIntersection(e1.A, e1.B, currentpoint, currentline.perpendicularpoint).X; 
                    double d2 = (double)GetIntersection(e1.A, e1.B, currentpoint, currentline.perpendicularpoint).Y;
                    if (d1 == 99999 && d2 == 99999)
                    {
                        continue;
                    }
                    d1 = Math.Round(d1, 6, MidpointRounding.ToEven);
                    d2= Math.Round(d2, 6, MidpointRounding.ToEven);
                    //PointF t = new PointF((float)d1, (float)d2);
                    PointF t = getInterHPoint(e1, new Edge(currentpoint, currentline.perpendicularpoint));
                    
                    if (t.Equals(start) && t.Y < currentpoint.Y && (t.X > e1.A.X && t.X < e1.B.X))
                    {
                        doubleintersect = 1;
                        rightedge = i;
                    }
                    if (t.Y > start.Y && t.Y < currentpoint.Y && (t.X > e1.A.X&&t.X<e1.B.X))
                    {
                        flag = 2;
                        start = t;
                        whichedge = i;
                        doubleintersect = 0;
                    }
                }
                if (start.Y == -900)
                {
                    hyperplane.Add(new Edge(currentpoint, currentline.perpendicularpoint));
                }
                else
                {
                    hyperplane.Add(new Edge(currentpoint, start));
                    if (leftscan.Y > rightscan.Y)
                    {
                        hyperplane[hyperplane.Count - 1].up = leftscan;
                        hyperplane[hyperplane.Count - 1].down = rightscan;
                    }
                    else
                    {
                        hyperplane[hyperplane.Count - 1].up = leftscan;
                        hyperplane[hyperplane.Count - 1].down = rightscan;
                    }
                    

                }
                //交兩條線
                if (doubleintersect == 0)
                {
                    if (flag == 1)
                    {
                        if (Math.Sqrt((Math.Pow((currentpoint.X - voronoi.subelistleft[whichedge].up.X), 2) + Math.Pow((currentpoint.Y - voronoi.subelistleft[whichedge].up.Y), 2))) > Math.Sqrt((Math.Pow((currentpoint.X - voronoi.subelistleft[whichedge].down.X), 2) + Math.Pow((currentpoint.Y - voronoi.subelistleft[whichedge].down.Y), 2))))
                        {
                            leftscan = voronoi.subelistleft[whichedge].up;
                            currentline.A = leftscan;
                            currentline.perpendicular();
                            if (cross(currentpoint, voronoi.subelistleft[whichedge].B, start))
                            {
                                voronoi.subelistleft[whichedge].A = start;
                            }
                            else
                            {
                                voronoi.subelistleft[whichedge].B = start;
                            }
                            //voronoi.subelistleft[whichedge].B = start;
                            alredyleft.Add(whichedge);
                        }
                        else
                        {
                            leftscan = voronoi.subelistleft[whichedge].down;
                            currentline.A = leftscan;
                            currentline.perpendicular();
                            if (cross(currentpoint, voronoi.subelistleft[whichedge].B, start))
                            {
                                voronoi.subelistleft[whichedge].A = start;
                            }
                            else
                            {
                                voronoi.subelistleft[whichedge].B = start;
                            }
                            //voronoi.subelistleft[whichedge].B = start;
                            alredyleft.Add(whichedge);
                        }
                    }
                    else if (flag == 2)
                    {
                        if (Math.Sqrt((Math.Pow((currentpoint.X - voronoi.subelistright[whichedge].up.X), 2) + Math.Pow((currentpoint.Y - voronoi.subelistright[whichedge].up.Y), 2))) > Math.Sqrt((Math.Pow((currentpoint.X - voronoi.subelistright[whichedge].down.X), 2) + Math.Pow((currentpoint.Y - voronoi.subelistright[whichedge].down.Y), 2))))
                        {
                            rightscan = voronoi.subelistright[whichedge].up;
                            currentline.B = rightscan;
                            currentline.perpendicular();
                            if (cross(currentpoint, voronoi.subelistright[whichedge].A, start))
                            {
                                voronoi.subelistright[whichedge].A = start;
                            }
                            else
                            {
                                voronoi.subelistright[whichedge].B = start;
                            }
                            //voronoi.subelistright[whichedge].A = start;
                            alredyright.Add(whichedge);
                        }
                        else
                        {
                            rightscan = voronoi.subelistright[whichedge].down;
                            currentline.B = rightscan;
                            currentline.perpendicular();
                            if (cross(currentpoint, voronoi.subelistright[whichedge].A, start))
                            {
                                voronoi.subelistright[whichedge].A = start;
                            }
                            else
                            {
                                voronoi.subelistright[whichedge].B = start;
                            }
                            //voronoi.subelistright[whichedge].A = start;
                            alredyright.Add(whichedge);
                        }

                    }
  
                }
                else if (doubleintersect == 1)
                {
                    if (Math.Sqrt((Math.Pow((currentpoint.X - voronoi.subelistleft[leftedge].up.X), 2) + Math.Pow((currentpoint.Y - voronoi.subelistleft[leftedge].up.Y), 2))) > Math.Sqrt((Math.Pow((currentpoint.X - voronoi.subelistleft[leftedge].down.X), 2) + Math.Pow((currentpoint.Y - voronoi.subelistleft[leftedge].down.Y), 2))))
                    {
                        leftscan = voronoi.subelistleft[leftedge].up;
                        currentline.A = leftscan;
                        currentline.perpendicular();
                        if (cross(currentpoint, voronoi.subelistleft[leftedge].B, start))
                        {
                            voronoi.subelistleft[leftedge].A = start;
                        }
                        else
                        {
                            voronoi.subelistleft[leftedge].B = start;
                        }
                        //voronoi.subelistleft[whichedge].B = start;
                        alredyleft.Add(leftedge);
                    }
                    else
                    {
                        leftscan = voronoi.subelistleft[leftedge].down;
                        currentline.A = leftscan;
                        currentline.perpendicular();
                        if (cross(currentpoint, voronoi.subelistleft[leftedge].B, start))
                        {
                            voronoi.subelistleft[leftedge].A = start;
                        }
                        else
                        {
                            voronoi.subelistleft[leftedge].B = start;
                        }
                        //voronoi.subelistleft[whichedge].B = start;
                        alredyleft.Add(leftedge);
                    }
                    if (Math.Sqrt((Math.Pow((currentpoint.X - voronoi.subelistright[rightedge].up.X), 2) + Math.Pow((currentpoint.Y - voronoi.subelistright[rightedge].up.Y), 2))) > Math.Sqrt((Math.Pow((currentpoint.X - voronoi.subelistright[rightedge].down.X), 2) + Math.Pow((currentpoint.Y - voronoi.subelistright[rightedge].down.Y), 2))))
                    {
                        rightscan = voronoi.subelistright[rightedge].up;
                        currentline.B = rightscan;
                        currentline.perpendicular();
                        if (cross(currentpoint, voronoi.subelistright[rightedge].A, start))
                        {
                            voronoi.subelistright[rightedge].A = start;
                        }
                        else
                        {
                            voronoi.subelistright[rightedge].B = start;
                        }
                        //voronoi.subelistright[whichedge].A = start;
                        alredyright.Add(rightedge);
                    }
                    else
                    {
                        rightscan = voronoi.subelistright[rightedge].down;
                        currentline.B = rightscan;
                        currentline.perpendicular();
                        if (cross(currentpoint, voronoi.subelistright[rightedge].A, start))
                        {
                            voronoi.subelistright[rightedge].A = start;
                        }
                        else
                        {
                            voronoi.subelistright[rightedge].B = start;
                        }
                        //voronoi.subelistright[whichedge].A = start;
                        alredyright.Add(rightedge);
                    }

                }
                    currentpoint = start;
            }
            voronoi.hyperplane = hyperplane;


            for (int i = 0; i < voronoi.subelistleft.Count; i++)
            {   
                bool flag1=false;
                bool flagone = false, flagtwo = false;
                for(int j = 0; j < alredyleft.Count; j++)
                {
                    if (i == alredyleft[j])
                    {
                        flag1 = true;
                        break;
                    }
                }
                if (flag1)
                {
                    continue;
                }
                Edge e1 = voronoi.subelistleft[i];
                int index = -1;
                for(int j = 0; j < hyperplane.Count; j++)
                {   
                    if(voronoi.subelistleft[i].B.Y<hyperplane[j].A.Y && voronoi.subelistleft[i].B.Y > hyperplane[j].B.Y)
                    {
                        index = j;
                        break;
                    }               
                    
                }
                if (index == -1)
                {
                    continue;
                }
                float xvector = hyperplane[index].B.X - hyperplane[index].A.X;
                float yvector = hyperplane[index].B.Y - hyperplane[index].A.Y;

                //PointF a = new PointF(hyperplane[index].A.X+(((e1.A.Y-hyperplane[index].A.Y)/yvector)*xvector), e1.A.Y);
                PointF a = new PointF(hyperplane[index].A.X+(((e1.A.Y-hyperplane[index].A.Y)*xvector)/yvector), e1.A.Y);
                if (a.X < e1.A.X)
                {
                    flagone = true;
                }

                /*double d1 = (double)GetIntersection(e1.A, e1.B, hyperplane[index].A, hyperplane[index].B).X;
                double d2 = (double)GetIntersection(e1.A, e1.B, hyperplane[index].A, hyperplane[index].B).Y;
                if (d1 == 99999 && d2 == 99999)
                {
                    continue;
                }
                d1 = Math.Round(d1, 2, MidpointRounding.ToEven);
                d2 = Math.Round(d2, 2, MidpointRounding.ToEven);
                PointF t = new PointF((float)d1, (float)d2);
                if (t.X < e1.B.X )
                {
                    flagone = true;
                }*/
                flag1 = false;
                //檢查右邊的點
                /*for (int j = 0; j < alredyleft.Count; j++)
                {
                    if (i == alredyleft[j])
                    {
                        flag1 = true;
                        break;
                    }
                }
                if (flag1)
                {
                    continue;
                }
                e1 = voronoi.subelistleft[i];
                index = -1;
                for (int j = 0; j < hyperplane.Count; j++)
                {
                    if (voronoi.subelistleft[i].A.Y < hyperplane[j].A.Y && voronoi.subelistleft[i].A.Y > hyperplane[j].B.Y)
                    {
                        index = j;
                        break;
                    }

                }
                if (index == -1)
                {
                    continue;
                }
                xvector = hyperplane[index].B.X - hyperplane[index].A.X;
                yvector = hyperplane[index].B.Y - hyperplane[index].A.Y;

                PointF a = new PointF(hyperplane[index].A.X+(((e1.A.Y-hyperplane[index].A.Y)*xvector)/yvector), e1.A.Y);
                if (a.X < e1.B.X)
                {
                    flagone = true;
                }*/

                /*d1 = (double)GetIntersection(e1.A, e1.B, hyperplane[index].A, hyperplane[index].B).X;
                d2 = (double)GetIntersection(e1.A, e1.B, hyperplane[index].A, hyperplane[index].B).Y;
                if (d1 == 99999 && d2 == 99999)
                {
                    continue;
                }
                d1 = Math.Round(d1, 2, MidpointRounding.ToEven);
                d2 = Math.Round(d2, 2, MidpointRounding.ToEven);
                t = new PointF((float)d1, (float)d2);
                if (t.X < e1.A.X)
                {
                    flagtwo = true;
                }*/
                if (flagone)
                {
                    voronoi.subelistleft.Remove(voronoi.subelistleft[i]);
                    i--;
                }
            }


            for (int i = 0; i < voronoi.subelistright.Count; i++)
            {
                bool flag1 = false;
                bool flagone = false, flagtwo = false;
                for (int j = 0; j < alredyright.Count; j++)
                {
                    if (i == alredyright[j])
                    {
                        flag1 = true;
                        break;
                    }
                }
                if (flag1)
                {
                    continue;
                }
                Edge e1 = voronoi.subelistright[i];
                int index = -1;
                for (int j = 0; j < hyperplane.Count; j++)
                {
                    if (voronoi.subelistright[i].B.Y < hyperplane[j].A.Y && voronoi.subelistright[i].B.Y > hyperplane[j].B.Y)
                    {
                        index = j;
                        break;
                    }

                }
                if (index == -1)
                {
                    continue;
                }

                float xvector = hyperplane[index].B.X - hyperplane[index].A.X;
                float yvector = hyperplane[index].B.Y - hyperplane[index].A.Y;

                PointF a = new PointF(hyperplane[index].A.X + (((e1.A.Y - hyperplane[index].A.Y) * xvector) / yvector), e1.A.Y);
                if (a.X > e1.A.X)
                {
                    flagone = true;
                }
                /*double d1 = (double)GetIntersection(e1.A, e1.B, hyperplane[index].A, hyperplane[index].B).X;
                double d2 = (double)GetIntersection(e1.A, e1.B, hyperplane[index].A, hyperplane[index].B).Y;
                if (d1 == 99999 && d2 == 99999)
                {
                    continue;
                }
                d1 = Math.Round(d1, 2, MidpointRounding.ToEven);
                d2 = Math.Round(d2, 2, MidpointRounding.ToEven);
                PointF t = new PointF((float)d1, (float)d2);
                if (t.X > e1.B.X)
                {
                    flagone = true;
                }
                flag1 = false;
                //檢查右邊的點
                for (int j = 0; j < alredyright.Count; j++)
                {
                    if (i == alredyright[j])
                    {
                        flag1 = true;
                        break;
                    }
                }
                if (flag1)
                {
                    continue;
                }
                e1 = voronoi.subelistright[i];
                index = -1;
                for (int j = 0; j < hyperplane.Count; j++)
                {
                    if (voronoi.subelistright[i].A.Y < hyperplane[j].A.Y && voronoi.subelistright[i].A.Y > hyperplane[j].B.Y)
                    {
                        index = j;
                        break;
                    }

                }
                if (index == -1)
                {
                    continue;
                }
                d1 = (double)GetIntersection(e1.A, e1.B, hyperplane[index].A, hyperplane[index].B).X;
                d2 = (double)GetIntersection(e1.A, e1.B, hyperplane[index].A, hyperplane[index].B).Y;
                if (d1 == 99999 && d2 == 99999)
                {
                    continue;
                }
                d1 = Math.Round(d1, 2, MidpointRounding.ToEven);
                d2 = Math.Round(d2, 2, MidpointRounding.ToEven);
                t = new PointF((float)d1, (float)d2);
                if (t.X > e1.A.X)
                {
                    flagtwo = true;
                }*/
                if (flagone)
                {
                    voronoi.subelistright.Remove(voronoi.subelistright[i]);
                    i--;
                }
            }


            



            /*float rightleftest = 99999,leftrightest=-999999;

            foreach(PointF p in voronoi.convexhull.subconvexlist1)
            {
                if (p.X > leftrightest)
                {
                    leftrightest = p.X;
                }
            }
            foreach (PointF p in voronoi.convexhull.subconvexlist2)
            {
                if (p.X > leftrightest)
                {
                    rightleftest = p.X;
                }
            }
            for (int i = 0; i < voronoi.subelistleft.Count; i++)
            {
                if (voronoi.subelistleft[i].A.X > rightleftest)
                {
                    voronoi.subelistleft.Remove(voronoi.subelistleft[i]);
                }
            }
            for(int i=0;i< voronoi.subelistright.Count;i++)
            {
                if (voronoi.subelistright[i].B.X < leftrightest)
                {
                    voronoi.subelistright.Remove(voronoi.subelistright[i]);
                    i--;
                }
            }*/

            Voronoi v = voronoi;
            return v;
        }
        public List<PointF> sortconvexhsublist(List<PointF> convexlist)
        {
            float left = 999999, right = -99999;
            PointF mid = new PointF(), leftest = new PointF(), rightest = new PointF();
            //找最左邊和最右邊的點
            for (int i = 0; i < convexlist.Count; i++)
            {
                if (convexlist[i].X > right)
                {
                    right = convexlist[i].X;
                    rightest = convexlist[i];
                }
                if (convexlist[i].X < left)
                {
                    left = convexlist[i].X;
                    leftest = convexlist[i];
                }
            }
            if (left == right && convexlist.Count > 1)
            {
                convexlist = convexlist.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();
            }
            else
            {
                mid.X = (left + right) / 2;
                mid.Y = (leftest.Y + rightest.Y) / 2;
                for (int j = 0; j < convexlist.Count; j++)
                {
                    for (int i = 1; i < convexlist.Count; i++)
                    {
                        if (convexlist[i - 1].Y < mid.Y && convexlist[(i) % convexlist.Count].Y > mid.Y)
                        {
                            PointF temp = convexlist[i];
                            convexlist[i] = convexlist[(i - 1) % convexlist.Count];
                            convexlist[(i - 1) % convexlist.Count] = temp;
                        }
                        else if (convexlist[i - 1].Y > mid.Y && convexlist[(i) % convexlist.Count].Y < mid.Y)
                        {

                        }
                        else if ((convexlist[i - 1].Y == convexlist[(i) % convexlist.Count].Y))
                        {
                            if (convexlist[i - 1].X < convexlist[(i) % convexlist.Count].X)
                            {
                                PointF temp = convexlist[i];
                                convexlist[i] = convexlist[(i - 1) % convexlist.Count];
                                convexlist[(i - 1) % convexlist.Count] = temp;
                            }
                        }
                        else
                        {
                            if (determinant(convexlist[i - 1], convexlist[(i) % convexlist.Count], mid))
                            {
                                PointF temp = convexlist[i];
                                convexlist[i] = convexlist[(i - 1) % convexlist.Count];
                                convexlist[(i - 1) % convexlist.Count] = temp;
                            }
                        }
                    }
                }

            }
            return convexlist;

        }
        //3階det
        public bool determinant(PointF a, PointF b, PointF c)
        {
            if (b.X * c.Y - c.X * b.Y - a.X * c.Y + c.X * a.Y + a.X * b.Y - b.X * a.Y > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public PointF getInterHPoint(Edge a, Edge b)
        {
            //行列式求兩條直線交點
            PointF hp = new PointF();
            float D = (a.B.X - a.A.X) * (b.A.Y - b.B.Y)
            - (b.B.X - b.A.X) * (a.A.Y - a.B.Y);
            float D1 = (b.A.Y* b.B.X - b.A.X * b.B.Y)
            * (a.B.X - a.A.X)
            - (a.A.Y * a.B.X - a.A.X * a.B.Y)
            * (b.B.X - b.A.X);
            float D2 = (a.A.Y * a.B.X - a.A.X * a.B.Y)
            * (b.A.Y - b.B.Y)
            - (b.A.Y * b.B.X - b.A.X * b.B.Y)
            * (a.A.Y - a.B.Y);
            hp.X = D1 / D;
            hp.Y = D2 / D;
            return hp;
        }

        public List<Edge> sortelist(List<Edge> elist)
        {
            foreach(Edge e in elist)
            {
                if (e.A.X > e.B.X)
                {
                    PointF temp = e.A;
                    e.A = e.B;
                    e.B = temp;
                }
            }
            return elist;
        }
        //算交點
        public static PointF GetIntersection(PointF lineFirstStar, PointF lineFirstEnd, PointF lineSecondStar, PointF lineSecondEnd)
        {
            float a = 0, b = 0;
            int state = 0;
            if (lineFirstStar.X != lineFirstEnd.X)
            {
                a = (lineFirstEnd.Y - lineFirstStar.Y) / (lineFirstEnd.X - lineFirstStar.X);
                state |= 1;
            }
            if (lineSecondStar.X != lineSecondEnd.X)
            {
                b = (lineSecondEnd.Y - lineSecondStar.Y) / (lineSecondEnd.X - lineSecondStar.X);
                state |= 2;
            }
            switch (state)
            {
                case 0: //L1與L2都平行Y軸
                    {
                        if (lineFirstStar.X == lineSecondStar.X)
                        {
                            //throw new Exception("兩條直線互相重合，且平行於Y軸，無法計算交點。");
                            return new PointF(99999, 99999);
                        }
                        else
                        {
                            //throw new Exception("兩條直線互相平行，且平行於Y軸，無法計算交點。");
                            return new PointF(99999, 99999);
                        }
                    }
                case 1: //L1存在斜率, L2平行Y軸
                    {
                        float x = lineSecondStar.X;
                        float y = (lineFirstStar.X - x) * (-a) + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
                case 2: //L1 平行Y軸，L2存在斜率
                    {
                        float x = lineFirstStar.X;
                        //網上有相似代碼的，這一處是錯誤的。你可以對比case 1 的邏輯 進行分析
                        //源code:lineSecondStar * x + lineSecondStar * lineSecondStar.X + p3.Y;
                        float y = (lineSecondStar.X - x) * (-b) + lineSecondStar.Y;
                        return new PointF(x, y);
                    }
                case 3: //L1，L2都存在斜率
                    {
                        if (a == b)
                        {
                            // throw new Exception("兩條直線平行或重合，無法計算交點。");
                            return new PointF(0, 0);
                        }
                        float x = (a * lineFirstStar.X - b * lineSecondStar.X - lineFirstStar.Y + lineSecondStar.Y) / (a - b);
                        float y = a * x - a * lineFirstStar.X + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
            }
            // throw new Exception("不可能發生的情況");
            return new PointF(99999, 99999);
        }


        #region file
        //讀檔
        private void Openfile(object sender, EventArgs e)
        {
            Clear();
            counter = 0;
            filedata.Clear();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            // 設定OpenFileDialog屬性
            openFileDialog1.Title = "選擇要開啟的文字檔案";
            openFileDialog1.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filepath;
                string theFile = openFileDialog1.FileName; //取得檔名
                filepath = Path.GetFullPath(theFile);
                StreamReader file = new StreamReader(filepath);
                foreach (string line in File.ReadLines(filepath))
                {
                    if (line.Length > 0)
                    {
                        if (line[0].Equals('#'))
                        {
                            continue;
                        }
                    }

                    string[] sp = line.Split(' ');
                    for (int i = 0; i < sp.Length; i++)
                    {
                        float x = 0;
                        if (sp[i] != "")
                        {
                            x = Convert.ToSingle(sp[i]);
                            filedata.Add(x);
                        }

                    }

                }
                file.Close();
                /*foreach (int a in filedata)
                    {
                         textBox1.Text += a.ToString() + " ";
                    }*/
            }
            readdata();

        }
        //把各個case寫入暫存
        private void readdata()
        {
            int count = counter;
            //幾個case
            float pointnum;
            pointnum = filedata[counter];
            counter++;
            if (pointnum != 0)
            {
                for (int i = count + 1; i < count + pointnum * 2; i += 2)
                {
                    PointF p = new PointF();
                    p.X = filedata[i];
                    p.Y = filedata[i + 1];
                    plist.Add(p);
                    //voronoi.plist.Add(p);
                    counter += 2;
                }
            }
            else
            {
                counter = 0;
            }
            drawlistpoint(plist);
            foreach(var p in plist)
            {
                listBox1.Items.Add("(" + p.X + ", " + p.Y + ")");
            }
            
            

        }
        //匯出檔案
        private void saveasclick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title="儲存";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
                for (int i = 0; i < voronoi.plist.Count; i++)
                {
                    sw.WriteLine("Point " + voronoi.plist[i].X + " " + voronoi.plist[i].Y);
                }
                for (int i = 0; i < voronoi.elist.Count; i++)
                {
                    sw.WriteLine("Edge " + Convert.ToInt32(voronoi.elist[i].A.X) + " " + Convert.ToInt32(voronoi.elist[i].A.Y) + " "
                        + Convert.ToInt32(voronoi.elist[i].B.X) + " " + Convert.ToInt32(voronoi.elist[i].B.Y));
                }
                sw.Close();
            }
        }

        private void openOutputFileClick(object sender, EventArgs e)
        {
            Clear();
            filedata.Clear();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            // 設定OpenFileDialog屬性
            openFileDialog1.Title = "選擇要開啟的文字檔案";
            openFileDialog1.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filepath;
                string theFile = openFileDialog1.FileName; //取得檔名
                filepath = Path.GetFullPath(theFile);
                StreamReader file = new StreamReader(filepath);
                foreach (string line in File.ReadLines(filepath))
                {
                    if (line.Length > 0)
                    {
                        if (line[0].Equals('#'))
                        {
                            continue;
                        }
                    }

                    string[] sp = line.Split(' ');
                    
                        if (sp[0] == "Point")
                        {
                            DrawPointF(Convert.ToSingle(sp[1]), Convert.ToSingle(sp[2]),Color.DarkBlue);
                            voronoi.plist.Add(new PointF(Convert.ToSingle(sp[1]), Convert.ToSingle(sp[2])));
                        }
                        else if (sp[0] == "Edge")
                        {
                            PointF A = new PointF(Convert.ToSingle(sp[1]), Convert.ToSingle(sp[2]));
                            PointF B = new PointF(Convert.ToSingle(sp[3]), Convert.ToSingle(sp[4]));
                            Edge e1 = new Edge(A, B);
                            voronoi.elist.Add(e1);
                            DrawLine(e1.A, e1.B, Color.Red);

                        }
                        /*float x = 0;
                        if (sp[i] != "")
                        {
                            x = Convert.ToSingle(sp[i]);
                            filedata.Add(x);
                        }*/

          

                }
                file.Close();

            }
   
        }

        #endregion

    }
}
