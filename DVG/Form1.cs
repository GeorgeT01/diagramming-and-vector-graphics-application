using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Drawing2D;
using MindFusion.Diagramming.Export;

using MindFusion.Diagramming.Commands;
using MindFusion.Drawing;
using Pen = System.Drawing.Pen;
using SolidBrush = MindFusion.Drawing.SolidBrush;
using MindFusion.Diagramming;


namespace DVG
{
    public partial class Form1 : Form
    {
        // form shadow
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
        //connectors
        private class Connector
        {
            public Connector(Shape head, string name)
            {
                _head = head;
                _name = name;
            }
            public Shape Head
            {
                get { return _head; }
            }

            public string Name
            {
                get { return _name; }
            }
            private Shape _head;
            private string _name;
        }

        
        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);



            // allow shape naming
            diagramView1.AllowInplaceEdit = true;

            diagram1.NodeEffects.Add(new GlassEffect());
            diagram1.NodeEffects.Add(new AeroEffect());
            //undo-redo defult
#if !STANDARD
            diagram1.UndoManager.UndoEnabled = true;
            diagram1.UndoManager.History.Capacity = 30;
#endif
            Color defAnch = Color.Red;
            // diagram orientation
            diagram1.LinkCascadeOrientation = MindFusion.Diagramming.Orientation.Auto;
            
            // menu icons
            aboutToolStripMenuItem.Image = Properties.Resources.info;
            newToolStripMenuItem.Image = Properties.Resources.file;
            openToolStripMenuItem.Image = Properties.Resources.open;
            saveToolStripMenuItem.Image = Properties.Resources.save;
            saveAsToolStripMenuItem.Image = Properties.Resources.save_as;
            printToolStripMenuItem.Image = Properties.Resources.print;
            exitToolStripMenuItem.Image = Properties.Resources.logout;
            undoToolStripMenuItem.Image = Properties.Resources.undo;
            redoToolStripMenuItem.Image = Properties.Resources.redo;
            howToToolStripMenuItem.Image = Properties.Resources.help;
            imageToolStripMenuItem.Image = Properties.Resources.img;
            pdfToolStripMenuItem.Image = Properties.Resources.pdf;
            previewToolStripMenuItem.Image = Properties.Resources.preview;
            copyToolStripMenuItem.Image = Properties.Resources.paste; // :P image rename
            pastToolStripMenuItem.Image = Properties.Resources.copy;// :P image rename
            cutToolStripMenuItem.Image = Properties.Resources.cut;


            pictureBox1.Image = Properties.Resources.close_white;
            pictureBox2.Image = Properties.Resources.max_white;
            pictureBox3.Image = Properties.Resources.min_white;
            pictureBox4.Image = Properties.Resources.dvg_logo_s;//form icon
            pictureBox5.Image = Properties.Resources.connector;

            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.CenterImage;//form icon
            pictureBox5.SizeMode = PictureBoxSizeMode.CenterImage;


          
            this.Icon = Properties.Resources.dvg_logo;
            diagram1.BackBrush = new MindFusion.Drawing.LinearGradientBrush(
       Color.LightBlue, Color.LightCyan, 30);
            


