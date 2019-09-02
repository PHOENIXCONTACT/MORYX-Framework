namespace Marvin.AbstractionLayer.UI.Tests
{
    [DetailsComponentRegistration(TypeName)]
    public class DetailsComponentMock : EditModeViewModelBase, IDetailsComponent
    {
        public const string TypeName = "MockiMock";

        public bool InitializeCalled { get; private set; }

        public void Initialize(string typeName)
        {
            InitializeCalled = true;
        }

        public void SetBusy(bool to)
        {
            IsBusy = to;
        }
    }

    [DetailsComponentRegistration(DetailsConstants.DefaultType)]
    public class DefaultDetailsMock : DetailsComponentMock
    {

    }
}