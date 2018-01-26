using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management.Tests
{
    public interface IReferenceResource : IPublicResource
    {
        IMyResource Reference { get; set; }

        IEnumerable<IMyResource> MoreReferences { get; }

        INonPublicResource NonPublic { get; }

        IMyResource GetReference();

        IReadOnlyList<IMyResource> GetReferences();

        void SetReference(IMyResource reference);

        event EventHandler<IMyResource> ReferenceChanged;

        event EventHandler<IMyResource[]> SomeChanged;
    }
}