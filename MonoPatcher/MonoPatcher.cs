using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MonoPatcherLib
{
    public static class MonoPatcher
    {
        public enum InitializationTypes
        {
            /// <summary>
            /// MonoPatcher hasn't been initialized.
            /// </summary>
            None,
            /// <summary>
            /// MonoPatcher was initialized as early as possible via a DLL Hook.
            /// </summary>
            Early,
            /// <summary>
            /// MonoPatcher was initialized late via XML.
            /// </summary>
            Late
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
                Marshal.Copy(replacementByteArray, 0, originalMethodHandle, 24);
                Marshal.Copy(replacementByteArray, 28, new IntPtr((int*)originalMethodHandle.ToPointer() + 28), 12);
            }
        }

        public static void ReplaceProperty(PropertyInfo originalProp, PropertyInfo replacementProp)
        {
            var originalGetter = originalProp.GetGetMethod();
            var originalSetter = originalProp.GetSetMethod();

            var replacementGetter = replacementProp.GetGetMethod();
            var replacementSetter = replacementProp.GetSetMethod();

            if (replacementGetter != null && originalGetter != null)
                ReplaceMethod(originalGetter, replacementGetter);

            if (replacementSetter != null && originalSetter != null)
                ReplaceMethod(originalSetter, replacementSetter);
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
        }
    }
}
