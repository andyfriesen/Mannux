
using Input;
using Sprites;

using Entities;
using Entities.Enemies;

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mannux;

class Engine : Game {
    public XNAGraph graph;
    private GraphicsDeviceManager graphics;
    public InputHandler input;			// public because.  I'm a shoddy designer because I don't feel like making an accessor wah wah oh woe is me.
    private Squared.Tiled.Map map;
    private Squared.Tiled.Layer obstructionLayer;
    ObstructionTileset obstructionTiles;

    public BitmapSprite tileset;
    public Entity cameraTarget;	// The engine focuses the camera on this entity
    public Entity player;			// the player entity (merely for convenience)
    public Timer time;

    public List<Entity> entities = new List<Entity>();			// entities currently on the map
    private List<Entity> killlist = new List<Entity>();			// entities to be removed
    private List<Entity> addlist = new List<Entity>();			// entities to be added

    int xwin = 0;
    int ywin = 0;

    public Engine() {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void Initialize() {
        base.Initialize();
        IsMouseVisible = true;
    }

    protected override void LoadContent() {
        graph = new XNAGraph(graphics.GraphicsDevice, this.Content);
        input = new InputHandler();

        tileset = new BitmapSprite(graph, "mantiles", 16, 16, 19, new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16));
        TabbySprite = new BitmapSprite(graph, "tabpis", 64, 64, 8, new Rectangle(24, 24, 16, 40));
        DoorSprite = new BitmapSprite(graph, "door", 16, 64, 7, new Rectangle(0, 0, 16, 64));
        RipperSprite = new BitmapSprite(graph, "ripper", 16, 32, 4, new Rectangle(0, 0, 16, 20));
        BoomSprite = new BitmapSprite(graph, "boom", 16, 16, 7, new Rectangle(0, 0, 16, 16));
        BulletSprite = new BitmapSprite(graph, "bullet", 8, 8, 8, new Rectangle(0, 0, 8, 8));

        player = new Player(this);
        cameraTarget = player;
        player.X = player.Y = 32;	// arbitrary, if the map doesn't stipulate a starting point.

        obstructionTiles = new ObstructionTileset();

        MapSwitch("tiledtest.tmx");

        obstructionLayer = null;
        foreach (var l in map.Layers) {
            string value;
            var s = l.Value.Properties.TryGetValue("type", out value);
            if (s && value == "obstructions") {
                obstructionLayer = l.Value;
                //obstructionLayer.Opacity = 0;
                break;
            }
        }

        time = new Timer(100);
    }

    // -------------- Core engine logic --------------------

    protected override void Update(GameTime gameTime) {
        base.Update(gameTime);
        input.Poll();
        ProcessEntities();
    }

    protected override void Draw(GameTime gameTime) {
        base.Draw(gameTime);
        graph.Begin();
        Render();
        graph.End();
        RenderEntityHotspots();
    }

    void ProcessEntities() {
        foreach (Entity e in entities) {
            e.Tick();
        }

        foreach (Entity e in killlist) {
            entities.Remove(e);
        }
        killlist.Clear();

        foreach (Entity e in addlist) {
            entities.Add(e);
        }
        addlist.Clear();
    }

    public Line? IsObs(float x, float y, float w, float h) {
        return IsObs((int)x, (int)y, (int)w, (int)h);
    }

    public Line? IsObs(int x, int y, int w, int h) {
        return IsObs(new Rectangle(x, y, w, h));
    }

    public Line? IsObs(Rectangle r) {
        var ty = r.Top / map.TileHeight;
        var tey = r.Bottom / map.TileHeight;
        var tx = r.Left / map.TileWidth;
        var tex = r.Right / map.TileWidth;

        if (ty < 0) {
            return new Line(0, 0, map.Width, 0);
        }
        if (ty >= map.Height) {
            return new Line(0, map.Height, map.Width, map.Height);
        }
        if (tx < 0) {
            return new Line(0, 0, 0, map.Height);
        }
        if (tx >= map.Width) {
            return new Line(map.Width, 0, map.Width, map.Height);
        }

        for (var y = ty; y <= tey; ++y) {
            for (var x = tx; x <= tex; ++x) {
                var s = r;
                s.Offset(-x * map.TileWidth, -y * map.TileHeight);

                var result = obstructionTiles.Test(obstructionLayer.GetTile(x, y), s);
                if (result != null) {
                    var q = result.Value;
                    q.Offset(x * map.TileWidth, y * map.TileHeight);
                    return q;
                }
            }
        }

        return null;
    }

