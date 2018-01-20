using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleBoi
{
    class ByteStack : Stack<byte>
    {
        public ByteStack(int capacity) : base(capacity)
        {
            
        }

        public void Push(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                this.Push(bytes[i]);
            }
        }
    }
}
