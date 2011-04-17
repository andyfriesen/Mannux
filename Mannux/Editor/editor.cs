// integrated map editor
// The main reason we're doing this is so that you can stop the game, edit the map, and resume.  Sweet stuff.
// Lots of evil public members here because I'm a bitch. -- andy

using Cataract;
using Import.Geo;

using System;
using System.IO;
//using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Editor {
    class Editor {
        public Engine engine;
        OpenGLForm form;
        bool killflag;

        // UI elements
        MainMenu menu;
        public StatusBar statbar;
        TileSetPreview tilesetpreview;
        MapInfoView mapinfoview;
        public MapEntPropertiesView mapentpropertiesview;
        AutoSelectionThing autoselectionthing;

        public IEditorState state;
        public TileSetMode tilesetmode;
        public CopyPasteMode copypastemode;
        public ObstructionMode obstructionmode;
        public EntityEditMode entityeditmode;

        public Editor(Engine e) {
            engine = e;
            form = e.graph;

            tilesetmode = new TileSetMode(this);
            copypastemode = new CopyPasteMode(this);
            obstructionmode = new ObstructionMode(this);
            entityeditmode = new EntityEditMode(this);

            statbar = new StatusBar();
            statbar.Panels.Add(new StatusBarPanel());
            statbar.Panels.Add(new StatusBarPanel());

            statbar.Panels[0].AutoSize = StatusBarPanelAutoSize.Spring;
            statbar.Panels[1].AutoSize = StatusBarPanelAutoSize.Contents;
            statbar.ShowPanels = true;

            // muahahahah

            menu = new MainMenu(new MenuItem[] {
                new MenuItem("&File", new MenuItem[] {
                    new MenuItem("&New",               new EventHandler(NewMap   ), Shortcut.CtrlN),
                    new MenuItem("&Open...",           new EventHandler(OpenMap  ), Shortcut.CtrlO),
                    new MenuItem("-"),
                    new MenuItem("&Save",              new EventHandler(SaveMap  ), Shortcut.CtrlS),
                    new MenuItem("Save &As...",        new EventHandler(SaveMapAs), Shortcut.F12  ),
                    new MenuItem("-"),
                    new MenuItem("E&xit",              new EventHandler(Exit     ))
                }),
                new MenuItem("&Edit", new MenuItem[] {
                    new MenuItem("&Map Properties...", new EventHandler(ShowMapProperties   )),
                    new MenuItem("&Tileset...",        new EventHandler(ShowTileSet         )),
                    new MenuItem("Map &Entities...",   new EventHandler(ShowMapEntProperties)),
                    new MenuItem("&Auto Selection Thing...",	new EventHandler(ShowAutoSelectionThing)),
                }),
                new MenuItem("&Mode", new MenuItem[] {
                    new MenuItem("&Tiles",             new EventHandler(SetTileSetMode        )),
                    new MenuItem("&Copy/paste",        new EventHandler(SetCopyPasteMode      )),
                    new MenuItem("&Obstructions",      new EventHandler(SetObstructionEditMode)),
                    new MenuItem("Map &Entities",      new EventHandler(SetMapEntEditMode     ))
                })
            });

            tilesetpreview = new TileSetPreview(engine.ts);
            tilesetpreview.ChangeTile += new ChangeTileHandler(OnTileChange);
            mapinfoview = new MapInfoView(this);
            mapentpropertiesview = new MapEntPropertiesView(this);
            autoselectionthing = new AutoSelectionThing(this);

            form.AddOwnedForm(mapinfoview);
            form.AddOwnedForm(mapentpropertiesview);
            form.AddOwnedForm(tilesetpreview);
            form.AddOwnedForm(autoselectionthing);
        }

        public void Init() {
            // Assigns events and such
            form.KeyDown += new KeyEventHandler(this.KeyPress);
            form.MouseDown += new MouseEventHandler(this.MouseClick);
            form.MouseWheel += new MouseEventHandler(this.MouseClick);
            form.MouseMove += new MouseEventHandler(this.MouseDown);
            form.MouseUp += new MouseEventHandler(this.MouseUp);
            form.Closing += new CancelEventHandler(this.OnClosing);

            killflag = false;

            //form.Controls.Add(statbar);
            form.Menu = menu;
            // make the window bigger; if we don't, GL will just scale things.  Blech.
            form.ClientSize = new System.Drawing.Size(
                form.XRes, form.YRes// + statbar.Height
            );

            state = tilesetmode;

            form.Text = "Mannux -- Editor";
        }

        public void Shutdown() {
            // removes added events so that the engine can resume as if nothing had happened
            form.KeyDown -= new KeyEventHandler(this.KeyPress);
            form.MouseDown -= new MouseEventHandler(this.MouseClick);
            form.MouseWheel -= new MouseEventHandler(this.MouseClick);
            form.MouseMove -= new MouseEventHandler(this.MouseDown);
            form.MouseUp -= new MouseEventHandler(this.MouseUp);
            form.Closing -= new CancelEventHandler(this.OnClosing);

            form.Controls.Remove(statbar);
            form.Menu = null;
            form.ClientSize = new System.Drawing.Size(form.XRes, form.YRes);

            // update the map obstructions
            engine.obs.Generate(engine.map.Obs);

            mapinfoview.Hide();
            mapentpropertiesview.Hide();
            tilesetpreview.Hide();
        }

        public void Execute() {
            Init();

            while (!killflag) {
                Application.DoEvents();

                engine.input.Keyboard.Poll();
                if (engine.input.Keyboard.Axis(1) == 0) engine.XWin -= 2;
                if (engine.input.Keyboard.Axis(1) == 255) engine.XWin += 2;
                if (engine.input.Keyboard.Axis(0) == 0) engine.YWin -= 2;
                if (engine.input.Keyboard.Axis(0) == 255) engine.YWin += 2;

                form.Clear();
                Render();
                form.ShowPage();
            }

            Shutdown();
        }

        void Render() {
            int i = 0;
            foreach (Import.Map.Layer lay in engine.map.Layers) {
                if (lay.visible)
                    engine.RenderLayer(lay, i++ != 0);
            }

            engine.RenderEntities();

            state.RenderHUD();
        }

        int x, y;
        void MouseClick(object o, MouseEventArgs e) {
            state.MouseClick(e);
        }

        void MouseDown(object o, MouseEventArgs e) {
            x = e.X; y = e.Y;
            state.MouseDown(e);
        }

        void MouseUp(object o, MouseEventArgs e) {
            state.MouseUp(e);
        }

        void KeyPress(object o, KeyEventArgs e) {
            state.KeyPress(e);
        }

        public void OnClosing(object o, CancelEventArgs e) {
            killflag = true;
            e.Cancel = true;
        }

        //--

        void NewMap(object o, EventArgs e) {
            NewMapDlg dlg = new NewMapDlg();
            DialogResult result = dlg.ShowDialog();

            if (result == DialogResult.Cancel)
                return;

            engine.map = new Import.Map(dlg.MapWidth, dlg.MapHeight);
            engine.map.AddLayer();

            engine.entities.Clear();
            engine.entities.Add(engine.player);



            tilesetmode.curlayer = 0;
            mapinfoview.Update();
        }

        void OpenMap(object o, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "map";
            dlg.Filter = "Mannux Maps (*.map)|*.map|All files (*.*)|*.*";
            dlg.Title = "Open Map";

            string s = Directory.GetCurrentDirectory();

            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;

            //engine.map = (Import.Map)engine.maps.Load(dlg.FileName);
            engine.MapSwitch(dlg.FileName);
            mapinfoview.Update();

            Directory.SetCurrentDirectory(s);
        }

        void SaveMap(object o, EventArgs e) {
            try {
                FileStream fs = new FileStream(engine.mapfilename, FileMode.Create);
                Import.MannuxMap.Save(engine.map, fs);
                fs.Close();
            } catch (System.Exception ex) {
                Console.WriteLine("Error saving map!");
                Console.WriteLine(ex.ToString());
                return;
            }
        }

        void SaveMapAs(object o, EventArgs e) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = "map";
            dlg.Filter = "Mannux Maps (*.map)|*.map|All files (*.*)|*.*";
            dlg.Title = "Save Map As...";

            string s = Directory.GetCurrentDirectory();

            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;

            FileStream fs = new FileStream(dlg.FileName, FileMode.Create);
            Import.MannuxMap.Save(engine.map, fs);
            fs.Close();

            Directory.SetCurrentDirectory(s);

            engine.mapfilename = dlg.FileName;
        }

        public void Exit(object o, EventArgs e) {
            killflag = true;
        }

        public void ShowTileSet(object o, EventArgs e) {
            tilesetpreview.Show();
        }

        public void ShowMapProperties(object o, EventArgs e) {
            mapinfoview.Show();
        }

        public void ShowMapEntProperties(object o, EventArgs e) {
            mapentpropertiesview.Show();
        }

        void ShowAutoSelectionThing(object o, EventArgs e) {
            autoselectionthing.Show();
        }

        //--

        void SetTileSetMode(object o, EventArgs e) {
            state = tilesetmode;
        }

        void SetCopyPasteMode(object o, EventArgs e) {
            state = copypastemode;
        }

        void SetObstructionEditMode(object o, EventArgs e) {
            state = obstructionmode;
        }

        void SetMapEntEditMode(object o, EventArgs e) {
            state = entityeditmode;
        }

        //--

        void OnTileChange(int newtile) {
            tilesetmode.curtile = newtile;
        }

    }
}
