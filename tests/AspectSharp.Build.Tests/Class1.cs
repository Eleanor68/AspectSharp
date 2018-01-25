using X;

namespace AspectSharp.Build.Tests
{
    public class Class1
    {
        public static void M()
        {
            Class3.Z();
            Class3.Y();
            Class4.Equals(null, null);
        }
    }

    public class Class2
    {
    }

    public class Class4
    {
    }
}
