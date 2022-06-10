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
////164*2000 = 328 000
//#define BUF_SIZE 328000

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
        const int OV_LINE = 1;
        const int OV_RECT = 2;
        const int OV_RECT_FILL = 3;
        const int OV_TEXT = 4;

        const int BUF_SIZE = 328000;

        unsafe static byte[] bytePtrToArr(byte* toConvert, int length)
        {
            byte[] result = new byte[length];
            bool fixFlag = false;

            for (int i = 0; i < length; i++)
            {
                if(!fixFlag)
                {
                    if (toConvert[i] == 0)
                    {
                        fixFlag = true;
                    }
                    result[i] = toConvert[i];
                }
                else
                {
                    result[i] = 0;
                }
            }

            return result;
        }
        unsafe static void Main(string[] args)
        {
            Console.WriteLine("CSC v0.8.2");

            Console.ReadLine();

            using (MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateOrOpen("Global\\OVTMappingObject", BUF_SIZE, MemoryMappedFileAccess.ReadWriteExecute))
            {
                using (MemoryMappedViewAccessor viewAccessor = memoryMappedFile.CreateViewAccessor())
                {
                    int count = 0;
                    while(true)
                    {
                        viewAccessor.Read<int>(0, out count);

                        OVObject[] serverArray = new OVObject[count];
                        viewAccessor.ReadArray<OVObject>(4, serverArray, 0, count);

                        Console.Clear();
                        Console.WriteLine("count: {0}", count);
                        for (int i = 0; i < count; i++)
                        {
                            fixed (OVObject* temp = &serverArray[i])
                            {
                                byte[] tempByteArr = new byte[128];
                                tempByteArr = bytePtrToArr(temp->text, 128);
                                string result = System.Text.Encoding.ASCII.GetString(tempByteArr);

                                Console.WriteLine("{0} | x: {1} | r: {2}", result, temp->x, temp->r);
                            }
                        }
                        //System.Threading.Thread.Sleep(500);
                    }

                }
            }

            Console.ReadLine();
        }
    }
}

//namespace CSClient
//{
//    unsafe struct OVObject
//    {
//        public short type;

//        public float x;
//        public float y;

//        public short r;
//        public short g;
//        public short b;
//        public short a;

//        public float xTo;
//        public float yTo;

//        public float width;
//        public float length;

//        public fixed byte text[128];
//    };

//    internal class Program
//    {
//        const int OV_EMPTY = 0;
//        const int OV_LINE = 1;
//        const int OV_RECT = 2;
//        const int OV_RECT_FILL = 3;
//        const int OV_TEXT = 4;

//        const int BUF_SIZE = 328000;

//        unsafe static byte[] bytePtrToArr(byte* toConvert, int length)
//        {
//            byte[] result = new byte[length];
//            bool fixFlag = false;

//            for (int i = 0; i < length; i++)
//            {
//                if (!fixFlag)
//                {
//                    if (toConvert[i] == 0)
//                    {
//                        fixFlag = true;
//                    }
//                    result[i] = toConvert[i];
//                }
//                else
//                {
//                    result[i] = 0;
//                }
//            }

//            return result;
//        }
//        unsafe static void Main(string[] args)
//        {
//            Console.WriteLine("CSC v0.7.1");
//            Console.WriteLine("the size of short is: {0}", sizeof(short));
//            Console.WriteLine("the size of float is: {0}", sizeof(float));
//            Console.WriteLine("the size of byte is: {0}", sizeof(byte));
//            Console.WriteLine("the size of OVObject is: {0}", sizeof(OVObject));


//            Console.ReadLine();

//            using (MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateOrOpen("Global\\OVTMappingObject", BUF_SIZE, MemoryMappedFileAccess.ReadWriteExecute))
//            {
//                using (MemoryMappedViewAccessor viewAccessor = memoryMappedFile.CreateViewAccessor())
//                {
//                    int count = 0;
//                    viewAccessor.Read<int>(0, out count);

//                    OVObject[] serverArray = new OVObject[count];
//                    viewAccessor.ReadArray<OVObject>(4, serverArray, 0, count);

//                    Console.WriteLine("count is: {0} | r of 0th object is: {1} | r of last object is {2}", count, serverArray[0].r, serverArray[count - 1].r);

//                    fixed (OVObject* temp = &serverArray[0])
//                    {

//                        byte[] tempByteArr = new byte[128];
//                        tempByteArr = bytePtrToArr(temp->text, 128);
//                        Console.WriteLine();
//                        for (int i = 0; i < 128; i++)
//                        {
//                            Console.Write(" ");
//                            Console.Write("{0}", tempByteArr[i]);
//                        }
//                        Console.WriteLine();
//                        string result = System.Text.Encoding.ASCII.GetString(tempByteArr);
//                        Console.WriteLine("first object says: {0}", result);
//                    }
//                    fixed (OVObject* temp = &serverArray[count - 1])
//                    {
//                        byte[] tempByteArr = new byte[128];
//                        tempByteArr = bytePtrToArr(temp->text, 128);
//                        string result = System.Text.Encoding.ASCII.GetString(tempByteArr);
//                        Console.WriteLine("last object says: {0}", result);
//                    }
//                }
//            }

//            Console.ReadLine();
//        }
//    }
//}