using MouseDrag;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace BejeweledBot
{
    public partial class Form1 : Form
    {
        private Form overlay;
        private Board board;
        private System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer();
        private Rectangle overlayArea;
        private VirtualMouse virtualMouse = new VirtualMouse();
        private Font drawFont = new Font("Arial", 8);
        private int iTamanhoCelula = 120;

        public Form1()
        {
            KeyboardIntercept keyboardIntercept = new KeyboardIntercept();
            keyboardIntercept.KeyIntercepted += new EventHandler<KeyEventArgs>(Form_KeyDown);
            InitializeComponent();
            pibDebug.CreateGraphics();
            overlay = new OverlayForm();
            refreshTimer.Interval = 600;//era 600
            refreshTimer.Tick += refreshTimer_Tick;
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            //captura
            using (Bitmap bmp = ScreenCap.Grab(overlayArea))
            {
                board.Update(bmp);
                drawColorRep(board.TileColors);
            }

            //verifica melhor jogada
            Move bestMove = MoveHandler.GetBestMove(board.SimplifiedTiles);

            //move a peça
            if (bestMove != null)
            {
                //Point pPontoCoordenada = new Point(overlay.Location.X + bestMove.Pt.X * iTamanhoCelula + (iTamanhoCelula / 2), overlay.Location.Y + bestMove.Pt.Y * iTamanhoCelula + (iTamanhoCelula / 2));
                Point pPontoCoordenadaInicial = new Point(overlay.Location.X + (bestMove.Pt.X * iTamanhoCelula) + (iTamanhoCelula / 2), overlay.Location.Y + (bestMove.Pt.Y * iTamanhoCelula) + (iTamanhoCelula / 2));

                //virtualMouse.Move(click.X, click.Y);
                InputSender.SetCursorPosition(pPontoCoordenadaInicial.X, pPontoCoordenadaInicial.Y);
                Thread.Sleep(5);

                //virtualMouse.Click();
                InputSender.SendMouseInput(
                [
                    new InputSender.MouseInput
                    {
                        dwFlags = (uint)InputSender.MouseEventF.LeftDown
                    }
                ]);
                Point ppPontoCoordenadaFinal = pPontoCoordenadaInicial;

                //para iTamanhoCelula = 120
                if (bestMove.Direction == Direction.UP)
                    for (int i = 0; i < iTamanhoCelula; i++)
                    {
                        i = i + 19;
                        pPontoCoordenadaInicial.X = pPontoCoordenadaInicial.X - (iTamanhoCelula / 5);
                        InputSender.SetCursorPosition(pPontoCoordenadaInicial.X, pPontoCoordenadaInicial.Y);
                        Thread.Sleep(1);
                    }
                else if (bestMove.Direction == Direction.DOWN)
                    for (int i = 0; i < iTamanhoCelula; i++)
                    {
                        i = i + 19;
                        pPontoCoordenadaInicial.X = pPontoCoordenadaInicial.X + (iTamanhoCelula / 5);
                        InputSender.SetCursorPosition(pPontoCoordenadaInicial.X, pPontoCoordenadaInicial.Y);
                        Thread.Sleep(1);
                    }
                else if (bestMove.Direction == Direction.LEFT)
                    for (int i = 0; i < iTamanhoCelula; i++)
                    {
                        i = i + 19;
                        pPontoCoordenadaInicial.Y = pPontoCoordenadaInicial.Y - (iTamanhoCelula / 5);
                        InputSender.SetCursorPosition(pPontoCoordenadaInicial.X, pPontoCoordenadaInicial.Y);
                        Thread.Sleep(1);
                    }
                else if (bestMove.Direction == Direction.RIGHT)
                    for (int i = 0; i < iTamanhoCelula; i++)
                    {
                        i = i + 19;
                        pPontoCoordenadaInicial.Y = pPontoCoordenadaInicial.Y + (iTamanhoCelula / 5);
                        InputSender.SetCursorPosition(pPontoCoordenadaInicial.X, pPontoCoordenadaInicial.Y);
                        Thread.Sleep(1);
                    }
                ////virtualMouse.Move(pPontoCoordenada.X, pPontoCoordenada.Y);
                //InputSender.SetCursorPosition(pPontoCoordenadaInicial.X, pPontoCoordenadaInicial.Y);

                //virtualMouse.Click();
                InputSender.SendMouseInput(
                [
                    new InputSender.MouseInput
                    {
                        dwFlags = (uint)InputSender.MouseEventF.LeftUp
                    }
                ]);
            }
        }

        private void btnGrabScreen_Click(object sender, EventArgs e)
        {
            overlay.Hide();
            overlayArea = new Rectangle(overlay.Location.X,
                                        overlay.Location.Y,
                                        overlay.Width,
                                        overlay.Height);
            using (Bitmap bmp = ScreenCap.Grab(overlayArea))
            {
                board = new Board(iTamanhoCelula, bmp);
            }
            refreshTimer.Start();
        }

        private void drawColorRep(Color[,] colors) //TODO: refactor
        {
            int side = iTamanhoCelula;
            int width = colors.GetLength(0);
            int height = colors.GetLength(1);
            for (int row = 0; row < width; row++)
                for (int col = 0; col < height; col++)
                {
                    using (SolidBrush br = new SolidBrush(colors[row, col]))
                    using (Graphics g = pibDebug.CreateGraphics())
                    {
                        g.FillRectangle(br, new Rectangle(row * side, col * side, side, side));
                        string colorString = board.SimplifiedTiles[row, col].ToString();

                        g.DrawString(colorString, drawFont, new SolidBrush(Color.Black), row * side, col * side);
                    }
                }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            refreshTimer.Stop();
            overlay.Show();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                refreshTimer.Stop();
            }
        }
    }
}