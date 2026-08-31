using System.Collections.Generic;

namespace PahlUnity
{
    public interface ISpecContainer
    {
        IReadOnlyList<SpecFieldRaw> Specs { get; }
    }
}
