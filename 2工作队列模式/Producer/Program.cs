using RabbitMQ.Client;
using System.Text;

// 创建连接工厂对象，指定主机名和登录凭据信息
ConnectionFactory factory = new()
{
    HostName = "192.168.3.100",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// 声明一个队列，如果不存在则创建
var queueName = "task_queue";
channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

string[] messages = new string[]
{
    "Hello", "World", "Welcome", "to", "RabbitMQ"
};

// 向队列中发送多个消息，并设置消息持久化
foreach (var message in messages)
{
    var body = Encoding.UTF8.GetBytes(message);
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true; // 设置消息持久化

    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
    Console.WriteLine("[生产者] 发送： {0}", message);
}

Console.WriteLine("按 [enter] 键退出");
Console.ReadLine();