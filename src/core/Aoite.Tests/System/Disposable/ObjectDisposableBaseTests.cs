using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Disposable
{
    public class ObjectDisposableBaseTests
    {
        class ObjectDisposable : ObjectDisposableBase
        {
            public static bool IsDisposeManaged;
            public static bool IsDisposeUnmanaged;

            protected override void DisposeManaged()
            {
                IsDisposeManaged = true;
                base.DisposeManaged();
            }

            protected override void DisposeUnmanaged()
            {
                IsDisposeUnmanaged = true;
                base.DisposeUnmanaged();
            }
        }
        public ObjectDisposableBaseTests()
        {
            ObjectDisposable.IsDisposeManaged = false;
            ObjectDisposable.IsDisposeUnmanaged = false;
        }

        [Fact()]
        public void DisposeTest()
        {
            ObjectDisposable o = new ObjectDisposable();
            Assert.False(o.IsDisposed);
            o.Dispose();
            Assert.True(o.IsDisposed);
            Assert.True(ObjectDisposable.IsDisposeManaged);
            Assert.True(ObjectDisposable.IsDisposeUnmanaged);
        }

        void CreateObjectDisposable()
        {
            ObjectDisposable o = new ObjectDisposable();
        }
        [Fact()]
        public void DestructorTest()
        {
            CreateObjectDisposable();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.False(ObjectDisposable.IsDisposeManaged);
            Assert.True(ObjectDisposable.IsDisposeUnmanaged);
        }


    }
}
