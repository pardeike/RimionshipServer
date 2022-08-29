using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RimionshipServer.API;
namespace RimionshipServer.Tests
{
    public class GrpcTests
    {
        [Test]
        public async Task TestSyncLock()
        {
            var tasks = new ConcurrentBag<Task>{
                                          SyncTest(true),
                                          SyncTest(true),
                                          SyncTest(true)
                                      };
            
            Parallel.For(0, 150, _ =>
                                 {
                                     tasks.Add(SyncTest(true));
                                 });

            Assert.That(() => tasks.All(x => !x.IsCompleted));
            Assert.That(() => tasks.All(x => !x.IsFaulted));
            Assert.That(() => tasks.All(x => !x.IsCanceled));
            
            var sw = new Stopwatch();
            sw.Start();
            var resetEvent = ToggleResetEvent();
            sw.Stop();
            Assert.That(() => sw.Elapsed < TimeSpan.FromSeconds(1));
            await resetEvent;
            Assert.That(() => tasks.All(x => x.IsCompleted));
        }

        private static readonly ManualResetEventSlim _mres = new ();

        public static Task ToggleResetEvent()
        {
            return Task.Factory.StartNew(() =>
                                         {
                                             _mres.Set();
                                             Thread.Sleep(1000);
                                             _mres.Reset();
                                         }, CancellationToken.None, TaskCreationOptions.None, PriorityScheduler.LowestSingleCore);
        }
        
        private static async Task<bool> SyncTest(bool WaitForChange)
        {
            await Task.Factory.StartNew(() =>
                                                     {
                                                         if (WaitForChange)
                                                             _mres.Wait();

                                                         Task.Delay(1).Wait();
                                                     },  default, TaskCreationOptions.None, PriorityScheduler.Lowest);
            return true;
        }
    }
}