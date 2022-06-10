using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
                        result[i] = (byte)0;
                    }
                    else
                    {
                        result[i] = toConvert[i];
                    }
                }
                else
                {
                    result[i] = (byte)0;
                }
            }

            return result;
        }

        unsafe static string bytePtrToStr(byte* toConvert, int length)
        {
            byte[] result = new byte[length];
            string strResult = "";
            bool fixFlag = false;

            for (int i = 0; i < length; i++)
            {
                if (!fixFlag)
                {
                    if (toConvert[i] == 0)
                    {
                        fixFlag = true;
                        result[i] = (byte)0;
                    }
                    else
                    {
                        result[i] = toConvert[i];
                    }
                }
                else
                {
                    result[i] = (byte)0;
                }
            }
            var size = System.Array.IndexOf(result, (byte)0);
            strResult = System.Text.Encoding.ASCII.GetString(result, 0, size < 0 ? length : size);
            return strResult;
        }

        unsafe static void clientThread()
        {
            using (MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateOrOpen("Global\\OVTMappingObject", BUF_SIZE, MemoryMappedFileAccess.ReadWriteExecute))
            {
                using (MemoryMappedViewAccessor viewAccessor = memoryMappedFile.CreateViewAccessor())
                {
                    using(Mutex mutex = new Mutex(false, "OVTMutexObject"))
                    {
                        int count = 0;
                        while (true)
                        {
                            mutex.WaitOne();

                            viewAccessor.Read<int>(0, out count);

                            OVObject[] serverArray = new OVObject[count];
                            viewAccessor.ReadArray<OVObject>(4, serverArray, 0, count);

                            mutex.ReleaseMutex();

                            Console.Clear();
                            Console.WriteLine("count: {0}", count);
                            for (int i = 0; i < count; i++)
                            {
                                fixed (OVObject* temp = &serverArray[i])
                                {
                                    string result = bytePtrToStr(temp->text, 128);

                                    Console.WriteLine("{0} | x: {1} | r: {2}", result, temp->x, temp->r);
                                }
                            }
                            System.Threading.Thread.Sleep(1);
                        }
                    }

                }
            }
        }

        unsafe static void Main(string[] args)
        {
            Console.WriteLine("CSC v0.9.4");

            Console.ReadLine();

            Thread t_clientThread = new Thread(clientThread);
            t_clientThread.Start();

            Console.ReadLine();
        }
    }
}