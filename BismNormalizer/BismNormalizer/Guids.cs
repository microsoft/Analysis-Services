// Guids.cs
// MUST match guids.h
using System;

namespace BismNormalizer
{
    static class GuidList
    {
        public const string guidBismNormalizerPkgString = "d094957e-b4bc-493f-b473-a0da301b21a1";
        public const string guidBismNormalizerCmdSetString = "1bbd436a-e530-49da-aea2-0a69108a1c57";
        public const string guidBismNormalizerEditorFactoryString = "53f713ab-4e20-4874-831b-168ad70597b0";

        public static readonly Guid guidBismNormalizerCmdSet = new Guid(guidBismNormalizerCmdSetString);
        public static readonly Guid guidBismNormalizerEditorFactory = new Guid(guidBismNormalizerEditorFactoryString);
    };
}