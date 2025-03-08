using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Pcf.ReceivingFromPartner.Core.Consumers;

public static class PromocodeConsumer
{
    public static void Register(IModel channel, string exchangeName, string queueName, string routingKey)
    {
        channel.BasicQos(0, 10, false);
        channel.QueueDeclare(queueName, false, false, false, null);
        channel.QueueBind(queueName, exchangeName, routingKey, null);
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, e) =>
        {
            var body = e.Body;
            var message = JsonSerializer.Deserialize<MessageDto>(Encoding.UTF8.GetString(body.ToArray()));
            Console.WriteLine($"{DateTime.Now} Recivied message: {message.Content}");
            channel.BasicAck(e.DeliveryTag, false);
        };
        channel.BasicConsume(queueName, false, consumer);
        Console.WriteLine($"Promocode key: {routingKey} (name: {exchangeName}) ");
        Console.ReadLine(); 
    }
}