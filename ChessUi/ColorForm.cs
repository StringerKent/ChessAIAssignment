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
    public partial class ColorForm : Form
    {
        public ColorForm()
        {
            InitializeComponent();
        }

        public ColorForm(ChessForm chessForm)
        {   
            ChessForm = chessForm;
            InitializeComponent();
        }

        public ChessForm ChessForm { get; set; }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            ChessForm.VisibleBoard.ColorTheme = trackBar1.Value;
            ChessForm.Redraw();
        }
    }
}
