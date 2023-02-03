using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Wrapping.Implementation
{
    public class CommandApdu
    {
        //private static int MAX_APDU_SIZE = 65544;

        private readonly byte[] _Content;

        //index of start of data within the _Content array
        private readonly byte _DataOffset;

        private void checkArrayBounds(byte[] b, int ofs, int len)
        {
            if (ofs < 0 || len < 0)
                throw new ArgumentException("Offset and length must not be negative");

            if (b == null)
            {
                if (ofs != 0 && len != 0)
                    throw new ArgumentException("offset and length must be 0 if array is null");
            }
            else
            {
                if (ofs > b.Length - len)
                    throw new ArgumentException("Offset plus length exceed array size");
            }
        }

        public CommandApdu(int cla, int ins, int p1, int p2, int ne) : this(cla, ins, p1, p2, null, 0, 0, ne)
        {
        }
        public CommandApdu(int cla, int ins, int p1, int p2, byte[] data, int ne) : this(cla, ins, p1, p2, data, 0, data?.Length ?? 0, ne)
        {
        }

        private CommandApdu(int cla, int ins, int p1, int p2, byte[] data, int dataOffset, int dataLength, int ne)
        {
            checkArrayBounds(data, dataOffset, dataLength);
            if (dataLength > 65535)
                throw new ArgumentException("dataLength is too large");

            if (ne < 0 || ne > 65536)
                throw new ArgumentOutOfRangeException(nameof(ne));

            Ne = ne;

            Nc = dataLength;

            if (dataLength == 0)
            {
                if (ne == 0)
                {
                    // case 1
                    _Content = new byte[4];
                    SetHeader(cla, ins, p1, p2);
                }
                else
                {
                    // case 2s or 2e
                    if (ne <= 256)
                    {
                        // case 2s
                        // 256 is encoded as 0x00
                        var len = (byte)(ne != 256 ? ne : 0);
                        _Content = new byte[5];
                        SetHeader(cla, ins, p1, p2);
                        _Content[4] = len;
                    }
                    else
                    {
                        // case 2e
                        byte l1, l2;
                        // 65536 is encoded as 0x00 0x00
                        if (ne == 65536)
                        {
                            l1 = 0;
                            l2 = 0;
                        }
                        else
                        {
                            l1 = (byte)(ne >> 8);
                            l2 = (byte)ne;
                        }
                        _Content = new byte[7];
                        SetHeader(cla, ins, p1, p2);
                        _Content[5] = l1;
                        _Content[6] = l2;
                    }
                }
            }
            else
            {
                if (ne == 0)
                {
                    //// case 3s or 3e
                    //if (dataLength <= 255)
                    //{
                    //    // case 3s
                    //    _Content = new byte[4 + 1 + dataLength];
                    //    SetHeader(cla, ins, p1, p2);
                    //    _Content[4] = (byte)dataLength;
                    //    _DataOffset = 5;
                    //    Array.Copy(data, dataOffset, _Content, 5, dataLength);
                    //}
                    //else
                    //{
                    //    // case 3e
                    //    _Content = new byte[4 + 3 + dataLength];
                    //    SetHeader(cla, ins, p1, p2);
                    //    _Content[4] = 0;
                    //    _Content[5] = (byte)(dataLength >> 8);
                    //    _Content[6] = (byte)dataLength;
                    //    _DataOffset = 7;
                    //    Array.Copy(data, dataOffset, _Content, 7, dataLength);
                    //}
                }
                else
                {
                    // case 4s or 4e
                    if (dataLength <= 255 && ne <= 256)
                    {
                        // case 4s
                        _Content = new byte[4 + 2 + dataLength];
                        SetHeader(cla, ins, p1, p2);
                        _Content[4] = (byte)dataLength;
                        _DataOffset = 5;
                        Array.Copy(data, dataOffset, _Content, 5, dataLength);
                        _Content[^1] = (byte)(ne != 256 ? ne : 0);
                    }
                    else
                    {
                        // case 4e
                        _Content = new byte[4 + 5 + dataLength];
                        SetHeader(cla, ins, p1, p2);
                        _Content[4] = 0;
                        _Content[5] = (byte)(dataLength >> 8);
                        _Content[6] = (byte)dataLength;
                        _DataOffset = 7;
                        Array.Copy(data, dataOffset, _Content, 7, dataLength);
                        if (ne != 65536)
                        {
                            var leOfs = _Content.Length - 2;
                            _Content[leOfs] = (byte)(ne >> 8);
                            _Content[leOfs + 1] = (byte)ne;
                        } // else le == 65536: no need to fill in, encoded as 0
                    }
                }
            }
        }

        private void SetHeader(int cla, int ins, int p1, int p2)
        {
            _Content[0] = (byte)cla;
            _Content[1] = (byte)ins;
            _Content[2] = (byte)p1;
            _Content[3] = (byte)p2;
        }

        /// <summary>
        /// class byte CLA.
        /// </summary>
        public int CLA => _Content[0];

        /// <summary>
        /// class byte CLA.
        /// </summary>
        public int INS => _Content[1];

        /// <summary>
        /// parameter byte P1
        /// </summary>
        public int P1 => _Content[2] & 0xff;

        /// <summary>
        /// parameter byte P2.
        /// </summary>
        public int P2 => _Content[3] & 0xff;

        /**
         * Returns the number of data bytes in the command body (Nc) or 0 if this
         * APDU has no body. This call is equivalent to
         * <code>GetData().Length</code>.
         *
         * @return the number of data bytes in the command body or 0 if this APDU
         * has no body.
         */

        /// <summary>
        /// class byte CLA.
        /// </summary>
        public int Nc { get; }

        /**
         * Returns a copy of the data bytes in the command body. If this APDU as
         * no body, this method returns a byte array with length zero.
         *
         * @return a copy of the data bytes in the command body or the empty
         *    byte array if this APDU has no body.
         */
        //TODO pretty sure read length is not command 'data'
        public byte[] GetData()
        {
            var data = new byte[Nc];
            Array.Copy(_Content, _DataOffset, data, 0, Nc);
            return data;
        }

        /**
         * Returns the maximum number of expected data bytes in a response
         * APDU (Ne).
         *
         * @return the maximum number of expected data bytes in a response APDU.
         */
        public int Ne { get; }

        /// <summary>
        /// Contents
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return _Content;
            //return Arrays.CopyOf(_Content, _Content.Length);
        }

        /**
         * Returns a string representation of this command APDU.
         *
         * @return a string representation of this command APDU.
         */
        //public override string ToString()
        //{
        //    return "CommmandAPDU: " + _Content.Length + " bytes, _Nc=" + _Nc + ", _Ne=" + _Ne;
        //}

        /**
         * Compares the specified object with this command APDU for equality.
         * Returns true if the given object is also a CommandApdu and its bytes are
         * identical to the bytes in this CommandApdu.
         *
         * @param obj the object to be compared for equality with this command APDU
         * @return true if the specified object is equal to this command APDU
         */
        //public override bool Equals(object obj)
        //{
        //    if (this == obj)
        //    {
        //        return true;
        //    }
        //    if (obj instanceof CommandApdu == false) {
        //        return false;
        //    }
        //    CommandApdu other = (CommandApdu)obj;
        //    return Arrays.equals(_Content, other._Content);
        //}

        /**
         * Returns the hash code value for this command APDU.
         *
         * @return the hash code value for this command APDU.
         */
        //      @Override
        //public int hashCode()
        //      {
        //          return Arrays.hashCode(_Content);
        //      }

        //private void readObject(java.io.ObjectInputStream in)
        //{
        //      _Content = (byte[])in.readUnshared();
        //      // initialize transient fields
        //      parse();
        //  }

    }

}

