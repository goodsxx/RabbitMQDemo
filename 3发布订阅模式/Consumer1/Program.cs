using RabbitMQ.Client.Events;
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
// 创建连接和通道
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// 声明交换机，指定交换机类型为 fanout
channel.ExchangeDeclare(
    exchange: "logs",
    type: ExchangeType.Fanout);

// 声明队列，让系统随机生成队列名
// 当我们不向 QueueDeclare（） 提供任何参数时，我们会创建一个具有生成名称的非持久、独占、自动删除队列：
var queueName = channel.QueueDeclare().QueueName;

// 将队列绑定到交换机
channel.QueueBind(
    queue: queueName,  // 队列名称
    exchange: "logs",  // 交换机名称
    routingKey: "");   // 发送到 fanout 类型的交换机时，routingKey 不起作用，可以设置为空字符串

Console.WriteLine(" [消费者1] 等待消息中...");

// 创建事件消费者，用于处理接收到的消息
var consumer = new EventingBasicConsumer(channel);

// 处理接收到的消息
consumer.Received += (model, ea) =>
{
    ReadOnlyMemory<byte> body = ea.Body.ToArray();
    string message = Encoding.UTF8.GetString(body.Span);
    Console.WriteLine(" [消费者1] 收到消息：{0}", message);
};

// 订阅队列
channel.BasicConsume(
    queue: queueName,  // 队列名称
    autoAck: true,     // 是否自动发送确认消息
    consumer: consumer);

Console.WriteLine("按[Enter]键退出");
Console.ReadLine();