    public void GetObstructions(Rectangle r, Action<Line> cb) {
        var ty = r.Top / map.TileHeight;
        var tey = r.Bottom / map.TileHeight;
        var tx = r.Left / map.TileWidth;
        var tex = r.Right / map.TileWidth;

        if (ty < 0) {
            cb(new Line(0, 0, map.Width, 0));
            return;
        }
        if (ty >= map.Height) {
            cb(new Line(0, map.Height, map.Width, map.Height));
            return;
        }
        if (tx < 0) {
            cb(new Line(0, 0, 0, map.Height));
            return;
        }
        if (tx >= map.Width) {
            cb(new Line(map.Width, 0, map.Width, map.Height));
            return;
        }

        for (var y = ty; y <= tey; ++y) {
            for (var x = tx; x <= tex; ++x) {
                var tileSpaceRect = r;
                tileSpaceRect.Offset(-x * map.TileWidth, -y * map.TileHeight);

                var lines = obstructionTiles.LinesForTile(obstructionLayer.GetTile(x, y));
                if (lines == null) {
                    continue;
                }

                Point intercept = Point.Zero;
                foreach (var l in lines) {
                    if (l.Touches(tileSpaceRect, ref intercept)) {
                        var q = l;
                        q.Offset(x * map.TileWidth, y * map.TileHeight);
                        cb(q);
                    }
                }
            }
        }
    }

    public int XWin {
        get { return xwin; }
        set {
            xwin = value;
            if (xwin > map.Width * map.TileWidth - graph.XRes)
                xwin = map.Width * map.TileHeight - graph.XRes;
            if (xwin < 0) xwin = 0;
        }
    }

    public int YWin {
        get { return ywin; }
        set {
            ywin = value;
            if (ywin > map.Height * map.TileHeight - graph.YRes)
                ywin = map.Height * map.TileHeight - graph.YRes;
            if (ywin < 0) ywin = 0;
        }
    }
    // ----------- Rendering pipeline -------------------

    public void RenderEntity(Entity e) {
        int x = e.X - e.sprite.HotSpot.X - xwin;
        int y = e.Y - e.sprite.HotSpot.Y - ywin;

        if (e.Visible) {
            e.sprite.Draw(x, y, e.anim.frame);
        }
    }

    public void RenderEntities() {
        foreach (Entity e in entities) {
            RenderEntity(e);
        }
    }

    public void RenderEntityHotspots() {
        foreach (var e in entities) {
            graph.DrawRect(new Rectangle(e.X - xwin, e.Y - ywin, e.sprite.HotSpot.Width, e.sprite.HotSpot.Height));
            if (e.groundSurface.HasValue) {
                var q = e.groundSurface.Value;
                q.Offset(-xwin, -ywin);
                graph.DrawLine(q, Color.Violet);
            }
        }
    }

    public void Render() {
        graph.Clear();

        if (cameraTarget != null) {
            // going through the accessors so that the range checking code is called
            XWin = cameraTarget.X - graph.XRes / 2;
            YWin = cameraTarget.Y - graph.YRes / 2;
        }

        map.Draw(graph.SpriteBatch, new Rectangle(0, 0, graph.XRes, graph.YRes), new Vector2(xwin, ywin));
        RenderEntities();
    }

    public string mapfilename;
    // TODO: something to indicate how the two maps connect, for a fancy transition type thing
    public void MapSwitch(string mapname) {
        map = Squared.Tiled.Map.Load(mapname, Content);
        mapfilename = mapname;

        obstructionTiles.FirstTileID = map.Tilesets["obstiles"].FirstTileID;

        // nuke all existing entities, except the player
        entities.Clear();
        entities.Add(player);

        foreach (var og in map.ObjectGroups) {
            foreach (var o in og.Value.Objects) {
                SpawnEntity(o.Value);
            }
        }
    }

    public void MapSwitch(string mapname, int x, int y) {
        MapSwitch(mapname);
        player.X = x;
        player.Y = y;
    }

    void SpawnEntity(Squared.Tiled.Object e) {
        // ew.

        switch (e.Type) {
            case "player":
                // Only applies when the game starts on this map.
                player.X = e.X;
                player.Y = e.Y;
                break;

            case "door":
                SpawnEntity(new Entities.Door(this,
                                              e.X, e.Y, 		// door position
                                              Dir.left, 		// direction?
                                              e.Properties["destination"],
                                              Convert.ToInt32(e.Properties["destinationX"]),
                                              Convert.ToInt32(e.Properties["destinationY"])));
                break;

            case "ripper":
                SpawnEntity(new Entities.Enemies.Ripper(this, e.X, e.Y));
                break;

            case "hopper":
                SpawnEntity(new Entities.Enemies.Hopper(this, e.X, e.Y));
                break;

            default:
                throw new System.Exception(String.Format("Engine::MapSwitch Unknown entity type {0}", e.Type));
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

            if (ent.Y + ent.Height > ent2.Y && ent.Y < ent2.Y + ent2.Height) {
                return ent2;
            }

        }
        return null;
    }

    public Entity DetectInXCoords(Entity ent) {
        foreach (Entity ent2 in entities) {
            if (ent == ent2) continue;

            if (ent.X + ent.Width > ent2.X && ent.X < ent2.X + ent2.Width) {
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

    public BitmapSprite TabbySprite;
    public BitmapSprite RipperSprite;
    public BitmapSprite DoorSprite;
    public BitmapSprite BoomSprite;
    public BitmapSprite BulletSprite;
}
