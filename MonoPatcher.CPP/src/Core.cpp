#include "Core.h"
#include "iostream"
#include <filesystem>
#include "MinHook.h"
#include "Sims3/ScriptHost.h"
#include "Addresses.h"
#include "mono.h"
#include "MonoHooks.h"

typedef int(__thiscall *INITIALIZESCRIPTHOST)(void* me);

INITIALIZESCRIPTHOST fpInitializeScriptHost = NULL;

// thiscall hooking hack
int __fastcall DetourInitializeScriptHost(void* me, void* _) {
	printf("Initializing ScriptHost");

	// We load plugins just before the script-host is initialized so that
	// plugins can initialize their own state before the script-host gets
	// a chance to call their internal Mono calls.
	// And we load them here, instead of in `Core::Initialize`, so that we're
	// not calling `LoadLibrary` within the context of our DLLMain.
	// (See: https://learn.microsoft.com/windows/win32/dlls/dynamic-link-library-best-practices)
	Core::GetInstance()->LoadPlugins();
	// We initialize each plugin after all the plugins have loaded so that
	// plugins can detect the presence of each other, and also so that they can
	// initialize their state outside of their DLL entrypoints.
	Core::GetInstance()->InitializePlugins();

	int result = fpInitializeScriptHost(me);
	MonoHooks::InitializeScriptHost();
	ScriptHost::GetInstance()->CreateMonoClass("MonoPatcherLib", "DLLEntryPoint");
	return result;
}

Core* Core::_instance = nullptr;

Core* Core::GetInstance() {
	return _instance;
}

bool Core::Create() {
	_instance = new Core();
	return _instance->Initialize();
}

void Core::LoadPlugins() {
	printf("\nLoading plugins.\n");
	std::filesystem::path path(L"MonoPatcher/plugins/");
	std::error_code error;
	for (auto& p : std::filesystem::recursive_directory_iterator(path, error)) {
		auto extension = p.path().extension();
		auto& e = extension.native();
		if (e.length() != 4) continue;
		if (((e[1] != 'd') & (e[1] != 'D')) | ((e[2] != 'l') & (e[2] != 'L')) | ((e[3] != 'l') & (e[3] != 'L'))) continue;
		HMODULE lib = LoadLibraryW(p.path().c_str());
		if (!lib) continue;
		loadedPlugins.emplace_back(lib);
		printf("Loaded Plugin: %ls\n", p.path().c_str());
	}
}

void Core::InitializePlugins() {
	printf("\nInitializing plugins.\n");
	for (HMODULE plugin : loadedPlugins) {
		FARPROC init = GetProcAddress(plugin, "InitMonoPatcherPlugin");
		if (!init) continue;
		typedef void (*InitMonoPatcherPlugin)(PluginInitContext* context);
		PluginInitContext context{};
		reinterpret_cast<InitMonoPatcherPlugin>(init)(&context);
	}
}

bool Core::Initialize() {
	printf("Mono Patcher CPP Core initializing\n");

	if (!Addresses::Initialize())
		return false;

	// Initialize MinHook.
	if (MH_Initialize() != MH_OK)
		return false;

	if (MH_CreateHook((void*)Addresses::InitializeScriptHost, &DetourInitializeScriptHost,
		reinterpret_cast<LPVOID*>(&fpInitializeScriptHost)) != MH_OK)
	{
		return false;
	}

	if (MH_EnableHook((void*)Addresses::InitializeScriptHost) != MH_OK)
	{
		return false;
	}
	
	if (!MonoHooks::Initialize()) 
	{
		return false;
	}

	return true;
}