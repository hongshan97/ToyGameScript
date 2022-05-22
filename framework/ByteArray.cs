using System;
public class ByteArray {
    public const int DEFAULT_SIZE = 1024;
    private int initSize = 0; // 初始大小
    public byte[] bytes; // buffer

    public int readIdx = 0, writeIdx = 0; // 读写位置
    private int capacity = 0; // 当前容量
    public int remain { get { return capacity - writeIdx; } } // 后面剩余空间
    public int length { get{ return writeIdx - readIdx; } } // 未处理数据长度

    public ByteArray(int size = DEFAULT_SIZE) {
        bytes = new byte[size];
        initSize = capacity = size;
        readIdx = writeIdx = 0;
    }
    
    public ByteArray(byte[] defaultBytes) {
        bytes = defaultBytes;
        initSize = capacity = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }

    // 扩容
    public void ReSize(int size) {
        if(size < length || size < initSize)
            return;
        for(capacity = 1; capacity < size; capacity <<= 1);
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIdx, newBytes, 0, length);
        bytes = newBytes;
        readIdx = 0;
        writeIdx = length;
    }

    // 判断未处理数据是否在不影响性能情况下前移，避免短的未处理数据占据byts后端从而remain太小
    public void CheckAndMoveBytes() {
        if(length < 8) {  // 8合适吗？
            if(length > 0)
                Array.Copy(bytes, readIdx, bytes, 0, length);

            writeIdx = length;
            readIdx = 0;
        }
    }

    public void MoveBytes() {
        if(length > 0)
            Array.Copy(bytes, readIdx, bytes, 0, length);
    }

    // 往ByteArray写入
    public int Write(byte[] bs, int offset, int count) {
        if(remain < count)
            ReSize(length + count); // 注意要加length
        Array.Copy(bs, offset, bytes, writeIdx, count);
        writeIdx = length + count;
        return count;
    }

    // 读取
    // public int Read(byte[] bs, int offset, int count) {
    //     count = Math.Min(count, length);
    //     Array.Copy(bytes, readIdx, bs, offset, count);
    //     readIdx += count;
    //     CheckAndMoveBytes();
    //     return count;
    // }
}
