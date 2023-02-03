using System;
using System.IO;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping.Implementation;
//package net.sf.scuba.tlv;

//import java.io.DataOutputStream;
//import java.io.IOException;
//import java.io.OutputStream;

/**
 * An output-stream for constructing TLV structures which wraps an existing
 * output-stream.
 *
 * Typical use is to first write a tag using {@code writeTag(int)},
 * and then:
 * <ul>
 *   <li>either directly write a value using {@code writeValue(byte[])}
 *       (which will cause the length and that value to be written),
 *   </li>
 *   <li>or use a series of lower-level output-stream {@code write} calls to write
 *       the value and terminate with a {@code writeValueEnd()}
 *       (which will cause the length and value to be computed and written).
 *   </li>
 * </ul>
 *
 * Nested structures can be constructed by writing new tags during value construction.
 *
 * @author Martijn Oostdijk (martijn.oostdijk@gmail.com)
 *
 * @version $Revision: 320 $
 */
public class TLVOutputStream //extends OutputStream
{

    private MemoryStream outputStream;
    private TLVOutputState state;

    ///**
    // * Constructs a TLV output-stream by wrapping an existing output-stream.
    // *
    // * @param outputStream the existing output-stream
    // */
    public TLVOutputStream(MemoryStream outputStream)
    {
        this.outputStream = outputStream;
        state = new TLVOutputState();
    }

    /**
     * Writes a tag to the output-stream (if TLV state allows it).
     *
     * @param tag the tag to write
     *
     * @throws IOException on error writing to the underlying output-stream
     */
    public void writeTag(int tag)
    {
        var tagAsBytes = TLVUtil.GetTagAsBytes(tag);
        if (state.canBeWritten())
            outputStream.Write(tagAsBytes);
        state.setTagProcessed(tag);
    }

    /**
     * Writes a length to the output-stream (if TLV state allows it).
     *
     * @param length the length to write
     *
     * @throws IOException on error writing to the underlying output-stream
     */
    public void writeLength(int length)
    {
        var lengthAsBytes = TLVUtil.GetLengthAsBytes(length);
        state.setLengthProcessed(length);
        if (state.canBeWritten())
            outputStream.Write(lengthAsBytes);
    }

    /**
     * Writes a value at once.
     * If no tag was previously written, an exception is thrown.
     * If no length was previously written, this method will write the length before writing <code>value</code>.
     * If length was previously written, this method will check whether the length is consistent with <code>value</code>'s length.
     *
     * @param value the value to write
     *
     * @throws IOException on error writing to the underlying output-stream
     */
    public void writeValue(byte[] value) //throws IOException
    {
        if (value == null)
            throw new ArgumentException("Cannot write null.");

        if (state.getIsAtStartOfTag())
            throw new InvalidOperationException("Cannot write value bytes yet. Need to write a tag first.");

        if (state.getIsAtStartOfLength())
        {
            writeLength(value.Length);
            write(value);
        }
        else
        {
            write(value);
            //TODO state.updatePreviousLength(value.Length);
        }
    }

    /**
     * Writes the specified byte to this output-stream.
     * Note that this can only be used for writing value bytes and
     * will throw an exception unless we have already written a tag.
     *
     * @param b the byte to write
     *
     * @throws IOException on error writing to the underlying output-stream
     */

    public /*override*/ void write(int b) //throws IOException
    {
        write(new byte[] { (byte)b }, 0, 1);
    }

    /**
     * Writes the specified bytes to this output-stream.
     * Note that this can only be used for writing value bytes and
     * will throw an exception unless we have already written a tag.
     *
     * @param bytes the bytes to write
     *
     * @throws IOException on error writing to the underlying output-stream
     */
    public /*override*/ void write(byte[] bytes)
    {
        write(bytes, 0, bytes.Length);
    }

    /**
     * Writes the specified number of bytes to this output-stream starting at the
     * specified offset.
     * Note that this can only be used for writing value bytes and
     * will throw an exception unless we have already written a tag.
     *
     * @param bytes the bytes to write
     * @param offset the offset
     * @param length the number of bytes to write
     *
     * @throws IOException on error writing to the underlying output-stream
     */
    public /*override*/ void write(byte[] bytes, int offset, int length)
    {
        if (state.getIsAtStartOfTag())
            throw new InvalidOperationException("Cannot write value bytes yet. Need to write a tag first.");
        if (state.getIsAtStartOfLength())
            state.setDummyLengthProcessed();
        state.updateValueBytesProcessed(bytes, offset, length);
        if (state.canBeWritten())
            outputStream.Write(bytes, offset, length);
    }

    /**
     * Marks the end of the value written thus far. This will adjust the length and
     * write the buffer to the underlying output-stream.
     *
     * @throws IOException on error writing to the underlying output-stream
     */
    public void writeValueEnd()
    {
        //if (state.isAtStartOfLength()) {
        //    throw new InvalidOperationException("Not processing value yet.");
        //}
        //if (state.isAtStartOfTag() && !state.isDummyLengthSet()) {
        //    return; /* TODO: check if this case ever happens. */
        //}
        //byte[]
        //bufferedValueBytes = state.getValue();
        //int length = bufferedValueBytes.Length;
        //state.updatePreviousLength(length);
        //if (state.canBeWritten()) {
        //    byte[] lengthAsBytes = TLVUtil.GetLengthAsBytes(length);
        //    outputStream.write(lengthAsBytes);
        //    outputStream.write(bufferedValueBytes);
        //}
    }

    /**
     * Flushes the underlying output-stream. Note that this does not
     * flush the value buffer if the current value has not been completed.
     *
     * @throws IOException on error writing to the underlying output-stream
     */

    public /*override*/ void flush()
    {
        //outputStream.flush();
    }

    /**
     * Closes this output-stream and releases any system resources
     * associated with this stream.
     *
     * @throws IOException on error writing to the underlying output-stream
     */
    public /*override*/ void close()
    {
        //if (!state.canBeWritten()) {
        //    throw new InvalidOperationException("Cannot close stream yet, illegal TLV state.");
        //}
        //outputStream.close();
    }
}
