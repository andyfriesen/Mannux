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

        public PointCollection(Point[] value) {
            this.AddRange(value);
        }

        public Point this[int index] {
            get {
                return ((Point)(List[index]));
            }
            set {
                List[index] = value;
            }
        }

        public int Add(Point value) {
            return List.Add(value);
        }

        public void AddRange(Point[] value) {
            for (int i = 0; (i < value.Length); i = (i + 1)) {
                this.Add(value[i]);
            }
        }

        public void AddRange(PointCollection value) {
            for (int i = 0; (i < value.Count); i = (i + 1)) {
                this.Add(value[i]);
            }
        }

        public bool Contains(Point value) {
            return List.Contains(value);
        }

        public void CopyTo(Point[] array, int index) {
            List.CopyTo(array, index);
        }

        public int IndexOf(Point value) {
            return List.IndexOf(value);
        }

        public void Insert(int index, Point value) {
            List.Insert(index, value);
        }

        public new PointEnumerator GetEnumerator() {
            return new PointEnumerator(this);
        }

        public void Remove(Point value) {
            List.Remove(value);
        }

        public class PointEnumerator : object, IEnumerator {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public PointEnumerator(PointCollection mappings) {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public Point Current {
                get {
                    return ((Point)(baseEnumerator.Current));
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
