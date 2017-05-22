using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessUi
{
    public partial class FenForm : Form
    {
        public FenForm()
        {
            InitializeComponent();
        }

        public string FEN
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public bool BadFen { get; set; }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            var game = new Chess.Game();
            game.New();
            try
            {
                game.LoadFEN(textBox1.Text);
                BadFen = false;
                DialogResult = DialogResult.OK;

            }
            catch (Exception)
            {
                MessageBox.Show("Unable to load FEN - string. Check the format");
                DialogResult = DialogResult.Cancel;
                BadFen = true;
            }
        }

        private void FenForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (BadFen)
                e.Cancel = true;
            BadFen = false;
        }
    }
}
