using System.Threading;

namespace System
{
    /// <summary>
    /// 表示用于管理资源访问的锁定状态，可实现多线程读取或进行独占式写入访问。
    /// </summary>
    public class LockSlim : ObjectDisposableBase
    {
        private ReaderWriterLockSlim _Slim;
        /// <summary>
        /// 获取实际的读写锁对象。
        /// </summary>
        public ReaderWriterLockSlim Slim
        {
            get
            {
                return this._Slim;
            }
        }

        /// <summary>
        /// 初始化一个 <see cref="System.LockSlim"/> 类的新实例。
        /// </summary>
        public LockSlim()
        {
            _Slim = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// 提供锁定递归策略，初始化一个 <see cref="System.LockSlim"/> 类的新实例。
        /// </summary>
        /// <param name="recursionPolicy">锁定递归策略。</param>
        public LockSlim(LockRecursionPolicy recursionPolicy)
        {
            _Slim = new ReaderWriterLockSlim(recursionPolicy);
        }

        /// <summary>
        /// 获取一个新的读取锁。
        /// </summary>
        public IDisposable Read()
        {
            this.ThrowWhenDisposed();
            return new LockExtend(this._Slim, LockMode.Read);
        }

        /// <summary>
        /// 获取一个新的写入锁。
        /// </summary>
        public IDisposable Write()
        {
            this.ThrowWhenDisposed();
            return new LockExtend(this._Slim, LockMode.Write);
        }

        /// <summary>
        /// 获取一个新的可更新的读取锁。
        /// </summary>
        public IDisposable UpgradeableRead()
        {
            this.ThrowWhenDisposed();
            return new LockExtend(this._Slim, LockMode.UpgradeableRead);
        }

        enum LockMode
        {
            Write, Read, UpgradeableRead
        }

        class LockExtend : IDisposable
        {
            ReaderWriterLockSlim _Slim;
            LockMode _Mode;
            public LockExtend(ReaderWriterLockSlim slim, LockMode mode)
            {
                _Slim = slim;
                this._Mode = mode;
                switch(mode)
                {
                    case LockMode.Write:
                        slim.EnterWriteLock();
                        break;
                    case LockMode.Read:
                        slim.EnterReadLock();
                        break;
                    case LockMode.UpgradeableRead:
                        slim.EnterUpgradeableReadLock();
                        break;
                }
            }

            void IDisposable.Dispose()
            {
                switch(_Mode)
                {
                    case LockMode.Write:
                        _Slim.ExitWriteLock();
                        break;
                    case LockMode.Read:
                        _Slim.ExitReadLock();
                        break;
                    case LockMode.UpgradeableRead:
                        _Slim.ExitUpgradeableReadLock();
                        break;
                }
            }
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            this._Slim.Dispose();
            this._Slim = null;
        }
    }
}