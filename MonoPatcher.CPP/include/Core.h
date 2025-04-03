#pragma once
#include "pch.h"
#include <vector>

class Core {
public:
	bool Initialize();
	void LoadPlugins();
	void InitializePlugins();
	static bool Create();
	static Core* GetInstance();

	struct PluginInitContext {
		// We don't supply plugins with any data during initialization--yet,
		// but we do give them a version number for the structure,
		// so that we can add additional data without breaking ABI-compatibility.
		uint8_t structureVersion = 0;
	};

	std::vector<HMODULE> loadedPlugins;
private:
	static Core* _instance;
};