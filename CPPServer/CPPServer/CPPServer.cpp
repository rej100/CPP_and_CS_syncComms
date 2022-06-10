#include <iostream>
#include <string>

#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <conio.h>
#include <tchar.h>

#define OV_EMPTY 0
#define OV_LINE 1
#define OV_RECT 2
#define OV_RECT_FILL 3
#define OV_TEXT 4
//164*2000 = 328 000
#define BUF_SIZE 328000

TCHAR szName[] = TEXT("Global\\OVTMappingObject");

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
    std::cout << "CPP Server v0.9.5" << std::endl;

    //mapped file prep begin
    HANDLE hMapFile;
    DWORD64 pBuf;

    hMapFile = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, BUF_SIZE, szName);                 

    if (hMapFile == NULL)
    {
        _tprintf(TEXT("Could not create file mapping object (%d).\n"), GetLastError());
        return 1;
    }

    pBuf = (DWORD64)MapViewOfFile(hMapFile, FILE_MAP_ALL_ACCESS, 0, 0, BUF_SIZE);

    if (pBuf == NULL)
    {
        _tprintf(TEXT("Could not map view of file (%d).\n"), GetLastError());
        CloseHandle(hMapFile);
        return 1;
    }
    //mapped file prep end

    //mutex prep begin
    HANDLE hMutex;

    hMutex = CreateMutex(NULL, FALSE, TEXT("OVTMutexObject"));

    if (hMutex == NULL)
    {
        printf("CreateMutex error: %d\n", GetLastError());
    }
    else if (GetLastError() == ERROR_ALREADY_EXISTS)
    {
        printf("CreateMutex opened an existing mutex\n");
    }
    else
    {
        printf("CreateMutex created a new mutex.\n");
    }
    //mutex prep end

    srand(static_cast <unsigned> (time(0)));

    int countTest = 5;

    OVObject* cinArray;

    std::cin.get();
    while (true)
    {
        countTest = (rand() % 10) + 1;
        cinArray = new OVObject[countTest];

        for (int i = 0; i < countTest; i++)
        {
            std::string ftestObjStrCin;
            char ftestObjTxtCin[128];

            ftestObjStrCin = "TSTOBJnumber " + std::to_string(i);
            strcpy_s(ftestObjTxtCin, ftestObjStrCin.c_str());

            float randX = 0.1 + static_cast<float>(rand()) / (static_cast<float>(RAND_MAX / (4321.4321)));
            short randR = static_cast<short>(rand() % 256);

            cinArray[i] = constructOVO(OV_LINE, randX, 1.2f, randR, 2, 3, 4, 2.1f, 2.2f, 100.1f, 100.2f, ftestObjTxtCin);
        }

        WaitForSingleObject(hMutex, INFINITE);
        CopyMemory((PVOID)(pBuf), &countTest, 4);
        CopyMemory((PVOID)(pBuf + 4), cinArray, sizeof(OVObject) * countTest);
        ReleaseMutex(hMutex);

        system("cls");
        std::cout << "count: " << countTest << std::endl;
        for (int i = 0; i < countTest; i++)
        {
            std::cout << cinArray[i].text << " | x: " << cinArray[i].x << " | r: " << cinArray[i].r << std::endl;
        }
        Sleep(1);
    }

    std::cout << "copied" << std::endl;
    _getch();

    std::cout << "overinnit" << std::endl;

    if ( UnmapViewOfFile((LPCVOID)pBuf) == 0 )
    {
        std::cout << "unmap failed" << std::endl;
    }

    CloseHandle(hMapFile);
    CloseHandle(hMutex);
    

    std::cin.get();

}