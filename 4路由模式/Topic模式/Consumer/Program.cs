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
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// 定义并配置 Topic 类型的交换机
var exchangeName = "topic_exchange";
channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

// 定义并配置多个队列，分别绑定到不同的路由规则上
var queueA = "queue_A";
channel.QueueDeclare(queue: queueA, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueBind(queue: queueA, exchange: "topic_exchange", routingKey: "topic.A.#");

var queueB = "queue_B";
channel.QueueDeclare(queue: queueB, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueBind(queue: queueB, exchange: "topic_exchange", routingKey: "topic.B.*");

var queueC = "queue_C";
channel.QueueDeclare(queue: queueC, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueBind(queue: queueC, exchange: "topic_exchange", routingKey: "topic.#.C");

// 启动消费者 A，从队列 A 中接收和处理消息
var consumerA = new EventingBasicConsumer(channel);
consumerA.Received += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($"收到来自 queue A 的消息: {message}");
};
channel.BasicConsume(queue: queueA, autoAck: true, consumer: consumerA);

// 启动消费者 B，从队列 B 中接收和处理消息
var consumerB = new EventingBasicConsumer(channel);
consumerB.Received += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($"收到来自 queue B 的消息: {message}");
};
channel.BasicConsume(queue: queueB, autoAck: true, consumer: consumerB);

// 启动消费者 C，从队列 C 中接收和处理消息
var consumerC = new EventingBasicConsumer(channel);
consumerC.Received += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($"收到来自 queue C 的消息: {message}");
};
channel.BasicConsume(queue: queueC, autoAck: true, consumer: consumerC);

Console.WriteLine("按 [enter] 键退出.");
Console.ReadLine();
