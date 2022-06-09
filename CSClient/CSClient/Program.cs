using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

//#define OV_EMPTY 0
//#define OV_LINE 1
//#define OV_RECT 2
//#define OV_RECT_FILL 3
//#define OV_TEXT 4

////164*128
//#define BUF_SIZE 20992

namespace CSClient
{
    unsafe struct OVObject
    {
        public short type;

        public float x;
        public float y;

        public short r;
        public short g;
        public short b;
        public short a;

        public float xTo;
        public float yTo;

        public float width;
        public float length;

        public fixed byte text[128];
    };

    internal class Program
    {
        const int OV_EMPTY = 0;
        const int OV_LINE = 0;
        const int OV_RECT = 0;
        const int OV_RECT_FILL = 0;
        const int OV_TEXT = 0;

        const int BUF_SIZE = 20992;
        unsafe static void Main(string[] args)
        {
            Console.WriteLine("CSC v0.2.8");
            Console.WriteLine("the size of short is: {0}", sizeof(short));
            Console.WriteLine("the size of float is: {0}", sizeof(float));
            Console.WriteLine("the size of byte is: {0}", sizeof(byte));
            Console.WriteLine("the size of OVObject is: {0}", sizeof(OVObject));

            OVObject[] serverArray = new OVObject[128];

            Console.ReadLine();

            //using (MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateOrOpen("XboxGameBarPoc_SharedMemory", 0x644, MemoryMappedFileAccess.ReadWriteExecute))
            //OVTMappingObject
            using (MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateOrOpen("Global\\OVTMappingObject", BUF_SIZE, MemoryMappedFileAccess.ReadWriteExecute))
            {
                using (MemoryMappedViewAccessor viewAccessor = memoryMappedFile.CreateViewAccessor())
                {
                    viewAccessor.ReadArray<OVObject>(0, serverArray, 0, 128);
                    Console.WriteLine("r is: {0}", serverArray[127].r);
                    //int count = 0;
                    //viewAccessor.Read<int>(0, out count);
                    //Box[] boxArray = new Box[count];
                    //viewAccessor.ReadArray<Box>(4, boxArray, 0, count);
                }
            }

            Console.ReadLine();
        }
    }
}
