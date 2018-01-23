using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleBoi
{
    class ByteStack
    {
        private int capacity;
        List<byte> allBytes = new List<byte>();
        public ByteStack(int capacity)
        {
            this.capacity = capacity;
        }

        public void Push(byte[] bytes)
        {
            this.allBytes.AddRange(bytes); 
        }

        public byte[] GetBytes()
        {
            allBytes.AddRange(new byte[101 - allBytes.Count]);
            return allBytes.ToArray();
        }
    }
}
