using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App.Infrastructure
{
	public abstract class TimerService : BackgroundService
	{
		protected int delay = 10000;

		public TimerService()
		{
		}

		protected abstract void StartExecute();

		protected abstract void StopExecute(int executedResult = 0);

		protected abstract void BeginExecute();

		protected abstract void ExecuteError(Exception e);

		protected abstract void AfterExecute();

		protected abstract bool CanExecuteAsync();

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			if (CanExecuteAsync())
			{
				StartExecute();
				try
				{
					while (!stoppingToken.IsCancellationRequested)
					{
						try
						{
							BeginExecute();
							TaskServices();
							AfterExecute();
						}
						catch (Exception e)
						{
							ExecuteError(e);
						}
						await Task.Delay(delay, stoppingToken);
					}
				}
				catch (Exception)
				{
					if (!stoppingToken.IsCancellationRequested)
					{
						StopExecute(-1);
					}
				}
				finally
				{
					StopExecute();
				}
			}
		}

		protected abstract void TaskServices();
	}
}
