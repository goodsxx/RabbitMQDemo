using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;

namespace Producer;

public class RpcClient : IDisposable
{
    private const string QUEUE_NAME = "rpc_queue";

    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper = new();

    public RpcClient()
    {
        // 创建连接工厂对象，指定主机名和登录凭据信息
        ConnectionFactory factory = new()
        {
            HostName = "192.168.3.100",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        // 创建连接对象和通道对象
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        // 创建一个自动生成名称的队列，用于接收来自服务器端的响应消息
        replyQueueName = channel.QueueDeclare().QueueName;

        // 创建并配置一个消费者对象，用于处理从 replyQueueName 队列中接收到的响应消息
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            // 从回调字典中查找与该消息相关联的任务源对象，并将响应消息传递给它。
            if (!callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                return;
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            tcs.TrySetResult(response);
        };
        channel.BasicConsume(consumer: consumer,
                             queue: replyQueueName,
                             autoAck: true);

        Console.WriteLine("[RPC 客户端] 已注册会掉队列");
    }

    // 发起 RPC 请求的方法，返回一个任务源对象，可用于异步等待服务器端的响应消息。
    public Task<string> CallAsync(string message, CancellationToken cancellationToken = default)
    {
        IBasicProperties props = channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = replyQueueName;
        var messageBytes = Encoding.UTF8.GetBytes(message);

        // 创建一个新的任务源对象，用于异步等待服务器端的响应消息。
        var tcs = new TaskCompletionSource<string>();
        callbackMapper.TryAdd(correlationId, tcs);

        // 将请求消息发送到指定的队列上，并在请求消息的属性中添加用于标识该请求的 correlationId 和对应的回调队列名 replyQueueName。
        channel.BasicPublish(exchange: string.Empty,
                             routingKey: QUEUE_NAME,
                             basicProperties: props,
                             body: messageBytes);

        Console.WriteLine($"[RPC 客户端] 已向 RPC 服务端发出请求，请求ID：{correlationId}.");

        // 如果在指定的时间内仍未收到服务器端的响应消息，则取消此次请求。
        cancellationToken.Register(() =>
        {
            callbackMapper.TryRemove(correlationId, out _);
        });
        return tcs.Task;
    }

    // 实现 IDisposable 接口，用于在使用完毕后释放资源。
    public void Dispose()
    {
        connection.Close();
    }
}
