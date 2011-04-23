namespace Import {
    using System;
    using System.Collections;
    using Geo;


    [Serializable()]
    public class PointCollection : CollectionBase {

        public PointCollection() {
        }

        public PointCollection(PointCollection value) {
            this.AddRange(value);
        }

        public PointCollection(Vertex[] value) {
            this.AddRange(value);
        }

        public Vertex this[int index] {
            get {
                return ((Vertex)(List[index]));
            }
            set {
                List[index] = value;
            }
        }

        public int Add(Vertex value) {
            return List.Add(value);
        }

        public void AddRange(Vertex[] value) {
            for (int i = 0; (i < value.Length); i = (i + 1)) {
                this.Add(value[i]);
            }
        }

        public void AddRange(PointCollection value) {
            for (int i = 0; (i < value.Count); i = (i + 1)) {
                this.Add(value[i]);
            }
        }

        public bool Contains(Vertex value) {
            return List.Contains(value);
        }

        public void CopyTo(Vertex[] array, int index) {
            List.CopyTo(array, index);
        }

        public int IndexOf(Vertex value) {
            return List.IndexOf(value);
        }

        public void Insert(int index, Vertex value) {
            List.Insert(index, value);
        }

        public new PointEnumerator GetEnumerator() {
            return new PointEnumerator(this);
        }

        public void Remove(Vertex value) {
            List.Remove(value);
        }

        public class PointEnumerator : object, IEnumerator {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public PointEnumerator(PointCollection mappings) {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public Vertex Current {
                get {
                    return ((Vertex)(baseEnumerator.Current));
                }
            }

            object IEnumerator.Current {
                get {
                    return baseEnumerator.Current;
                }
            }

            public bool MoveNext() {
                return baseEnumerator.MoveNext();
            }

            bool IEnumerator.MoveNext() {
                return baseEnumerator.MoveNext();
            }

            public void Reset() {
                baseEnumerator.Reset();
            }

            void IEnumerator.Reset() {
                baseEnumerator.Reset();
            }
        }
    }
}
