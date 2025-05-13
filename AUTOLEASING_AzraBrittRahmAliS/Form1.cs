using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AUTOLEASING_AzraBrittRahmAliS
{
    public partial class Form1 : Form
    {
      
        public Form1()
        {
            InitializeComponent();
        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.Appearance = TabAppearance.FlatButtons;

            tabControl1.ItemSize = new Size(0, 1);

            tabControl1.SizeMode = TabSizeMode.Fixed;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button3GEN_Click(object sender, EventArgs e)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            char[] chars = new char[8];

            for (int i = 0; i < 8; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            textBox3Passwort.Text = new string(chars);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
