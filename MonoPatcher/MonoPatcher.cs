using MonoPatcherLib.Internal;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoPatcherLib
{
    public static class MonoPatcher
    {
        public static string ReplacementLog { get; private set; } = string.Empty;
        public static int ReplacementCount { get; private set; } = 0;

        public enum InitializationTypes
        {
            /// <summary>
            /// MonoPatcher hasn't been initialized.
            /// </summary>
            None,
            /// <summary>
            /// MonoPatcher was initialized as early as possible via a DLL Hook.
            /// </summary>
            CPP,
            /// <summary>
            /// MonoPatcher was initialized late via XML.
            /// </summary>
            XML
        }

        /// <summary>
        /// Current MonoPatcher version.
        /// </summary>
        public static Version Version = new Version(Constants.Version);

        /// <summary>
        /// How was MonoPatcher initialized.
        /// </summary>
        public static InitializationTypes InitializationType = InitializationTypes.None;

        /// <summary>
        /// All plugins loaded by MonoPatcher.
        /// </summary>
        public static List<LoadedPlugin> Plugins = new List<LoadedPlugin>();

        private static void Log(string text)
        {
            ReplacementLog += $"{text}\n";
        }

        public static void Initialize(InitializationTypes initType)
        {
            InitializationType = initType;
            if (initType == InitializationTypes.None) return;
            World.sOnStartupAppEventHandler += OnStartupApp;
            LoadPlugins();
        }

        public static void PatchAll()
        {
            var method = new StackTrace().GetFrame(1).GetMethod();
            var assembly = method.ReflectedType.Assembly;
            PatchAll(assembly);
        }

        public static void PatchAll(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach(var type in types)
            {
                var typePatches = type.GetCustomAttributes(typeof(TypePatchAttribute), false);
                foreach(var typePatch in typePatches)
                {
                    (typePatch as TypePatchAttribute).Apply(type);
                }
                var methods = type.GetMethods(ReflectionUtility.DefaultBindingFlags);
                foreach(var method in methods)
                {
                    var methodPatches = method.GetCustomAttributes(typeof(ReplaceMethodAttribute), false);
                    foreach(var methodPatch in methodPatches)
                    {
                        (methodPatch as ReplaceMethodAttribute).Apply(method);
                    }
                }
                var props = type.GetProperties(ReflectionUtility.DefaultBindingFlags);
                foreach(var prop in props)
                {
                    var propPatches = prop.GetCustomAttributes(typeof(ReplacePropertyAttribute), false);
                    foreach(var propPatch in propPatches)
                    {
                        (propPatch as ReplacePropertyAttribute).Apply(prop);
                    }
                }
            }
        }
        
        /// <summary>
        /// Fully replaces a method with a new one. Make sure the parameters and return values match.
        /// </summary>
        public static void ReplaceMethod(MethodInfo originalMethod, MethodInfo replacementMethod)
        {
            unsafe
            {
                var originalMethodHandle = originalMethod.MethodHandle.Value;
                var replacementMethodHandle = replacementMethod.MethodHandle.Value;
                var replacementByteArray = new byte[40];
                Marshal.Copy(replacementMethodHandle, replacementByteArray, 0, 40);

                // Don't replace name for metadata/reflection/etc reasons.
                Marshal.Copy(replacementByteArray, 0, originalMethodHandle, 24);
                Marshal.Copy(replacementByteArray, 28, new IntPtr(originalMethodHandle.ToInt32() + 28), 12);

                Log($"Replaced {originalMethod.Name} ({originalMethodHandle.ToInt32().ToString("X")}) with {replacementMethod.Name} ({replacementMethodHandle.ToInt32().ToString("X")})");
                ReplacementCount++;

                if (Hooking.WeavedMethods.TryGetValue(originalMethodHandle, out var weavedMethod))
                {
                    weavedMethod.Dispose();
                    Hooking.WeavedMethods.Remove(originalMethodHandle);
                }
            }
        }

        /// <summary>
        /// Replaces the IL (Intermediate Language) bytecode for a method. This needs the ASI to work - if InitializationType is NOT CPP nothing will happen.
        /// </summary>
        public static void ReplaceIL(MethodInfo method, byte[] il)
        {
            if (InitializationType != InitializationTypes.CPP) return;
            var heapAlloc = Marshal.AllocHGlobal(il.Length);
            Marshal.Copy(il, 0, heapAlloc, il.Length);
            Hooking.ReplaceMethodIL(method.MethodHandle.Value, heapAlloc, il.Length);
            if (Hooking.WeavedMethods.TryGetValue(method.MethodHandle.Value, out var alloc)){
                alloc.Dispose();
            }
            Hooking.WeavedMethods[method.MethodHandle.Value] = new Hooking.WeavedMethod(heapAlloc, il.Length);
            ReplacementCount++;
            Log($"Edited the IL instructions for {method.Name} ({method.MethodHandle.Value.ToInt32().ToString("X")})");
        }

        /// <summary>
        /// Retrieves the current IL (Intermediate Language) bytecode for a method.
        /// </summary>
        public static byte[] GetIL(MethodInfo method)
        {
            if (Hooking.WeavedMethods.TryGetValue(method.MethodHandle.Value, out var alloc))
            {
                return alloc.GetBytes();
            }
            return method.GetMethodBody().GetILAsByteArray();
        }

        public static void ReplaceProperty(PropertyInfo originalProp, PropertyInfo replacementProp)
        {
            var originalAccessors = originalProp.GetAccessors(true);
            var replacementAccessors = replacementProp.GetAccessors(true);

            MethodInfo originalGetter = null;
            MethodInfo originalSetter = null;

            MethodInfo replacementGetter = null;
            MethodInfo replacementSetter = null;

            foreach(var accessor in originalAccessors)
            {
                if (accessor.ReturnType == typeof(void))
                    originalSetter = accessor;
                else
                    originalGetter = accessor;
            }

            foreach(var accessor in replacementAccessors)
            {
                if (accessor.ReturnType == typeof(void))
                    replacementSetter = accessor;
                else
                    replacementGetter = accessor;
            }

            if (originalSetter != null && replacementSetter != null)
                ReplaceMethod(originalSetter, replacementSetter);

            if (originalGetter != null && replacementGetter != null)
                ReplaceMethod(originalGetter, replacementGetter);
        }

        private static void OnStartupApp(object sender, EventArgs e)
        {
            RegisterCommands();
        }

        private static void LoadPlugins()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // First register plugins then load, in case other mods want to check for compatibility and such.
            foreach(var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach(var type in types)
                {
                    if (type.GetCustomAttributes(typeof(PluginAttribute), false).Length > 0)
                    {
                        var plugin = new LoadedPlugin(type);
                        Plugins.Add(plugin);
                    }
                }
            }

            foreach(var plugin in Plugins)
            {
                plugin.Initialize();
            }
        }

        private static void RegisterCommands()
        {
            CommandSystem.RegisterCommand("monopatcher", "Shows info about MonoPatcher", (object[] args) =>
            {
                SimpleMessageDialog.Show("MonoPatcher", $"Version: {Version}\nInitialization Type: {InitializationType}");
                return 1;
            });
            CommandSystem.RegisterCommand("monopatcher_log", "Shows MonoPatcher log", (object[] args) =>
            {
                SimpleMessageDialog.Show("MonoPatcher", $"Replaced {ReplacementCount} methods.\n{ReplacementLog}");
                return 1;
            });
            CommandSystem.RegisterCommand("monopatcher_clearcache", "Clears Reflection stuff and executes GC.", (object[] args) =>
            {
                Simulator.ClearReflectionCache();
                GC.Collect();
                return 1;
            });
        }
    }
}
