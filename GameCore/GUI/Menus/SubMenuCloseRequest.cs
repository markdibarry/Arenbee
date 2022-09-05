using System;

namespace GameCore.GUI
{
    public class SubMenuCloseRequest
    {
        public SubMenuCloseRequest() { }

        public SubMenuCloseRequest(Action callback)
            : this(callback, cascadeTo: null)
        {
        }

        public SubMenuCloseRequest(bool closeAll)
            : this(callback: null, closeAll)
        {
        }

        public SubMenuCloseRequest(Action callback, bool closeAll)
            : this(callback, null)
        {
            CloseAll = closeAll;
        }

        public SubMenuCloseRequest(Type cascadeTo)
            : this(callback: null, cascadeTo)
        {
        }

        public SubMenuCloseRequest(Action callback, Type cascadeTo)
        {
            Callback = callback;
            CascadeTo = cascadeTo;
        }

        public Action Callback { get; }
        public Type CascadeTo { get; }
        public bool CloseAll { get; }
    }
}
