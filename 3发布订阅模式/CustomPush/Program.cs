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
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// 创建并配置处理网站用户注册的交换机和队列
var exchangeA = "exchange_A";
channel.ExchangeDeclare(exchange: exchangeA, type: ExchangeType.Fanout);
var queueA = channel.QueueDeclare().QueueName;
channel.QueueBind(queue: queueA, exchange: exchangeA, routingKey: "user_signup");

// 创建并配置处理网站订单支付的交换机和队列
var exchangeB = "exchange_B";
channel.ExchangeDeclare(exchange: exchangeB, type: ExchangeType.Fanout);
var queueB = channel.QueueDeclare().QueueName;
channel.QueueBind(queue: queueB, exchange: exchangeB, routingKey: "order_pay");

// 创建并配置处理所有类型消息的交换机和队列
var exchangeC = "exchange_C";
channel.ExchangeDeclare(exchange: exchangeC, type: ExchangeType.Fanout);
var queueC = channel.QueueDeclare().QueueName;
channel.QueueBind(queue: queueC, exchange: exchangeC, routingKey: "");

// 启动消费者 A，接收处理网站用户注册相关的消息
var consumerA = new EventingBasicConsumer(channel);
consumerA.Received += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($"接收到用户注册的消息: {message}");
};
channel.BasicConsume(queue: queueA, autoAck: true, consumer: consumerA);

// 启动消费者 B，接收处理网站订单支付相关的消息
var consumerB = new EventingBasicConsumer(channel);
consumerB.Received += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($"接收到订单支付的消息: {message}");
};
channel.BasicConsume(queue: queueB, autoAck: true, consumer: consumerB);

// 启动消费者 C，接收所有类型的消息
var consumerC = new EventingBasicConsumer(channel);
consumerC.Received += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($"接收到所有类型的消息: {message}");
};
channel.BasicConsume(queue: queueC, autoAck: true, consumer: consumerC);

// 发布消息到交换机 A，通知所有订阅者用户已注册成功
var messageA = "注册成功!";
var bodyA = Encoding.UTF8.GetBytes(messageA);
channel.BasicPublish(exchange: exchangeA, routingKey: "user_signup", basicProperties: null, body: bodyA);

// 发布消息到交换机 B，通知所有订阅者订单已支付成功
var messageB = "订单已支付!";
var bodyB = Encoding.UTF8.GetBytes(messageB);
channel.BasicPublish(exchange: exchangeB, routingKey: "order_pay", basicProperties: null, body: bodyB);

// 发布消息到交换机 C，通知所有订阅者有新的活动推出
var messageC = "新活动推出!";
var bodyC = Encoding.UTF8.GetBytes(messageC);
channel.BasicPublish(exchange: exchangeC, routingKey: "", basicProperties: null, body: bodyC);

Console.WriteLine("按 [enter] 键退出.");
Console.ReadLine();
