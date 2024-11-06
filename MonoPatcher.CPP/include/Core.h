#pragma once

class Core {
public:
	bool Initialize();
	static bool Create();
	static Core* GetInstance();
private:
	static Core* _instance;
};