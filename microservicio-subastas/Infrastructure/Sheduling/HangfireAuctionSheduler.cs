using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Sagas;
using Application.Contracts.Services;
using Hangfire;

namespace Infrastructure.Sheduling;

public class HangfireAuctionSheduler : IAuctionSheduler
{
    /// <summary>
    /// Programa la ejecución diferida del cierre de una subasta mediante un job en segundo plano.
    /// Utiliza el scheduler de Hangfire para invocar el proceso de cierre asociado al identificador de subasta en la fecha especificada.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) que debe cerrarse automáticamente.
    /// </param>
    /// <param name="endDate">
    /// Fecha y hora (<see cref="DateTime"/>) en la que debe iniciarse la saga de cierre de la subasta.
    /// </param>
    public void ScheduleAuctionClose(Guid auctionId, DateTime endDate)
    {
        BackgroundJob.Schedule<ICloseAuctionSagaStarter>(
            x => x.StartSagaAsync(auctionId), endDate);
    }
}
