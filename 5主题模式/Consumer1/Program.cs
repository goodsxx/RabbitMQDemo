using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

// 创建连接工厂对象，指定主机名和登录凭据信息
ConnectionFactory factory = new()
{
    HostName = "192.168.3.100",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

// 创建连接和通道
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// 声明主题交换机
channel.ExchangeDeclare(
    exchange: "topic_logs",
    type: ExchangeType.Topic);

// 声明队列，让系统随机生成队列名
var queueName = channel.QueueDeclare().QueueName;

// 绑定队列到交换机，为每个队列分别指定 Binding Key
channel.QueueBind(queue: queueName,
                  exchange: "topic_logs",
                  routingKey: "*.orange.*");

Console.WriteLine(" [消费者1] 等待 [*.orange.*] 消息中.");

// 创建事件消费者，用于处理接收到的消息
var consumer = new EventingBasicConsumer(channel);

// 处理接收到的消息
consumer.Received += (model, ea) =>
{
    ReadOnlyMemory<byte> body = ea.Body.ToArray();
    string message = Encoding.UTF8.GetString(body.Span);
    Console.WriteLine(" [消费者1] 收到消息: {0}", message);
};

// 启动消费者，开始监听队列
channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine("按 [enter] 键退出");
Console.ReadLine();