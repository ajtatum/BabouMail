using System.Threading;
using System.Threading.Tasks;
using BabouMail.Common.Models;

namespace BabouMail.Common.Interfaces
{
    public interface IBabouSender
    {
        SendResponse Send(IBabouEmail email, CancellationToken? token = null);
        Task<SendResponse> SendAsync(IBabouEmail email, CancellationToken? token = null);
    }
}
