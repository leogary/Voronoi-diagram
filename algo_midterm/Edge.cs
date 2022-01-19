/* $LAN=C#$ */
/*
	Name: Edge.cs
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
    public class Edge
    {
        public PointF A=new PointF();
        public PointF B=new PointF();
        public PointF midpoint=new PointF();
        public PointF perpendicularpoint = new PointF();
        public PointF up = new PointF();
        public PointF down = new PointF();
        public float xvector=0;
        public float yvector = 0;
        public Edge()
        {

        }
        public Edge(PointF A,PointF B)
        {
            this.A = A;
            this.B = B;
            if(A.X > B.X)
            {   
                PointF temp=A;
                A=B;
                B=temp;
            }
            else if(A.X==B.X&&A.Y>B.Y){
                PointF temp = A;
                A = B;
                B = temp;
            }
            this.midpoint.X=(A.X+B.X)/2;
            this.midpoint.Y = (A.Y + B.Y) / 2;
            perpendicular();

        }
        //中垂線
        public void perpendicular()
        {
            float x = A.X - B.X;
            float y = A.Y - B.Y;
            float temp;
            this.xvector = (-1 )* y;
            this.yvector =  x;
            this.perpendicularpoint.X = this.midpoint.X + this.xvector*600;
            this.perpendicularpoint.Y = this.midpoint.Y + this.yvector*600;
            /*if (this.perpendicularpoint.Y >= 600)
            {
                this.perpendicularpoint.X = this.midpoint.X + (((600 - this.midpoint.Y) / this.yvector) * this.xvector);
                this.perpendicularpoint.Y = 600;
            }
            else if (this.perpendicularpoint.Y<=0)
            {
                this.perpendicularpoint.X = this.midpoint.X + (((0 - this.midpoint.Y) / this.yvector) * this.xvector);
                this.perpendicularpoint.Y = 0;
            }*/

        }
        
    }
}
