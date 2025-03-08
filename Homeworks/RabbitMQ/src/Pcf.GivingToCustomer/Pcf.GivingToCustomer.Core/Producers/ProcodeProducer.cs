using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Pcf.GivingToCustomer.Core.Producers;

public class ProcodeProducer
{
    private string _exchangeType;
    private string _exchangeName;
    private string _routingKey;
    private IModel _model;

    public ProcodeProducer(string exchangeType, string exhangeName, string routingKey, IModel model)
    {
        _exchangeType = exchangeType;
        _exchangeName = exhangeName;
        _routingKey = routingKey;
        _model = model;
        _model.ExchangeDeclare(_exchangeName, _exchangeType);
    }

    public void Produce(string msgCtx)
    {
        var msg = new MsgDto()
        {
            Content = $"{msgCtx} (exchange: {_exchangeType})"
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg));
        
        _model.BasicPublish(_exchangeName, _routingKey, null, body);
        Console.WriteLine($"Promo-code sent: {_exchangeName}");
    }
}