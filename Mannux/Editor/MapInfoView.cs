using System;
using System.Windows.Forms;
using System.Drawing;

namespace Editor {
    class MapInfoView : Form {
        Editor editor;
        Engine engine;

        // Controls
        ListBox curlay;
        TextBox layername, widthbox, heightbox;
        // heh, probably need more things here at some point. ;)

        public MapInfoView(Editor e) {
            editor = e;
            engine = e.engine;

            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Text = "Map Properties";
            ShowInTaskbar = false;

            curlay = new ListBox();
            Controls.Add(curlay);
            curlay.Left = 10;
            curlay.Top = 10;
            curlay.SelectedIndexChanged += new EventHandler(SwitchLayer);

            object[][] layerbuttons = new object[][]
			{
				new object[] {	"Move &Up",		new EventHandler(MoveLayerUp)	},
				new object[] {	"Move &Down",	new EventHandler(MoveLayerDown)	},
				new object[] {	"&Remove",		new EventHandler(RemoveLayer)	},
				new object[] {	"Insert &New",	new EventHandler(InsertNewLayer)},
				new object[] {	"&Add New",		new EventHandler(AddNewLayer)	},
				new object[] {	"&Hide",		new EventHandler(HideLayer)		},
				new object[] {	"&Show",		new EventHandler(ShowLayer)		},
				new object[] {	"Show &Only",	new EventHandler(ShowLayerOnly)	},
				new object[] {	"Show A&ll",	new EventHandler(ShowAll)		},
			};

            int x = 140;
            int y = 10;
            foreach (object[] o in layerbuttons) {
                Button b = new Button();
                b.Text = (string)o[0];
                b.Click += (EventHandler)o[1];
                b.Location = new Point(x, y);

                y += b.Height + 5;

                Controls.Add(b);
            }

            Height = y + 30;

            x = 280;
            y = 10;

            Label l = new Label();
            l.Text = "Name";
            l.Location = new Point(x, y);
            Controls.Add(l);

            layername = new TextBox();
            layername.Location = new Point(x + l.Width, y);
            Controls.Add(layername);

            Button renamelay = new Button();
            renamelay.Text = "Rename";
            renamelay.Click += new EventHandler(RenameLayer);
            renamelay.Location = new Point(layername.Right + 5, layername.Top);
            Controls.Add(renamelay);

            Width = renamelay.Right + 10;

            y += l.Height + 15;
            Label lw = new Label();
            lw.Text = "Width";
            lw.Location = new Point(x, y);
            Controls.Add(lw);

            widthbox = new TextBox();
            widthbox.Location = new Point(x + lw.Width, y);
            widthbox.Text = engine.map.Width.ToString();
            Controls.Add(widthbox);

            Button resize = new Button();
            resize.Text = "Resize";
            resize.Click += new EventHandler(ChangeDims);
            resize.Location = new Point(widthbox.Right + 5, widthbox.Top + 10);
            Controls.Add(resize);

            y += l.Height + 5;
            Label lh = new Label();
            lh.Text = "Height";
            lh.Location = new Point(x, y);
            Controls.Add(lh);

            heightbox = new TextBox();
            heightbox.Location = new Point(x + lh.Width, y);
            heightbox.Text = engine.map.Height.ToString();
            Controls.Add(heightbox);

            Update();
        }

        public new void Update() {
            curlay.Items.Clear();
            for (int i = 0; i < engine.map.NumLayers; i++) {
                curlay.Items.Add(engine.map[i].Name);
            }

            if (editor.tilesetmode.curlayer >= engine.map.NumLayers)
                editor.tilesetmode.curlayer = engine.map.NumLayers - 1;

            heightbox.Text = engine.map.Height.ToString();
            widthbox.Text = engine.map.Width.ToString();

            curlay.SelectedIndex = editor.tilesetmode.curlayer;
            layername.Text = (string)curlay.SelectedItem;
        }

        void SwitchLayer(object o, EventArgs e) {
            editor.tilesetmode.curlayer = curlay.SelectedIndex;
            layername.Text = (string)curlay.SelectedItem;
        }

        void MoveLayerUp(object o, EventArgs e) {
            int lay = curlay.SelectedIndex;

            if (lay == 0) return;	// can't move it up

            engine.map.SwapLayers(lay, lay - 1);

            curlay.SelectedIndex -= 1;
            Update();
        }

        void MoveLayerDown(object o, EventArgs e) {
            int lay = curlay.SelectedIndex;

            if (lay == engine.map.NumLayers - 1) return;	// can't go down

            engine.map.SwapLayers(lay, lay + 1);

            curlay.SelectedIndex += 1;
            Update();
        }

        void RemoveLayer(object o, EventArgs e) {
            DialogResult result = MessageBox.Show("Are you sure?  There is no undoing this.",
                                                "MAYDAY MAYDAY WE ARE UNDER ATTACK",
                                                MessageBoxButtons.YesNo
                                                );

            if (result != DialogResult.Yes)
                return;

            engine.map.RemoveLayer(curlay.SelectedIndex);
            Update();
        }

        void InsertNewLayer(object o, EventArgs e) {
            engine.map.AddLayer(curlay.SelectedIndex);
            Update();
        }

        void AddNewLayer(object o, EventArgs e) {
            engine.map.AddLayer();
            Update();
        }

        void HideLayer(object o, EventArgs e) {
            engine.map[curlay.SelectedIndex].visible = false;
        }

        void ShowLayer(object o, EventArgs e) {
            engine.map[curlay.SelectedIndex].visible = true;
        }

        void ShowLayerOnly(object o, EventArgs e) {
            for (int i = 0; i < engine.map.NumLayers; i++)
                engine.map[i].visible = (i == curlay.SelectedIndex);
        }

        void ShowAll(object o, EventArgs e) {
            for (int i = 0; i < engine.map.NumLayers; i++)
                engine.map[i].visible = true;
        }

        void RenameLayer(object o, EventArgs e) {
            int l = curlay.SelectedIndex;

            curlay.Items[l] = layername.Text;
            engine.map[l].Name = layername.Text;
        }

        void ChangeDims(object o, EventArgs e) {
            DialogResult result = MessageBox.Show("Are you sure?  There is no undoing this.",
                                                "MAYDAY MAYDAY WE ARE UNDER ATTACK",
                                                MessageBoxButtons.YesNo
                                                );

            if (result != DialogResult.Yes)
                return;

            engine.map.Resize(Convert.ToInt32(widthbox.Text), Convert.ToInt32(heightbox.Text));
            Update();

        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }
    }
}
