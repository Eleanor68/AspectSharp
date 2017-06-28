namespace AspectSharp.Language.Tests.Skeletons
{
    public static class StaticMembers
    {
        static StaticMembers() { }

        public static int IntProperty { get; set; }

        public static int IntGetOnly { get; }

        public static int IntSetOnly { set { } }

        public static void VoidMethod() { }

        public static void VoidMethod(int p1) { }

        public static int IntMethod() { return default(int); }
    }
}
