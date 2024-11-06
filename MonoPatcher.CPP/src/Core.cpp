#include "pch.h"
#include "Core.h"
#include "iostream"
#include "MinHook.h"

Core* Core::_instance = nullptr;

Core* Core::GetInstance() {
	return _instance;
}

bool Core::Create() {
	_instance = new Core();
	return _instance->Initialize();
}

bool Core::Initialize() {
	printf("Mono Patcher CPP Core initializing\n");
	// Initialize MinHook.
	if (MH_Initialize() != MH_OK)
		return false;
}