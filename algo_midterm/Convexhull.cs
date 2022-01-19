/* $LAN=C#$ */
/*
	Name: Convexhull.cs
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
    public class Convexhull
    {
        public List<PointF> convexlist=new List<PointF>();
        public List<PointF> subconvexlist1 = new List<PointF>();
        public List<PointF> subconvexlist2 = new List<PointF>();
        public Edge uptangentline = new Edge();
        public Edge downtangentline = new Edge();
        public Convexhull(){
        
        }
        //sort3點convexhull
        public void sortconvexhull()
        {
            float left = 999999,right=-99999;
            PointF mid=new PointF(),leftest = new PointF(),rightest = new PointF(); 
            //找最左邊和最右邊的點
            for(int i = 0; i < this.convexlist.Count; i++)
            {
                if (this.convexlist[i].X > right)
                {
                    right = this.convexlist[i].X;
                    rightest = this.convexlist[i];
                }
                if (this.convexlist[i].X < left)
                {
                    left = this.convexlist[i].X;
                    leftest = this.convexlist[i];
                }
            }
            if (left==right&&this.convexlist.Count>1)
            {
                this.convexlist=this.convexlist.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();
            }
            else
            {
                mid.X = (left + right) / 2;
                mid.Y = (leftest.Y + rightest.Y) / 2;
                for (int j = 0; j < this.convexlist.Count+1; j++)
                {
                    for (int i = 1; i < this.convexlist.Count; i++)
                    {
                        if(this.convexlist[i-1].Y<mid.Y && this.convexlist[(i)% this.convexlist.Count].Y > mid.Y)
                        {
                            PointF temp = this.convexlist[i];
                            this.convexlist[i] = this.convexlist[(i - 1) % this.convexlist.Count];
                            this.convexlist[(i - 1) % this.convexlist.Count] = temp;
                        }
                        else if (this.convexlist[(i - 1) % this.convexlist.Count].Y > mid.Y && this.convexlist[(i) % this.convexlist.Count].Y < mid.Y)
                        {
                           
                        }
                        else if((this.convexlist[(i - 1) % this.convexlist.Count].Y ==this.convexlist[(i) % this.convexlist.Count].Y))
                        {
                            if((this.convexlist[(i - 1) % this.convexlist.Count].X < this.convexlist[(i) % this.convexlist.Count].X)&&(this.convexlist[(i) % this.convexlist.Count].Y <= mid.Y))
                            {
                                PointF temp = this.convexlist[i];
                                this.convexlist[i] = this.convexlist[(i - 1) % this.convexlist.Count];
                                this.convexlist[(i - 1) % this.convexlist.Count] = temp;
                            }
                            
                        }
                        else
                        {
                            if (determinant(this.convexlist[i-1], this.convexlist[(i) % this.convexlist.Count], mid))
                            {
                                PointF temp = this.convexlist[i];
                                this.convexlist[i] = this.convexlist[(i - 1) % this.convexlist.Count];
                                this.convexlist[(i - 1) % this.convexlist.Count] = temp;
                            }
                        }
                    }
                }
                    
            }
            
        }

        


        //3階det
        public bool determinant(PointF a,PointF b,PointF c)
        {
            if (b.X * c.Y - c.X * b.Y - a.X * c.Y + c.X * a.Y + a.X * b.Y - b.X * a.Y>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
