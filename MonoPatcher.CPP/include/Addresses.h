#pragma once

namespace Addresses {
	extern void* ScriptHostGetInstance;
	extern void* ScriptHostCreateMonoClass;
	extern void* InitializeScriptHost;
	extern void* MonoAddInternalCall;
	extern void* GenerateCode;
	extern void* JIT1;
	extern void* JIT2;
	extern void* RetMoreValuesOnStack;

	bool Initialize();
}