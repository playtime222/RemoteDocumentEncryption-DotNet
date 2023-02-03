using System;
using System.Collections.Generic;
using System.IO;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Wrapping.Implementation
{
    class TLVOutputState
    {
        ///**
        // * Encoded the tags, lengths, and (partial) values.
        // */
        private readonly Queue<TLVStruct> _State = new();

        //      /*
        //       * Encoded position, only one can be true.
        //       *
        //       * TFF: ^TLVVVVVV
        //       * FTF: T^LVVVVVV
        //       * FFT: TL^VVVVVV
        //       * FFT: TLVVVV^VV
        //       * TFF: ^
        //       */
        private bool _IsAtStartOfTag;
        private bool _IsAtStartOfLength;
        //private bool _IsReadingValue;

        //      public TLVOutputState()
        //      {
        //          this(new ArrayDeque<TLVStruct>(), true, false, false);
        //      }

        //      public TLVOutputState(TLVOutputState original)
        //      {
        //          this(original.getDeepCopyOfState(), original.isAtStartOfTag, original.isAtStartOfLength, original.isReadingValue);
        //      }

        //      private TLVOutputState(Deque<TLVStruct> state, boolean isAtStartOfTag, boolean isAtStartOfLength, boolean isReadingValue)
        //      {
        //          this.state = state;
        //          this.isAtStartOfTag = isAtStartOfTag;
        //          this.isAtStartOfLength = isAtStartOfLength;
        //          this.isReadingValue = isReadingValue;
        //      }

        public bool getIsAtStartOfTag()
        {
            return _IsAtStartOfTag;
        }

        public bool getIsAtStartOfLength()
        {
            return _IsAtStartOfLength;
        }

        public void setTagProcessed(int tag)
        {
            /* Length is set to MAX INT, we will update it when caller calls our setLengthProcessed. */
            var obj = new TLVStruct(tag);
            if (_State.Count != 0)
            {
                var parent = _State.Peek();
                var tagBytes = TLVUtil.GetTagAsBytes(tag);
                parent.write(tagBytes, 0, tagBytes.Length);
            }
            _State.Enqueue(obj);
            _IsAtStartOfTag = false;
            _IsAtStartOfLength = true;
            //_IsReadingValue = false;
        }

        public void setDummyLengthProcessed()
        {
            _IsAtStartOfTag = false;
            _IsAtStartOfLength = false;
            //_IsReadingValue = true;
            /* NOTE: doesn't call setLength, so that isLengthSet in stackFrame will remain false. */
        }

        public bool isDummyLengthSet()
        {
            if (_State.Count == 0)
                return false;
            return !_State.Peek().isLengthSet();
        }

        public void setLengthProcessed(int length)
        {
            if (length < 0)
                throw new ArgumentException("Cannot set negative length (length = " + length + ").");

            var obj = _State.Dequeue();
            if (_State.Count != 0)
            {
                var parent = _State.Peek();
                var lengthBytes = TLVUtil.GetLengthAsBytes(length);
                parent.write(lengthBytes, 0, lengthBytes.Length);
            }
            obj.setLength(length);
            _State.Enqueue(obj);
            _IsAtStartOfTag = false;
            _IsAtStartOfLength = false;
            //_IsReadingValue = true;
        }

        //      public void updatePreviousLength(int byteCount)
        //      {
        //          if (state.isEmpty())
        //          {
        //              return;
        //          }
        //          TLVStruct currentObject = state.peek();

        //          if (currentObject.isLengthSet && currentObject.getLength() == byteCount)
        //          {
        //              return;
        //          }

        //          currentObject.setLength(byteCount);

        //          if (currentObject.getValueBytesProcessed() == currentObject.getLength())
        //          {
        //              /* Update parent. */
        //              state.pop();
        //              byte[] lengthBytes = TLVUtil.GetLengthAsBytes(byteCount);
        //              byte[] value = currentObject.getValue();
        //              updateValueBytesProcessed(lengthBytes, 0, lengthBytes.Length);
        //              updateValueBytesProcessed(value, 0, value.Length);
        //              isAtStartOfTag = true;
        //              isAtStartOfLength = false;
        //              isReadingValue = false;
        //          }
        //      }

        public void updateValueBytesProcessed(byte[] bytes, int offset, int length)
        {
            if (_State.Count == 0)
                return;
            var currentObject = _State.Peek();
            var bytesLeft = currentObject.getLength() - currentObject.getValueBytesProcessed();
            if (length > bytesLeft)
                throw new ArgumentException("Cannot process " + length + " bytes! Only " + bytesLeft + " bytes left in this TLV object " + currentObject);

            currentObject.write(bytes, offset, length);

            if (currentObject.getValueBytesProcessed() == currentObject.getLength())
            {
                /* Stand back! I'm going to try recursion! Update parent(s)... */
                _State.Dequeue();
                updateValueBytesProcessed(currentObject.getValue(), 0, currentObject.getLength());
                _IsAtStartOfTag = true;
                _IsAtStartOfLength = false;
                //_IsReadingValue = false;
            }
            else
            {
                /* We already have these values?!? */
                _IsAtStartOfTag = false;
                _IsAtStartOfLength = false;
                //_IsReadingValue = true;
            }
        }

        //      public byte[] getValue()
        //      {
        //          if (state.isEmpty())
        //          {
        //              throw new InvalidOperationException("Cannot get value yet.");
        //          }
        //          return state.peek().getValue();
        //      }

        //      @Override
        //    public string toString()
        //      {
        //          return state.toString();
        //      }

        //      /*
        //       * TODO: ?? canBeWritten() <==> (state.size() == 1 && state.peek().isLengthSet()
        //       */
        public bool canBeWritten()
        {
            foreach (var stackFrame in _State)
            {
                if (!stackFrame.isLengthSet())
                    return false;
            }
            return true;
        }

        //      private Deque<TLVStruct> getDeepCopyOfState()
        //      {
        //          Deque<TLVStruct> newStack = new ArrayDeque<TLVStruct>(state.size());
        //          for (TLVStruct tlvStruct: state)
        //          {
        //              newStack.add(new TLVStruct(tlvStruct));
        //          }
        //          return newStack;
        //      }

        private class TLVStruct
        {

            private int tag;
            private int length;
            private bool _isLengthSet;
            private MemoryStream value;

            public TLVStruct(TLVStruct original) : this(original.tag, original.length, original._isLengthSet, original.getValue())
            {
            }

            public TLVStruct(int tag) : this(tag, int.MaxValue, false, null)
            {
            }

            public TLVStruct(int tag, int length, bool isLengthSet, byte[] value)
            {
                this.tag = tag;
                this.length = length;
                _isLengthSet = isLengthSet;
                this.value = new MemoryStream();
                if (value != null)
                    this.value.Write(value);
            }

            public void setLength(int length)
            {
                this.length = length;
                _isLengthSet = true;
            }

            public int getTag()
            {
                return tag;
            }

            public int getLength()
            {
                return length;
            }

            public bool isLengthSet()
            {
                return _isLengthSet;
            }

            public long getValueBytesProcessed()
            {
                return value.Length;
            }

            public byte[] getValue()
            {
                return value.ToArray();
            }

            public void write(byte[] bytes, int offset, int length)
            {
                value.Write(bytes, offset, length);
            }

            //  @Override
            //public string toString()
            //  {
            //      byte[] valueBytes = value.toByteArray();
            //      return "[TLVStruct " + Integer.toHexString(tag) + ", " + (isLengthSet ? length : "UNDEFINED") + ", " + Hex.bytesToHexString(valueBytes) + "(" + valueBytes.Length + ") ]";
            //  }
        }
    }
}
