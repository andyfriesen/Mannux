
using Cataract;
using Input;
using Import;
using Import.Geo;
using Sprites;

using Entities;
using Entities.Enemies;

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;

using Microsoft.Xna.Framework;

class Engine : Game {
    // for convenience.  I don't want to mess with stupid proper OOPness for
    // something as simple as a random number generator.
    public static Random rand = new Random();

    // END HACK


    public OpenGLForm graph;			// graphics viewport
    public InputHandler input;			// public because.  I'm a shoddy designer because I don't feel like making an accessor wah wah oh woe is me.
    public Map map;
    public TileSet tileset;
    public Entity cameratarget;	// The engine focuses the camera on this entity
    public Entity player;			// the player entity (merely for convenience)
    public Timer time;
    public Import.TileSet ts;				// purely for the editor's benefit
    public Editor.Editor editor;
    public VectorObstructionMap obs;

    public ArrayList entities = new ArrayList();			// entities currently on the map
    private ArrayList killlist = new ArrayList();			// entities to be removed
    private ArrayList addlist = new ArrayList();			// entities to be added

    bool killflag = false;
    int xwin = 0;
    int ywin = 0;

    protected override void LoadContent() {
        ts = ImageTileSet.Load("mantiles.png", 16, 16, 756);

        tilesets = new TileSetController(graph);
        sprites = new SpriteController(graph);

        input = new InputHandler();

        player = new Player(this);
        cameratarget = player;
        player.X = player.Y = 32;	// arbitrary, if the map doesn't stipulate a starting point.

        //map=v2Map.Load("map00.map");
        MapSwitch("data/maps/test2.map");
        obs = new VectorObstructionMap(map.Obs);

        tileset = new TileSet(graph, ts);

        time = new Timer(100);

        editor = new Editor.Editor(this);
    }

    /* 
     * This is a bit of a hack.
     * When the user closes the window, we pass it up, but set the killflag so that the engine knows to
     * abort when it can.
     */
    void OnClose(object o, CancelEventArgs e) {
        e.Cancel = true;
        graph.Closing -= new CancelEventHandler(OnClose);

        killflag = true;
    }

    // -------------- Core engine logic --------------------

    public void Execute() {
        int frames = 0, fps = 0, timedelta = 0;

        Init();

        int t = time;
        while (!killflag) {
            while (time > t) {
                t++;
                timedelta++;
                input.Keyboard.Poll();

                //-- temp hack
                if (input.Keyboard.Button(1)) {
                    editor.Execute();
                    t = time;					// so the engine doesn't think it has to catch up.
                }
                //--

                Application.DoEvents();

                ProcessEntities();
            }

            frames++;
            Render();
            graph.ShowPage();

            if (timedelta >= 100) {
                fps = frames;
                frames = 0;
                timedelta = 0;
                graph.Text = String.Format("Mannux - {0}fps", fps);
            }
        }
    }

    void ProcessEntities() {
        foreach (Entity e in entities)
            e.Tick();

        foreach (Entity e in killlist) {
            e.Dispose();
            entities.Remove(e);
        }
        killlist.Clear();

        foreach (Entity e in addlist)
            entities.Add(e);
        addlist.Clear();
    }

    public Line IsObs(int x1, int y1, int x2, int y2) {
        return obs.Test(x1, y1, x2, y2);
    }

    public int XWin {
        get { return xwin; }
        set {
            xwin = value;
            if (xwin > map.Width * tileset.Width - graph.XRes)
                xwin = map.Width * tileset.Width - graph.XRes;
            if (xwin < 0) xwin = 0;
        }
    }

    public int YWin {
        get { return ywin; }
        set {
            ywin = value;
            if (ywin > map.Height * tileset.Height - graph.YRes)
                ywin = map.Height * tileset.Height - graph.YRes;
            if (ywin < 0) ywin = 0;
        }
    }
    // ----------- Rendering pipeline -------------------

    public void RenderLayer(Map.Layer lay, bool transparent) {
        int xl, yl;        // x/y run length
        int xs, ys;        // x/y start
        int xofs, yofs;    // sub-tile offset
        int xw, yw;

        xw = (int)(lay.ParallaxX * xwin);
        yw = (int)(lay.ParallaxY * ywin);

        xs = xw / tileset.Width;
        ys = yw / tileset.Height;

        xofs = -(xw % tileset.Width);
        yofs = -(yw % tileset.Height);

        xl = graph.XRes / tileset.Width + 1;
        yl = graph.YRes / tileset.Height + 2;

        if (xs + xl > lay.Width) xl = lay.Width - xs;        // clip yo
        if (ys + yl > lay.Height) yl = lay.Height - ys;

        int curx = xofs;
        int cury = yofs;

        for (int y = 0; y < yl; y++) {
            for (int x = 0; x < xl; x++) {
                int t = lay[x + xs, y + ys];

                if (t != 0 || !transparent)
                    graph.Blit(tileset[t], curx, cury, transparent);
                curx += tileset.Width;
            }
            cury += tileset.Height;
            curx = xofs;
        }
    }


