#pragma once

class Core {
public:
	Core();
	static void Create();
	static Core* GetInstance();

private:
	static Core* _instance;
};