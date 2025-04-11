using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    #region round
    public partial class Form2 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
       (
           int nLeftRect,     // x-coordinate of upper-left corner
           int nTopRect,      // y-coordinate of upper-left corner
           int nRightRect,    // x-coordinate of lower-right corner
           int nBottomRect,   // y-coordinate of lower-right corner
           int nWidthEllipse, // width of ellipse
           int nHeightEllipse // height of ellipse
       );

        public Point mouseLocation;

        public Form2()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

#endregion round

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // make your own shit fucking retarded ass skid, ur not loved
            // mystizal owns u
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            // make your own shit fucking retarded ass skid, ur not loved
            // mystizal owns u
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            // make your own shit fucking retarded ass skid, ur not loved
            // mystizal owns u
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            // make your own shit fucking retarded ass skid, ur not loved
            // mystizal owns u
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            // make your own shit fucking retarded ass skid, ur not loved
            // mystizal owns u
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            // make your own shit fucking retarded ass skid, ur not loved
            // mystizal owns u
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            // make your own shit fucking retarded ass skid, ur not loved
            // mystizal owns u
        }
    }
}
