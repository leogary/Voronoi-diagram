/* $LAN=C#$ */
/*
	Name: Voronoi.cs
	Copyright: Copyright © 2021
	Author:簡志軒
    Student ID: M103040069
    Class: 資工碩一
	Date: 2021/12/28
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace algo_midterm
{   
    public class Voronoi
    {
        /*public List<PointF> subplist1 = new List<PointF>();
        public List<PointF> subplist2 = new List<PointF>();*/
        public List<PointF> plist=new List<PointF>();
        public List<Edge> elist=new List<Edge>();
        public List<Edge> subelistleft =new List<Edge>();
         public List<Edge> subelistright =new List<Edge>();
        public Convexhull convexhull = new Convexhull();
        public List<Edge> hyperplane = new List<Edge>();
        public Voronoi()
        {

        }
        public Voronoi(List<PointF> plist, List<Edge> elist)
        {
            this.plist = plist;
            this.elist = elist;
        }
        //3點在一條線上
        public bool onthesameline(Voronoi voronoi)
        {   
            //排序3個點
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    if (voronoi.plist[j].X > voronoi.plist[j + 1].X)
                    {
                        PointF temp = new PointF();
                        temp = voronoi.plist[j];
                        voronoi.plist[j] = voronoi.plist[j + 1];
                        voronoi.plist[j + 1] = temp;
                    }
                    else if(voronoi.plist[j].X == voronoi.plist[j + 1].X&& voronoi.plist[j].Y > voronoi.plist[j + 1].Y)
                    {
                        PointF temp = new PointF();
                        temp = voronoi.plist[j];
                        voronoi.plist[j] = voronoi.plist[j + 1];
                        voronoi.plist[j + 1] = temp;
                    }
                }

            }
            float slope1 = ((voronoi.plist[2].X - voronoi.plist[1].X) / (voronoi.plist[2].Y - voronoi.plist[1].Y));
            float slope2 = ((voronoi.plist[1].X - voronoi.plist[0].X) / (voronoi.plist[1].Y - voronoi.plist[0].Y));
            float slope3 = ((voronoi.plist[2].X - voronoi.plist[0].X) / (voronoi.plist[2].Y - voronoi.plist[0].Y));

            if (slope1 == slope2&&slope3 ==slope2&&slope1==slope3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
         //sort point
        public void sortplist()
        {
            for(int i = 0; i < this.plist.Count-1; i++)
            {
                for(int j = 0; j < this.plist.Count-1; j++)
                {
                    if (plist[j].X > plist[j+1].X)
                    {
                        PointF temp = plist[j];
                        plist[j] = plist[j+1];
                        plist[j+1] = temp;
                    }
                    else if (plist[j].X == plist[j + 1].X && plist[j].Y > plist[j + 1].Y)
                    {
                        PointF temp = plist[j];
                        plist[j] = plist[j + 1];
                        plist[j + 1] = temp;
                    }
                }
            }
        }
        //sort edge之間順序
        public void sortelist()
        {
            elist = elist.OrderBy(x => x.A.X).ThenBy(y => y.B.X).ToList();
        }
        //刪掉重複的點
        public void killreapet()
        {   
            Dictionary<string, string> map = new Dictionary<string, string>();
            for(int i = 0; i < this.plist.Count; i++)
            {
                
                string temp = this.plist[i].X.ToString() + this.plist[i].Y.ToString(),s;
                if (!map.TryGetValue(temp, out s))
                {
                    map.Add(temp, "1");
                }
                else
                {
                    this.plist.Remove(plist[i]);
                    i--;
                }
                

            }
            

        }
        
    }
}
