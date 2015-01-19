using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    partial class GA
    {
        /// <summary>
        /// 提供用于 System.IO 命名空间下的实用工具方法。
        /// </summary>
        public static class IO
        {
            /// <summary>
            /// 尝试创建本地不存在的目录。如果目录已经存在，将被不会做任何事情。
            /// </summary>
            /// <param name="path">目录路径。</param>
            /// <returns>返回创建后的地址。</returns>
            public static DirectoryInfo CreateDirectory(string path)
            {
                lock(string.Intern(path))
                {
                    var info = new DirectoryInfo(path);
                    if(!info.Exists) info.Create();
                    return info;
                }
            }

            /// <summary>
            /// 复制指定目录的所有数据到新的目录。
            /// </summary>
            /// <param name="sourceDirName">源目录路径。</param>
            /// <param name="destDirName">目标路径。</param>
            /// <param name="deleteSource">指示删除源目录。</param>
            public static void CopyDirectory(string sourceDirName, string destDirName, bool deleteSource)
            {
                CopyDirectory(sourceDirName, destDirName);
                if(deleteSource)
                {
                    Directory.Delete(sourceDirName, true);
                }
            }

            /// <summary>
            /// 复制指定目录的所有数据到新的目录。
            /// </summary>
            /// <param name="sourceDirName">源目录路径。</param>
            /// <param name="destDirName">目标路径。</param>
            public static void CopyDirectory(string sourceDirName, string destDirName)
            {
                if(!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                FileSystemInfo[] sfiles = dir.GetFileSystemInfos();
                if(sfiles != null && sfiles.Length > 0)
                {
                    for(int i = 0; i < sfiles.Length; i++)
                    {
                        if(sfiles[i].Attributes == FileAttributes.Directory)
                        {
                            CopyDirectory(sfiles[i].FullName, Path.Combine(destDirName, sfiles[i].Name));
                        }
                        else
                        {
                            FileInfo file = (FileInfo)sfiles[i];
                            file.CopyTo(Path.Combine(destDirName, file.Name), true);
                        }
                    }
                }
            }

            /// <summary>
            /// 删除本地已存在的目录。
            /// </summary>
            /// <param name="path">目录路径。</param>
            /// <param name="recursive">若要移除 <paramref name="path"/> 中的目录、子目录和文件，则为 true；否则为 false。</param>
            public static IAsyncJob DeleteDirectory(string path, bool recursive = true)
            {
                if(!Directory.Exists(path)) return null;
                var job = Ajob.Once(j =>
                {
                    while(true)
                    {
                        try
                        {
                            Directory.Delete(path, recursive);
                            break;
                        }
                        catch(Exception)
                        {
                            j.Delay(300);
                        }
                    }
                });
                job.Wait(300);
                return job;
            }

            /// <summary>
            /// 指定编码格式，以共享方式读取文件的所有行。
            /// </summary>
            /// <param name="filename">文件的路径。</param>
            /// <param name="encoding">编码格式。</param>
            /// <returns>返回文件的所有行。</returns>
            public static string[] ShareReadAllLines(string filename, Encoding encoding = null)
            {
                List<string> list = new List<string>();
                using(FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    StreamReader reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
                    string str;
                    while(!reader.EndOfStream)
                    {
                        str = reader.ReadLine();
                        list.Add(str);
                    }
                }
                return list.ToArray();
            }

            /// <summary>
            /// 指定编码格式，以共享方式读取文件。
            /// </summary>
            /// <param name="filename">文件的路径。</param>
            /// <param name="encoding">编码格式。</param>
            /// <returns>返回文件的所有文本内容。</returns>
            public static string ShareReadAllText(string filename, Encoding encoding = null)
            {
                using(FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
                {
                    return new StreamReader(stream, encoding ?? Encoding.UTF8).ReadToEnd();
                }
            }

            /// <summary>
            ///  检查指定的文件名是否包含不允许在文件名中使用的字符。
            /// </summary>
            /// <param name="filename">文件名。</param>
            /// <returns>如果这是一个合法的路径，将会返回 true，否则返回 false。</returns>
            public static bool IsValidName(string filename)
            {
                if(string.IsNullOrEmpty(filename)) return false;
                foreach(var c in Path.GetInvalidFileNameChars()) if(filename.Contains(c)) return false;
                return true;
            }
        }
        private static readonly Task preCompletedTask = GetCompletedTask();
        private static readonly Task preCanceledTask = GetPreCanceledTask();
        private static Task GetPreCanceledTask()
        {
            var source = new TaskCompletionSource<object>();
            source.TrySetCanceled();
            return source.Task;
        }

        private static Task GetCompletedTask()
        {
            var source = new TaskCompletionSource<object>();
            source.TrySetResult(null);
            return source.Task;
        }
        /// <summary>
        /// 创建将在时间延迟后完成的任务。
        /// </summary>
        /// <param name="delay">完成返回任务的等待时间跨度。</param>
        /// <returns>表示时间延迟的任务。</returns>
        public static Task Delay(TimeSpan delay)
        {
            return Delay((int)delay.TotalMilliseconds);
        }
        /// <summary>
        /// 创建将在时间延迟后完成的任务。
        /// </summary>
        /// <param name="delay">完成返回任务的等待时间跨度。</param>
        /// <param name="cancellationToken">将在完成“已返回”任务之前选中的取消标记。</param>
        /// <returns>表示时间延迟的任务。</returns>
        public static Task Delay(TimeSpan delay, CancellationToken cancellationToken)
        {
            return Delay((int)delay.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// 创建将在时间延迟后完成的任务。
        /// </summary>
        /// <param name="delay">在完成返回任务之前要等待的毫秒数。</param>
        /// <returns>表示时间延迟的任务。</returns>
        public static Task Delay(int delay)
        {
            return Delay(delay, CancellationToken.None);
        }
        /// <summary>
        /// 创建将在时间延迟后完成的任务。
        /// </summary>
        /// <param name="delay">在完成返回任务之前要等待的毫秒数。</param>
        /// <param name="cancellationToken">将在完成“已返回”任务之前选中的取消标记。</param>
        /// <returns>表示时间延迟的任务。</returns>
        public static Task Delay(int delay, CancellationToken cancellationToken)
        {
            if(delay < -1)
            {
                throw new ArgumentOutOfRangeException("dueTimeMs", "Invalid due time");
            }
            else if(cancellationToken.IsCancellationRequested)
            {
                return preCanceledTask;
            }
            else if(delay == 0)
            {
                return preCompletedTask;
            }

            var tcs = new TaskCompletionSource<object>();
            var ctr = new CancellationTokenRegistration();
            var timer = new Timer(self =>
            {
                ctr.Dispose();
                ((Timer)self).Dispose();
                tcs.TrySetResult(null);
            });

            if(cancellationToken.CanBeCanceled)
            {
                ctr = cancellationToken.Register(() =>
                {
                    timer.Dispose();
                    tcs.TrySetCanceled();
                });
            }

            timer.Change(delay, -1);
            return tcs.Task;
        }
    }
}
