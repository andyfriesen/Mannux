using System;
using System.Drawing;
using System.Windows.Forms;

namespace Editor {
    class NewMapDlg : Form {
        TextBox widthbox;
        TextBox heightbox;
        public NewMapDlg()
            : base() {
            FormBorderStyle = FormBorderStyle.FixedDialog;

            Label t = new Label();
            t.Text = "Width";
            t.Location = new Point(10, 10);
            t.Show();

            Label u = new Label();
            u.Text = "Height";
            u.Location = new Point(t.Left, t.Bottom + 5);
            u.Show();

            widthbox = new TextBox();
            widthbox.Location = new Point(t.Right + 5, 10);
            widthbox.Show();

            heightbox = new TextBox();
            heightbox.Location = new Point(widthbox.Left, widthbox.Bottom + 5);
            heightbox.Show();

            Button okbutton = new Button();
            okbutton.Text = "OK";
            okbutton.DialogResult = DialogResult.OK;
            okbutton.Location = new Point(t.Left, u.Bottom + 5);
            okbutton.Show();

            Button cancelbutton = new Button();
            cancelbutton.Text = "Cancel";
            cancelbutton.DialogResult = DialogResult.Cancel;
            cancelbutton.Location = new Point(okbutton.Right + 5, okbutton.Top);
            cancelbutton.Show();

            Controls.Add(t);
            Controls.Add(u);
            Controls.Add(widthbox);
            Controls.Add(heightbox);
            Controls.Add(okbutton);
            Controls.Add(cancelbutton);

            AcceptButton = okbutton;
            CancelButton = cancelbutton;

            Width = heightbox.Right + 35;
            Height = okbutton.Bottom + 35;
        }

        public int MapWidth { get { return Convert.ToInt32(widthbox.Text); } }
        public int MapHeight { get { return Convert.ToInt32(heightbox.Text); } }
    }
}
