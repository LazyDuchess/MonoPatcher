using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPatcher
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
        /// How was MonoPatcher initialized.
        /// </summary>
        public static InitializationTypes InitializationType = InitializationTypes.None;

        public static void Initialize(InitializationTypes initType)
        {
            InitializationType = initType;
            if (initType == InitializationTypes.None) return;
        }
    }
}
