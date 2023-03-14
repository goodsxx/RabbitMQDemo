using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

// 创建连接工厂对象，指定主机名和登录凭据信息
ConnectionFactory factory = new()
{
    HostName = "192.168.3.100",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

// 使用连接工厂对象创建一个新的 RabbitMQ 连接和通道对象。
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// 声明一个名为 "rpc_queue" 的队列，用于接收客户端发起的 RPC 请求消息。
channel.QueueDeclare(queue: "rpc_queue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

// 设置消费者对象的预取计数，即在当前消费者处理完之前，不会再从队列中取出新的消息。
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

// 创建一个事件驱动的消费者对象，用于接收来自 "rpc_queue" 队列中的 RPC 请求消息。
var consumer = new EventingBasicConsumer(channel);

// 定义消费者对象的 Received 事件处理函数，用于在收到客户端发送的 RPC 请求消息后生成响应消息并发送回客户端。
consumer.Received += (model, ea) =>
{
    string response = string.Empty;

    // 从消息体中读取请求数据、CorrelationId 和 ReplyTo 属性值。
    var body = ea.Body.ToArray();
    var props = ea.BasicProperties;
    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = props.CorrelationId;

    Console.WriteLine($"[RPC服务端] 收到 RPC 客户端请求，ID：{replyProps.CorrelationId}");

    try
    {
        // 解析请求消息中的参数，并调用 Fibonacci 函数进行计算。
        var message = Encoding.UTF8.GetString(body);
        int n = int.Parse(message);
        Console.WriteLine($"[RPC服务端] 解析 RPC 客户端消息：{message}");
        Console.WriteLine($"[RPC服务端] 调用斐波那契函数：Fib({message})");
        response = Fib(n).ToString();
    }
    catch (Exception e)
    {
        // 如果发生错误，则将异常信息作为响应消息返回给客户端。
        Console.WriteLine($"[RPC服务端] 发生异常：{e.Message}");
        response = string.Empty;
    }
    finally
    {
        // 将响应消息发送回客户端，并确认已经处理完当前的请求消息。
        var responseBytes = Encoding.UTF8.GetBytes(response);
        channel.BasicPublish(exchange: string.Empty,
                             routingKey: props.ReplyTo,
                             basicProperties: replyProps,
                             body: responseBytes);
        //消息确认
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }

    Console.WriteLine($"[RPC服务端] RPC 客户端消息处理完成");
};

channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);

Console.WriteLine("[RPC服务端] 等待 RPC 请求中。。。");
Console.ReadLine();

// 下面是一个简单的斐波那契函数实现，用于计算 RPC 请求中传递的参数值。
// 注意：该函数的实现采用了递归方式，可能会受到栈深度限制等问题的影响。
static int Fib(int n)
{
    if (n is 0 or 1)
    {
        return n;
    }

    return Fib(n - 1) + Fib(n - 2);
}
