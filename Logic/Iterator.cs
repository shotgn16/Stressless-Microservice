using System;

public class Iterator : IDisposable
{
    public int Incrementor;
    public int Value;

    public Task IterateUser()
    {
        if (Incrementor == 0)
        {
            Incrementor++;
            Value = 0;
        }

        else if (Incrementor > 0)
        {
            Incrementor++;
            Value++;
        }

        return Task.CompletedTask;
    }

    public void Dispose() => GC.Collect();
}
