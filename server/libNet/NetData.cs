using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
namespace libNet
{
    public class NetData : IDisposable
    {
        private byte[] _data;
        private int _position;
        private int _length;
        private int _capacity;

        public NetData()
        {
            _data = new byte[0];
            _position = 0;
            _length = 0;
            _capacity = 0;
        }
        public int Length { get { return _length; } }

        private byte ReadByte()
        {
            if (this._position >= this._length)
            {
                return 0;
            }
            return this._data[this._position++];
        }

        private int ReadInt()
        {
            int num = this._position += 4;
            if (num > this._length)
            {
                this._position = this._length;
                return -1;
            }
            return (((this._data[num - 4] | (this._data[num - 3] << 8)) | (this._data[num - 2] << 0x10)) | (this._data[num - 1] << 0x18));
        }

        private byte[] ReadBytes(int count)
        {
            int num = this._length - this._position;
            if (num > count)
            {
                num = count;
            }
            if (num <= 0)
            {
                return null;
            }
            byte[] buffer = new byte[num];
            if (num <= 8)
            {
                int num2 = num;
                while (--num2 >= 0)
                {
                    buffer[num2] = this._data[this._position + num2];
                }
            }
            else
            {
                Buffer.BlockCopy(this._data, this._position, buffer, 0, num);
            }
            this._position += num;
            return buffer;
        }

        public bool Read(out Message message)
        {
            message = null;
            _position = 0;
            if (_length > 4)
            {
                message = Message.GetOne();
                message.Size = ReadInt();
                if (message.Size <= 0 || message.Size <= _length - _position)
                {
                    if (message.Size > 0)
                    {
                        message.Body = ReadBytes(message.Size);
                    }
                    Remove(message.Size + 4);
                    return true;
                }
                else
                {
                    message = null;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void EnsureCapacity(int value)
        {
            if (value < this._capacity)
                return;
            int num1 = value;
            if (num1 < 0x100)
                num1 = 0x100;
            if (num1 < (this._capacity * 2))
                num1 = this._capacity * 2;
            byte[] buffer1 = new byte[num1];
            if (this._length > 0)
                Buffer.BlockCopy(this._data, 0, buffer1, 0, this._length);
            this._data = buffer1;
            this._capacity = num1;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (buffer.Length - offset < count)
            {
                count = buffer.Length - offset;
            }
            EnsureCapacity(buffer.Length + _length);
            Array.Clear(_data, _length, _capacity - _length);
            Buffer.BlockCopy(buffer, offset, _data, _length, count);
            _length += count;
        }

        private void Remove(int count)
        {
            if (_length >= count)
            {
                Buffer.BlockCopy(_data, count, _data, 0, _length - count);
                _length -= count;
                Array.Clear(_data, _length, _capacity - _length);
            }
            else
            {
                _length = 0;
                Array.Clear(_data, 0, _capacity);
            }
        }

        public void Dispose()
        {
            _capacity = 0;
            _position = 0;
            _length = 0;
            _data = null;
            _data = new byte[0];
            Pool.Push(this);
        }

        public void Clear()
        {
            _position = 0;
            Array.Clear(_data, 0, _data.Length);
        }

        internal NetDataPool Pool;

    }
}
