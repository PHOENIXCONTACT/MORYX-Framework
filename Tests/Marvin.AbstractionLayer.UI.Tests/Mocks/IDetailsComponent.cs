using System;

namespace Marvin.AbstractionLayer.UI.Tests
{
    public interface IDetailsComponent : IEditModeViewModel, IDetailsViewModel
    {
        bool InitializeCalled { get; }
    }
}