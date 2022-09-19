using NUnit.Framework;
using RimionshipServer.Services;
using System.Linq;
using System.Threading.Tasks;
namespace RimionshipServer.Tests;

public class AttentionTests
{
    [Test]
    public async Task TestAttentionService()
    {
        const string user = "User1";
        await using var service = new AttentionService();
        service.ChangeDecrement(1); // 1 + 1 = 2

        service.IncreaseAttentionScore(user, 100L);
        Assert.LessOrEqual(service.GetAttentionScore(user), 100L);
        await Task.Delay(service.CooldownMs + 10);
        Assert.LessOrEqual(service.GetAttentionScore(user), 98L);
        await Task.Delay(service.CooldownMs + 10);
        Assert.AreEqual(service.GetAttentionScore(user), 96L);

        service.ChangeDecrement(18); // 1 + 1 + 18 = 20

        await Task.Delay(service.CooldownMs + 10);
        Assert.AreEqual(service.GetAttentionScore(user), 76L);
    }

    [Test]
    public async Task TestSynchronousAttentionService()
    {
        await using var service = new AttentionService();
        const string user = "User";
        for (int i = 0; i < 1024; i++)
        {
            var usr = user + i;
            service.IncreaseAttentionScore(usr, 2);
        }
        for (int i = 0; i < 1024; i++)
        {
            var usr = user + i;
            Assert.That(service.GetAttentionScore(usr) <= 2L, "Error @ " + i);
        }
    }

    [Test]
    public async Task TestConcurrentAttentionService()
    {
        await using var service = new AttentionService();
        const string user = "User";
        Parallel.For(0, 8192, i =>
                                     {
                                         var usr = user + i;
                                         service.IncreaseAttentionScore(usr, 2);
                                     });
        Parallel.For(0, 8192, i =>
                                     {
                                         var usr = user + i;
                                         Assert.That(service.GetAttentionScore(usr) <= 2L, "Error @ " + i);
                                     });
    }

    [Test]
    public async Task TestGetAllUsers()
    {
        await using var service = new AttentionService();
        const string user = "User";
        Parallel.For(0, 8192, i =>
                              {
                                  var usr = user + i;
                                  service.IncreaseAttentionScore(usr, 800);
                              });
        Assert.That(service.GetAttentionScores().Count() == 8192);
        Assert.That(service.GetAttentionScores().Select(x => x.Name).Distinct().Count() == 8192);
        foreach (long l in service.GetAttentionScores().Select(x => x.Score))
        {
            Assert.That(l > 795);
            Assert.That(l < 801);
        }
    }
}