namespace MinimalApiSample.Services.Contracts;

public interface IVisitCounter
{
    int Count { get; }
    void Increase();
}