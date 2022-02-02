using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSport.API.Classes {
    public class QueryParameters {
        const int _max_size = 100;
        private int _size = 50;

        public int Page { get; set; }
        public int Size {
            get {
                return _size;
            }
            set {
                _size = Math.Min(_max_size, value);
            }
        }
    }
}
