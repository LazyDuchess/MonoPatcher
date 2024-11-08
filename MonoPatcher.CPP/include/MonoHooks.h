#pragma once
#include <map>
#include <set>

class HookedMethod {
public:
	HookedMethod();
	HookedMethod(char* begin, int size);
	int ilSize;
	char* ilBegin;
};

namespace MonoHooks {
	extern std::map<void*, HookedMethod> HookedMethodMap;
	extern std::set<void*> JITMethodSet;
	bool Initialize();
	void InitializeScriptHost();
}