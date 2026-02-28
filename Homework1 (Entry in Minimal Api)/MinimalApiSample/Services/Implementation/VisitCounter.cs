using MinimalApiSample.Services.Contracts;

namespace MinimalApiSample.Services.Implementation;

public class VisitCounter : IVisitCounter
{
    public int Count { get; private set; } = 0;

    public void Increase()
    {
        Count++;
    }
}