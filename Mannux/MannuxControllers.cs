using Import;
using Cataract;
using Sprites;

using System;
using System.IO;
using System.Collections;

class SpriteController : Controller {
    IGraph graph;

    public SpriteController(IGraph g) {
        graph = g;
    }

    protected override object Alloc(string fname) {
        return new BitmapSprite(graph, Import.ImageSprite.Load(fname));
    }

    protected override void DeallocAll() {
        foreach (DictionaryEntry d in resources) {
            Resource r = (Resource)d.Value;

            if (r != null)
                ((IDisposable)r.obj).Dispose();
        }
    }
}

class MapController : Controller {
    protected override object Alloc(string fname) {
        FileStream fs = new FileStream(fname, FileMode.Open);
        Map m = Import.MannuxMap.Load(fs);
        fs.Close();

        return m;
    }

    protected override void DeallocAll() {
        // ;P
    }
}

class TileSetController : Controller {
    IGraph graph;

    public TileSetController(IGraph g) {
        graph = g;
    }

    protected override object Alloc(string fname) {
        return new TileSet(graph, v2TileSet.Load(fname));
    }

    protected override void DeallocAll() {
        foreach (DictionaryEntry d in resources) {
            Resource r = (Resource)d.Value;

            if (r != null)
                ((IDisposable)r.obj).Dispose();
        }
    }
}

class SoundController : Controller {
    //Audiere.Context context=new Audiere.Context();

    protected override object Alloc(string fname) {
        return null;// new Audiere.Stream(context, fname);
    }

    protected override void DeallocAll() {
        /*foreach (DictionaryEntry d in resources)
        {
                Resource r=(Resource)d.Value;
			
                ((IDisposable)r.obj).Dispose();
        }*/
    }
}