    public void RenderEntity(Entity e) {
        int x = e.X - e.sprite.HotSpot.X - xwin;
        int y = e.Y - e.sprite.HotSpot.Y - ywin;

        if (e.Visible) e.sprite.Draw(x, y, e.anim.frame);
    }

    public void RenderEntities() {
        foreach (Entity e in entities)
            RenderEntity(e);
    }

    public void Render() {
        graph.Clear();

        if (cameratarget != null) {
            // going through the accessors so that the range checking code is called
            XWin = cameratarget.X - graph.XRes / 2;
            YWin = cameratarget.Y - graph.YRes / 2;
        }

        int n = 0;
        foreach (Import.Map.Layer l in map.Layers) {
            RenderLayer(l, n++ != 0);
        }
        RenderEntities();
    }

    public string mapfilename;
    // TODO: something to indicate how the two maps connect, for a fancy transition type thing
    public void MapSwitch(string mapname) {
        Map newmap;
        try {
            System.IO.Stream fs = new System.IO.FileStream(mapname, System.IO.FileMode.Open);
            newmap = MannuxMap.Load(fs);
            fs.Close();
        } catch (System.Exception ex) {
            Console.Write(ex.ToString());
            return;
        }

        map = newmap;
        mapfilename = mapname;

        // nuke all existing entities, except the player
        entities.Clear();
        entities.Add(player);

        foreach (MapEnt e in map.Entities)
            SpawnEntity(e);
    }

    public void MapSwitch(string mapname, int x, int y) {
        MapSwitch(mapname);
        player.X = x;
        player.Y = y;
    }

    void SpawnEntity(MapEnt e) {
        // ew.

        switch (e.type) {
            case "player":
                // Only applies when the game starts on this map.
                player.X = e.x;
                player.Y = e.y;
                break;

            case "door":
                SpawnEntity(new Entities.Door(this,
                                                  e.x, e.y, 		// door position
                                                  Dir.left, 		// direction?
                                                  e.data[0],		// dest map
                                                  Convert.ToInt32(e.data[1]),		// dest X
                                                  Convert.ToInt32(e.data[2])));		// dest Y
                break;

            case "ripper":
                SpawnEntity(new Entities.Enemies.Ripper(this, e.x, e.y));
                break;

            case "hopper":
                SpawnEntity(new Entities.Enemies.Hopper(this, e.x, e.y));
                break;

            default:
                throw new System.Exception(String.Format("Engine::MapSwitch Unknown entity type {0}", e.type));
        }
    }

    public Entity DetectCollision(Entity ent) {
        foreach (Entity ent2 in entities) {
            if (ent == ent2) continue;

            if (ent.X + ent.Width > ent2.X && ent.X < ent2.X + ent2.Width) //inside x coordinates
 			{
                if (ent.Y + ent.Height > ent2.Y && ent.Y < ent2.Y + ent2.Height) //inside y coordinates
 				{
                    return ent2;
                }
            }
        }
        return null;
    }

    public Entity DetectInYCoords(Entity ent) {
        foreach (Entity ent2 in entities) {
            if (ent == ent2) continue;

            if (ent.Y + ent.Height > ent2.Y && ent.Y < ent2.Y + ent2.Height) //inside y coordinates
 			{
                return ent2;
            }

        }
        return null;
    }

    public Entity DetectInXCoords(Entity ent) {
        foreach (Entity ent2 in entities) {
            if (ent == ent2) continue;

            if (ent.X + ent.Width > ent2.X && ent.X < ent2.X + ent2.Width) //inside x coordinates
 			{
                return ent2;
            }

        }
        return null;
    }

    /*
     * Here's the deal.  Altering an ArrayList invalidates any enumerators (or iterators 
     * if you're jiggy with C++ terminology) that are currently being used.  So instead of
     * actually nuking/adding the entity on the spot, we queue it up and nuke it once we're
     * done processing all the entities.
     */

    public void SpawnEntity(Entity e) {
        addlist.Add(e);
    }

    public void DestroyEntity(Entity e) {
        killlist.Add(e);
    }

    //------------------------ Resource management ----------------------

    public SpriteController sprites;	// initted in constructor
    public TileSetController tilesets;	// ditto
    //public SoundController sounds=new SoundController();
    public MapController maps = new MapController();
}
