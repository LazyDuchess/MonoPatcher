// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "Core.h"
#include <iostream>

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        DisableThreadLibraryCalls(hModule);
        if (!Core::Create()) {
            printf("Failed to initialize Mono Patcher CPP Core!\n");
            FreeLibraryAndExitThread(hModule, 0);
            return TRUE;
        }
        printf("Mono Patcher CPP Core initialized.\n");
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

