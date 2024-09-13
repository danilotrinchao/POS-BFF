using POS_BFF.Application.Contracts;
using POS_BFF.Application.Services;
using POS_BFF.Core.Domain.Entities;
using POS_BFF.Core.Domain.Requests;
using POS_BFF.Infra.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace POS_BFF.Presentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControllServiceController : ControllerBase
    {
        private readonly IControllTimeService _consumerService;
        private readonly ITimerCache _timerCache;
        public ControllServiceController(IControllTimeService consumer, ITimerCache timerCache)
        {
            _consumerService = consumer;
            _timerCache = timerCache;
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> GetSaleById(int userid)
        {
            try
            {
                var service = await _consumerService.GetServicesByUserId(userid);
                if (service == null)
                {
                    return NotFound();
                }
                return Ok(service);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve services: {ex.Message}");
            }
        }

        [HttpPut("stop/{id}")]
        public async Task<IActionResult> StopConsumerService(Guid id, int timeLeft)
        {
            try
            {
                var result = await _consumerService.UpdateConsumerService(id, timeLeft);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to stop sservice: {ex.Message}");
            }
        }


        [HttpPost()]
        public async Task<IActionResult> CreateConsume(ConsumerService consumer)
        {
            try
            {
                var result = await _consumerService.CreateConsumerService(consumer);
                if (result == null)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to stop sservice: {ex.Message}");
            }
        }

        [HttpGet("streamTimers")]
        public async Task<IActionResult> StreamTimers()
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            // Obter todos os cronômetros ativos na inicialização
            //var activeTimers = await _consumerService.GetActiveConsumerServicesAsync();
            var activeTimers = await _timerCache.GetTimersAsync();
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                // Atualizar o estado dos cronômetros
                var updatedTimers = new List<object>();

                foreach (var timer in activeTimers.Values)
                {
                    var elapsedTime = (int)(DateTime.UtcNow - timer.StartTime).TotalMinutes;
                    var timeRemaining = timer.totalTime - elapsedTime;

                    var status = new
                    {
                        TimerId = timer.id,
                        TimeRemaining = timeRemaining > 0 ? timeRemaining : 0
                    };

                    updatedTimers.Add(status);
                }

                var message = $"data: {JsonSerializer.Serialize(updatedTimers)}\n\n";
                var bytes = Encoding.UTF8.GetBytes(message);
                await Response.Body.WriteAsync(bytes, 0, bytes.Length);
                await Response.Body.FlushAsync();

                await Task.Delay(10000);// Atualiza a cada 10 segundos
            }

            return new EmptyResult();
        }
        [HttpGet("currentTimers")]
        public async Task<IActionResult> GetCurrentTimers()
        {
            var activeTimers = await _consumerService.GetActiveConsumerServicesAsync();

            // Calcular o tempo restante para cada cronômetro
            var timersStatus = activeTimers.Select(timer =>
            {
                var elapsedTime = (int)(DateTime.UtcNow - timer.StartTime).TotalMinutes;
                var timeRemaining = timer.totalTime - elapsedTime;
                return new
                {
                    TimerId = timer.id,
                    TimeRemaining = timeRemaining > 0 ? timeRemaining : 0
                };
            });

            return Ok(timersStatus);
        }

    }
}
