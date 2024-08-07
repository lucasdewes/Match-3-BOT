using System.Drawing;
using System.Windows.Forms;

namespace BejeweledBot
{
    internal class OverlayForm : Form
    {
        private int X { get; set; }
        private int Y { get; set; }
        private bool isDragged = false;
        private Point moveStartPoint;

        public OverlayForm(int width = 960, int height = 960)// original é 330          680/8 = 85      960/8 = 120
        {
            BackColor = Color.Blue;
            FormBorderStyle = FormBorderStyle.None;
            Bounds = new Rectangle(0, 0, width, height);
            MouseDown += overlay_MouseDown;
            MouseUp += overlay_MouseUp;
            MouseMove += overlay_MouseMove;
            Opacity = 0.2;
            TopMost = true;
            // Adiciona o evento de pintura
            Paint += OverlayForm_Paint;
            Show();
        }

        private void OverlayForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.White, 1); // Defina a cor e a espessura da linha aqui

            int numRows = 8;
            int numCols = 8;

            float rowHeight = (float)Height / numRows;
            float colWidth = (float)Width / numCols;

            // Desenhar linhas horizontais
            for (int i = 1; i < numRows; i++)
            {
                g.DrawLine(pen, 0, i * rowHeight, Width, i * rowHeight);
            }

            // Desenhar linhas verticais
            for (int i = 1; i < numCols; i++)
            {
                g.DrawLine(pen, i * colWidth, 0, i * colWidth, Height);
            }
        }

        private void overlay_MouseDown(object sender, MouseEventArgs e)
        {
            moveStartPoint = new Point(e.X, e.Y);
            isDragged = true;
        }

        private void overlay_MouseUp(object sender, MouseEventArgs e)
        {
            isDragged = false;
        }

        private void overlay_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged)
            {
                Point p1 = new Point(e.X, e.Y);
                Point p2 = PointToScreen(p1);
                Point p3 =
                Location = new Point(p2.X - moveStartPoint.X,
                                     p2.Y - moveStartPoint.Y);
            }
        }
    }
}