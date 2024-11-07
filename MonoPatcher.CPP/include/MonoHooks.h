#pragma once
#include <map>

class HookedMethod {
public:
	HookedMethod();
	HookedMethod(char* begin, int size);
	int ilSize;
	char* ilBegin;
};

namespace MonoHooks {
	extern std::map<void*, HookedMethod> HookedMethodMap;
	bool Initialize();
	void InitializeScriptHost();
}