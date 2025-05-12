public class IDGenerator
{
    private static int _counter = -1;

    public static int GenerateID()
    {
        return _counter++;
    }
}