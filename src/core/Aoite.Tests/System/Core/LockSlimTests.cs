using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class LockSlimTests
    {
        [Fact]
        public void LockSlimSampleTest1()
        {
            List<int> list = new List<int>();
            using(var ls = new LockSlim())
            {
                var job1 = Ajob.Once(j =>
                {
                    using(ls.Write())
                    {
                        for(int i = 0; i < 10; i++)
                        {
                            list.Add(i);
                        }
                    }
                });

                var job2 = Ajob.Once(j =>
                {
                    using(ls.Write())
                    {
                        for(int i = 10; i < 20; i++)
                        {
                            list.Add(i);
                        }
                    }
                });

                Ajob.Once(j =>
                {
                    while(true)
                    {
                        using(ls.Read())
                        {
                            if(list.Count != 20) j.Delay(100);
                            else break;
                        }
                    }
                    Assert.Equal(20, list.Count);
                    list.Sort();
                    for(int i = 0; i < 20; i++)
                    {
                        Assert.Equal(i, list[i]);
                    }
                }).Wait();
            }
        }

        [Fact]
        public void LockSlimSampleTest2()
        {
            List<int> list = new List<int>();
            Random random = new Random((int)DateTime.Now.Ticks & 100);
            using(var ls = new LockSlim())
            {
                AsyncJobHandler job = j =>
                {
                    using(ls.UpgradeableRead())
                    {
                        for(int i = 0; i < 100; i++)
                        {
                            int item;
                            while(!list.Contains(item = random.Next(0, (int)j.State)))
                            {
                                list.Add(item);
                            }
                        }
                    }
                };
                var job1 = Ajob.Once(job, state: 100);

                var job2 = Ajob.Once(job, state: 200);

                Ajob.Once(j =>
                {
                    job1.Wait();
                    job2.Wait();
                    Assert.Equal(list.Count, list.Distinct().Count());
                    Console.WriteLine(list.Count);
                }).Wait();
            }
        }
    }
}
