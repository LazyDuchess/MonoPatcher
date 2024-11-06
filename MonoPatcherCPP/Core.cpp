#include "pch.h"
#include "Core.h"
#include "iostream"

Core* Core::_instance = nullptr;

Core* Core::GetInstance() {
	return _instance;
}

void Core::Create() {
	_instance = new Core();
}

Core::Core() {
	printf("Mono Patcher Core initializing\n");
}