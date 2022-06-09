#include <iostream>
#include <string>

#include <windows.h>
#include <stdio.h>
#include <conio.h>
#include <tchar.h>

#define OV_EMPTY 0
#define OV_LINE 1
#define OV_RECT 2
#define OV_RECT_FILL 3
#define OV_TEXT 4

//164*128
#define BUF_SIZE 20992

TCHAR szName[] = TEXT("Global\\OVTMappingObject");
TCHAR szMsg[] = TEXT("Message from first process.");

//164 bytes of data
struct OVObject
{
    short type;

    float x;
    float y;

    short r;
    short g;
    short b;
    short a;

    float xTo;
    float yTo;

    float width;
    float length;

    char text[128];
};

OVObject constructOVO(short type, float x, float y, short r, short g, short b, short a, float xTo, float yTo, float width, float length, char* text)
{
    OVObject result;
    result.type = type;
    result.x = x;
    result.r = r;
    result.g = g;
    result.b = b;
    result.a = a;
    result.xTo = xTo;
    result.yTo = yTo;
    result.width = width;
    result.length = length;
    strcpy_s(result.text, text);
    
    return result;
}

int main()
{
    std::cout << "CPP Server v0.3.5" << std::endl;

    std::cout << sizeof(OVObject) << std::endl;

    OVObject serverArray[128];
    std::cout << "arraysize: " << sizeof(serverArray) << std::endl;
    
    char testObjTxt[128] = "xdTesting";

    OVObject testObj = constructOVO(OV_LINE, 1.1f, 1.2f, 1, 2, 3, 4, 2.1f, 2.2f, 100.1f, 100.2f, testObjTxt);

    std::cout << testObj.type << " " << testObj.width << " " << testObj.text << std::endl;

    


    for (int i = 0; i < 128; i++)
    {
        std::string ftestObjStr;
        char ftestObjTxt[128];

        ftestObjStr = "Test object number " + std::to_string(i);
        strcpy_s(ftestObjTxt, ftestObjStr.c_str());

        serverArray[i] = constructOVO(OV_LINE, 1.1f, 1.2f, (1 + i), 2, 3, 4, 2.1f, 2.2f, 100.1f, 100.2f, ftestObjTxt);
    }

    std::cout << serverArray[127].r << " " << serverArray[127].text << std::endl;

    HANDLE hMapFile;
    LPCTSTR pBuf;

    hMapFile = CreateFileMapping(
        INVALID_HANDLE_VALUE,    // use paging file
        NULL,                    // default security
        PAGE_READWRITE,          // read/write access
        0,                       // maximum object size (high-order DWORD)
        BUF_SIZE,                // maximum object size (low-order DWORD)
        szName);                 // name of mapping object

    if (hMapFile == NULL)
    {
        _tprintf(TEXT("Could not create file mapping object (%d).\n"),
            GetLastError());
        return 1;
    }
    pBuf = (LPTSTR)MapViewOfFile(hMapFile,   // handle to map object
        FILE_MAP_ALL_ACCESS, // read/write permission
        0,
        0,
        BUF_SIZE);

    if (pBuf == NULL)
    {
        _tprintf(TEXT("Could not map view of file (%d).\n"),
            GetLastError());

        CloseHandle(hMapFile);

        return 1;
    }


    CopyMemory((PVOID)pBuf, serverArray, sizeof(serverArray));
    _getch();

    std::cout << "overinnit" << std::endl;

    UnmapViewOfFile(pBuf);

    CloseHandle(hMapFile);

    std::cin.get();

}