            /////// Connectors
            _connectors = new Connector[]
                {
                    new Connector(
                        ArrowHeads.Arrow,
                        "Arrow"),
                    new Connector(
                        ArrowHeads.BackSlash,
                        "Back slash"),
                    new Connector(
                        ArrowHeads.BowArrow,
                        "Bow arrow"),
                    new Connector(
                        ArrowHeads.Circle,
                        "Circle"),
                    new Connector(
                        ArrowHeads.DoubleArrow,
                        "Double arrow"),
                    new Connector(
                        ArrowHeads.None,
                        "None"),
                    new Connector(
                        ArrowHeads.Pentagon,
                        "Pentagon"),
                    new Connector(
                        ArrowHeads.PointerArrow,
                        "Pointer"),
                    new Connector(
                        ArrowHeads.Quill,
                        "Quill"),
                    new Connector(
                        ArrowHeads.Reversed,
                        "Reversed"),
                    new Connector(
                        ArrowHeads.RevTriangle,
                        "Reversed triangle"),
                    new Connector(
                        ArrowHeads.RevWithCirc,
                        "Reversed with Circle"),
                    new Connector(
                        ArrowHeads.RevWithLine,
                        "Reversed with Line"),
                    new Connector(
                        ArrowHeads.Rhombus,
                        "Rhombus"),
                    new Connector(
                        ArrowHeads.Slash,
                        "Slash"),
                    new Connector(
                        ArrowHeads.Tetragon,
                        "Tetragon"),
                    new Connector(
                        ArrowHeads.Triangle,
                        "Triangle")
                };
            foreach (Connector c in _connectors)
                _connectorList.Items.Add(c.Name);

          
            _connectorList.SelectedIndex = 0;
            _connectorTypeCombo.SelectedIndex = 0;
        }
        private const int cGrip = 16;
        private const int cCaption = 32;
        // resize form 'right bottom corner'
        protected override void WndProc(ref Message m)
        {
   

            if (m.Msg == 0x84)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {

                    m.Result = (IntPtr)17;
                    return;
                }

            }
            base.WndProc(ref m);
        }
        private static Connector[] _connectors = null;

        private void _connectorList_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            // Draw the item
            ListBox_DrawItem(_connectorList, e);

            Pen pen = new Pen(Color.Black);
            Point p0 = new Point(e.Bounds.X + 2, e.Bounds.Y + 13);
            Point p1 = new Point(e.Bounds.X + 19, e.Bounds.Y + 13);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawLine(Pens.Black, p0, p1);

            DiagramLink.DrawArrowhead(new GdiGraphics(e.Graphics), pen, Brushes.LightSkyBlue,
                _connectors[e.Index].Head, new PointF(50, 0), p1, p0, 12F, false);

            pen.Dispose();
        }
        // draw item
        private void ListBox_DrawItem(ListBox lb, System.Windows.Forms.DrawItemEventArgs e)
        {
            Rectangle rcc;
            RectangleF rc;

            System.Drawing.SolidBrush brush;
            Pen selPen;
            System.Drawing.SolidBrush selBrush;
            StringFormat sf;

            rcc = e.Bounds;
            rcc.Inflate(-1, -1);
            rc = new RectangleF((float)e.Bounds.Left + 32,
                e.Bounds.Top, (float)e.Bounds.Width - 32,
                e.Bounds.Height);

            brush = new System.Drawing.SolidBrush(Color.Black);
            selPen = new Pen(Color.Goldenrod);
            selBrush = new System.Drawing.SolidBrush(Color.LightYellow);

            sf = StringFormat.GenericDefault;
            sf.LineAlignment = StringAlignment.Center;

            // Draw the selection if any
            if ((e.State & DrawItemState.Selected) > 0)
            {
                e.Graphics.FillRectangle(selBrush, rcc);
                e.Graphics.DrawRectangle(selPen, rcc);
            }
            else
            {
                System.Drawing.SolidBrush tb = new
                    System.Drawing.SolidBrush(Color.White);
                e.Graphics.FillRectangle(tb, e.Bounds);
                tb.Dispose();
            }

            brush.Dispose();
            selPen.Dispose();
            selBrush.Dispose();
        }

        private void _connectorList_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 32;
        }

        private void _connectorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int si = _connectorList.SelectedIndex;
            if (si < 0 || si >= _connectors.Length)
                return;

            diagram1.LinkHeadShape = _connectors[si].Head;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (diagram1.Items.Count >= 1)
            {
                if (MessageBox.Show("Are you sure you want exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            else if (diagram1.Items.Count < 1)
            {
                Application.Exit();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (diagram1.Items.Count >= 1)
            {
                if (MessageBox.Show("Are you sure you want to create a new file? \n your work will not be saved", "New File", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    diagram1.ClearAll();
                }
            }
            else if (diagram1.Items.Count < 1)
            {
                diagram1.ClearAll();
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if !STANDARD
            diagram1.UndoManager.Undo();
#endif
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if !STANDARD
            diagram1.UndoManager.Redo();
#endif
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (_openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    diagram1.LoadFromFile(_openFileDialog.FileName);
                }
                catch
                {
                    MessageBox.Show(this, "Failed to open document!");
                }
            }
        }
        // save as .dvg file
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _saveFileDialog.DefaultExt = "dvg";
            _saveFileDialog.Filter = "dvg files|*.dvg";
            if (_saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                diagram1.SaveToFile(_saveFileDialog.FileName, true);
            }
        }
        //save as image 'png'
        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = "png";
            saveFileDialog1.Filter = "PNG files|*.png";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image image = diagram1.CreateImage();
                image.Save(saveFileDialog1.FileName);
                image.Dispose();
            }
        }
        // print preview
        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diagramView1.PrintOptions.DocumentName = "DVG";
            diagramView1.PrintOptions.EnableImages = false;
            diagramView1.PrintOptions.EnableInterior = true;
            diagramView1.PrintOptions.EnableShadows = true;
            diagramView1.PrintOptions.Scale = 100;
            diagramView1.PrintPreview();

        }
        // print
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diagramView1.PrintOptions.DocumentName = "DVG";
            diagramView1.PrintOptions.EnableImages = false;
            diagramView1.PrintOptions.EnableInterior = true;
            diagramView1.PrintOptions.EnableShadows = true;
            diagramView1.PrintOptions.Scale = 100;
            diagramView1.Print();
        }
       //save as pdf
        private void pdfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = "pdf";
            saveFileDialog1.Filter = "PDF files|*.pdf";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MindFusion.Diagramming.Export.PdfExporter pdfExp = new MindFusion.Diagramming.Export.PdfExporter();
                pdfExp.Export(diagramView1.Diagram, saveFileDialog1.FileName);
            }
        }
        // exit app
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (diagram1.Items.Count >= 1)
            {
                if (MessageBox.Show("Are you sure you want exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            else if (diagram1.Items.Count < 1)
            {
                Application.Exit();
            }
        }
        // maximize pb2
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                this.WindowState = FormWindowState.Normal;
                pictureBox2.Image = Properties.Resources.max_white;

            }
            else
            {
                this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                this.WindowState = FormWindowState.Maximized;
                pictureBox2.Image = Properties.Resources.ifmax_white;
            }
        }
        //minimize
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Red;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Transparent;
      
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.LightBlue;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.Transparent;
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.LightBlue;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Transparent;
        }
        // connector type
        private void _connectorTypeCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            LinkShape shape = LinkShape.Polyline;
            short segments = 1;

            switch (_connectorTypeCombo.SelectedIndex)
            {
                case 0:
                    shape = LinkShape.Polyline;
                    break;
                case 1:
                    shape = LinkShape.Bezier;
                    break;
                case 2:
                    shape = LinkShape.Cascading;
                    segments = 3;
                    break;
            }

            diagram1.LinkShape = shape;
            diagram1.LinkSegments = segments;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // this.Location = Screen.AllScreens[0].WorkingArea.Location;
        }
        int mov, movx, movy;
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mov = 0;
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                this.WindowState = FormWindowState.Normal;
                pictureBox2.Image = Properties.Resources.max_white;
            }
            else
            {
                this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                this.WindowState = FormWindowState.Maximized;
                pictureBox2.Image = Properties.Resources.ifmax_white;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutFrm = new AboutForm();
            aboutFrm.ShowDialog();
        }



        private void howToToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.georgealromhin.ga/dvg/howTo?/");
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diagramView1.CopyToClipboard(diagram1.SelectAfterCreate);
            
        }

        private void pastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diagramView1.PasteFromClipboard(5,5);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diagramView1.CutToClipboard(diagram1.SelectAfterCreate);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
           mov = 1;
           movx = e.X;
           movy = e.Y;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mov == 1)
            {
                this.SetDesktopLocation(MousePosition.X - movx, MousePosition.Y - movy);
            }
        }
    }
}
