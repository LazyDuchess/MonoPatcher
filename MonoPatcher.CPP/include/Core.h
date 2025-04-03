#pragma once
#include "pch.h"
#include <vector>

class Core {
public:
	bool Initialize();
	void LoadPlugins();
	static bool Create();
	static Core* GetInstance();

	std::vector<HMODULE> loadedPlugins;
private:
	static Core* _instance;
};