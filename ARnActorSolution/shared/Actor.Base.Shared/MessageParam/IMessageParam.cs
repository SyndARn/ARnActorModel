namespace Actor.Base
{
    public interface IMessageParam<out T1, out T2>
    {
        T1 Item1 { get; }
        T2 Item2 { get; }
    }

    public interface IMessageParam<out T1, out T2, out T3>
    {
        T1 Item1 { get; }
        T2 Item2 { get; }
        T3 Item3 { get; }
    }

    public interface IMessageParam<out T1, out T2, out T3, out T4>
    {
        T1 Item1 { get; }
        T2 Item2 { get; }
        T3 Item3 { get; }
        T4 Item4 { get; }
    }

}