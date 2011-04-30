using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mannux.Import {
    // Pretends to be a collection, but lets you get at the underlying array if you want.
    // Like C++ std::vector
    class AutoArray<T> {
        private T[] data = null;
        private int length = 0;

        public AutoArray() {
            Reserve(8); // Arbitrary
        }

        private int Capacity {
            get {
                return data != null ? data.Length : 0;
            }
        }

        public void Realloc(int count) {
            Debug.Assert(count >= length);

            var newData = new T[count];
            for (var i = 0; i < length; ++i) {
                newData[i] = data[i];
            }
            data = newData;
        }

        public void Reserve(int count) {
            if (count > Capacity) {
                Realloc(count);
            }
        }

        public void Add(T t) {
            if (length == Capacity) {
                Reserve(length * 2);
            }
            data[length++] = t;
        }

        public void RemoveAt(int index) {
            Debug.Assert(0 <= index && index < Length);
            for (var i = index; i < length; ++i) {
                data[i] = data[i + 1];
            }
        }

        public int Length {
            get { return length; }
        }

        public T this[int index] {
            get {
                return data[index];
            }
            set {
                data[index] = value;
            }
        }

        public T[] ToArray() {
            var result = new T[length];
            System.Array.Copy(data, result, length);
            return result;
        }

        public T[] Array {
            get {
                return data;
            }
        }
    }
}
