using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintApp
{
    public partial class Paint : Form
    {
        // initialize variables
        Bitmap bm;
        Graphics g;
        bool isPainting = false;
        Point px, py;
        Pen p = new Pen(Color.Black, 1);
        Pen erase = new Pen(Color.White, 10);
        int shapeIndex;
        int x, y, shapeX, shapeY, startingX, startingY;
        ColorDialog cd = new ColorDialog();
        Color newColor;

        public Paint()
        {
            InitializeComponent();

            // create a bitmap object with the dimensions of the picture box
            bm = new Bitmap(pic.Width, pic.Height);

            // create a Graphics object from the bitmap
            g = Graphics.FromImage(bm);

            // clear the bitmap with white color
            g.Clear(Color.White);

            // assign the bitmap to the picture box
            pic.Image = bm;
        }

        // clear the picture box
        private void btn_clear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            pic.Image = bm;
            shapeIndex = 0;
        }

        // open a color dialog to choose a new color
        private void btn_color_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            newColor = cd.Color;
            pic_color.BackColor = cd.Color;
            p.Color = cd.Color;
        }

        // set the index to 5 for drawing a line
        private void btn_line_Click(object sender, EventArgs e)
        {
            shapeIndex = 5;
        }

        // set the index to 3 for drawing an ellipse
        private void btn_ellipse_Click(object sender, EventArgs e)
        {
            shapeIndex = 3;
        }

        // fill the region clicked with the new color
        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            if (shapeIndex == 7)
            {
                Point point = SetPoint(pic, e.Location);

                Fill(bm, point.X, point.Y, newColor);
            }
        }

        // set the index to 7 for filling the region with the new color
        private void btn_fill_Click(object sender, EventArgs e)
        {
            shapeIndex = 7;
        }

        // save the image in JPEG format
        private void btn_save_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Image(*.jpg)|*.jpg|(*.*|*.*)";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, pic.Width, pic.Height), bm.PixelFormat);
                btm.Save(sfd.FileName, ImageFormat.Jpeg);
                MessageBox.Show("Image Saved Successfully!");
            }
        }

        // set the starting point and paint flag when mouse is pressed
        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            // if the mouse click is a left click, init the method
            if (e.Button == MouseButtons.Left)
            {
                isPainting = true;
                py = e.Location;
                startingX = e.X;
                startingY = e.Y;
            }

            // if it's a right click, show the context menu
            if (e.Button == MouseButtons.Right)
            {
                contextMenuActions.Show(pic, e.Location);
            }
        }

        // draw the shape, refresh the picture box, and update the coordinates when mouse is moved
        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPainting)
            {
                if (shapeIndex == 1) // draw with pencil
                {
                    px = e.Location;

                    g.DrawLine(p, px, py);

                    py = px;
                }
                if (shapeIndex == 2) // erase with eraser
                {
                    px = e.Location;

                    g.DrawLine(erase, px, py);

                    py = px;
                }
            }

            // Check if the context menu is open
            if (contextMenuActions.Visible)
            {
                return; // If it is, return without executing the shape-drawing code
            }

            pic.Refresh();

            x = e.X;
            y = e.Y;

            shapeX = e.X - startingX;
            shapeY = e.Y - startingY;
        }

        // draw the shape when mouse is released
        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            // Check if the context menu is open
            if (contextMenuActions.Visible)
            {
                return; // If it is, return without executing the shape-drawing code
            }

            isPainting = false;

            shapeX = x - startingX;
            shapeY = y - startingY;

            if (shapeIndex == 3)
            {
                g.DrawEllipse(p, startingX, startingY, shapeX, shapeY);
            }

            if (shapeIndex == 4)
            {
                g.DrawRectangle(p, startingX, startingY, shapeX, shapeY);
            }

            if (shapeIndex == 5)
            {
                g.DrawLine(p, startingX, startingY, x, y);
            }
        }

        // set the index to 1 for drawing with pencil
        private void btn_pencil_Click(object sender, EventArgs e)
        {
            shapeIndex = 1;
        }

        private void btn_open_saved_file_Click(object sender, EventArgs e)
        {
            // if there is an already saved image, open it
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image(*.jpg)|*.jpg|(*.*|*.*)";

            // if the user selects jpg file, open it and display it in the picture box
            // and make it the background image
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                pic.Image = new Bitmap(ofd.FileName);
                bm = new Bitmap(ofd.FileName);
                g = Graphics.FromImage(bm);
                pic.Image = bm;
            }

        }

        // set the index to 2 for erasing with eraser
        private void btn_eraser_Click(object sender, EventArgs e)
        {
            shapeIndex = 2;
        }

        private void contextMenuActions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // if the user clicks on the cut option, cut the selected area
            if (e.ClickedItem.Text == "Cut")
            {
                CutCopyPaste("cut");
            }

            // if the user clicks on the copy option, copy the selected area
            if (e.ClickedItem.Text == "Copy")
            {
                CutCopyPaste("copy");
            }

            // if the user clicks on the paste option, paste the selected area
            if (e.ClickedItem.Text == "Paste")
            {
                CutCopyPaste("paste");
            }
        }

        // Prevent the form from staying in the background when it's closed
        private void Paint_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpMessage = "This is a simple paint application. You can draw with the pencil, erase with the eraser, draw a line, circle, rectangle, and fill the region with a new color.\n\nYou can also save your image in JPEG format and open an already saved image.\n\nYou can also cut, copy, and paste a selected area.\n\nEnjoy!";
            MessageBox.Show(helpMessage, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // set the index to 4 for drawing a rectangle
        private void btn_rect_Click(object sender, EventArgs e)
        {
            shapeIndex = 4;
        }

        // explain what this method does
        private void pic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (isPainting)
            {
                if (shapeIndex == 3)
                {
                    g.DrawEllipse(p, startingX, startingY, shapeX, shapeY);
                }

                if (shapeIndex == 4)
                {
                    g.DrawRectangle(p, startingX, startingY, shapeX, shapeY);
                }

                if (shapeIndex == 5)
                {
                    g.DrawLine(p, startingX, startingY, x, y);
                }
            }
        }

        // This method scales the point coordinates to fit the size of the PictureBox
        // This is useful when handling mouse events in a PictureBox that has been resized or
        // has a different aspect ratio than the image it contains.
        static Point SetPoint(PictureBox pb, Point pt)
        {
            float pX = pb.Width / pb.Width;
            float pY = pb.Height / pb.Height;

            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));
        }

        // This method validates whether a pixel
        // at a given location should be colored with the new color or not.
        private void Validate(Bitmap bm, Stack<Point> sp, int x, int y, Color old_color, Color new_color)
        {
            Color cx = bm.GetPixel(x, y);

            // If the color matches the old color, it pushes the location
            // onto the stack, sets the color of the pixel to the new color,
            if (cx == old_color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, new_color);
            }
        }

        // This method fills a region in a Bitmap with a new color
        // by replacing all pixels that have the old color with the new color.
        public void Fill(Bitmap bm, int x, int y, Color new_clr)
        {
            Color old_color = bm.GetPixel(x, y);

            Stack<Point> pixel = new Stack<Point>();

            pixel.Push(new Point(x, y));

            bm.SetPixel(x, y, new_clr);

            if (old_color == new_clr) return;

            while (pixel.Count > 0)
            {
                Point pt = (Point)pixel.Pop();
                if (pt.X > 0 && pt.Y > 0 && pt.X < bm.Width - 1 && pt.Y < bm.Height - 1)
                {
                    Validate(bm, pixel, pt.X - 1, pt.Y, old_color, new_clr);
                    Validate(bm, pixel, pt.X, pt.Y - 1, old_color, new_clr);
                    Validate(bm, pixel, pt.X + 1, pt.Y, old_color, new_clr);
                    Validate(bm, pixel, pt.X, pt.Y + 1, old_color, new_clr);
                }
            }
        }

        // This method handles cut, copy, and paste actions
        private void CutCopyPaste(string action)
        {
            if (action == "cut")
            {
                Clipboard.SetImage(pic.Image);
                g.Clear(Color.White);
                pic.Refresh();
            }
            else if (action == "copy")
            {
                Clipboard.SetImage(pic.Image);
            }
            else if (action == "paste")
            {
                if (Clipboard.ContainsImage())
                {
                    g.DrawImage(Clipboard.GetImage(), pic.PointToClient(Cursor.Position));
                }
            }
        }

    }
}
