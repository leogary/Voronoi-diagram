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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace algo_midterm
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
