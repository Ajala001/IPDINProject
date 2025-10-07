using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;

namespace App.Application.Services
{
    public class MembershipService(IUnitOfWork unitOfWork, IMembershipNumberCounters numberCounters) : IMembershipService
    {
        private static readonly SemaphoreSlim membershipNumberSemaphore = new SemaphoreSlim(1, 1);
        public async Task<string> GenerateMembershipNumberAsync(string role)
        {
            await membershipNumberSemaphore.WaitAsync();
            try
            {
                string year = DateTime.Now.Year.ToString();
                var counter = await numberCounters.GetMembershipCounterAsync(c => c.Role == role && c.Year == year);
                if (counter == null)
                {
                    counter = new MembershipNumberCounter
                    {
                        Role = role,
                        Year = year,
                        LastNumber = 0
                    };
                    await numberCounters.AddCounterAsync(counter);
                }

                counter.LastNumber++;
                await unitOfWork.SaveAsync();

                string prefix = role == "Member" ? "MEM" : "ADM";
                string uniqueId = counter.LastNumber.ToString("D4");

                return $"{prefix}/{year}/{uniqueId}";
            }
            finally
            {
                membershipNumberSemaphore.Release();
            }
        }

    }
}
