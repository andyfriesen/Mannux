using System;
using System.Collections;

// Generic resource controller

abstract class Controller : IDisposable {
    protected class Resource {
        public object obj;		//!< the actual resource
        public string fname;	//!< filename
        public int refcount;	//!< number of things using it

        public Resource(object o, string s) {
            obj = o;
            fname = s;
            refcount = 1;
        }
    }

    protected Hashtable resources = new Hashtable();

    Resource GetResourceFromObject(object o) {
        foreach (DictionaryEntry d in resources) {
            Resource r = (Resource)d.Value;

            if (r.obj == o)
                return r;
        }

        return null;
    }

    protected abstract object Alloc(string fname);

    public object Load(string fname) {
        Resource r = (Resource)resources[fname];

        if (r == null) {
            Console.WriteLine("Alloc {0}", fname);

            r = new Resource(Alloc(fname), fname);
            resources[fname] = r;
            return r.obj;
        }

        r.refcount++;
        return r.obj;
    }

    public void Free(IDisposable o) {
        Resource r = GetResourceFromObject(o);

        if (r == null) {
            return;	// !!!
        }

        Console.WriteLine("Decreffing {0}", r.fname);

        // TODO: see if it'd be worthwhile to make a countdown type dealie,
        // so that things aren't allocated, then immediately deallocated repeatedly.
        // Say, any resource not being used has 10 seconds to live before it's
        // allowed to be GC'd.
        r.refcount--;
        if (r.refcount == 0) {
            Console.WriteLine("Releasing {0}", r.fname);
            o.Dispose();
            resources.Remove(r.fname);
        }
    }

    protected abstract void DeallocAll();

    // IDisposable
    int disposestate = 0;
    public void Dispose() {
        if (disposestate != 0)
            return;

        disposestate = 1;

        DeallocAll();
        resources.Clear();

        disposestate = 2;
    }
}
