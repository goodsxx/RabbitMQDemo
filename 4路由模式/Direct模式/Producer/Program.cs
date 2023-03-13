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

// 定义并配置 Direct 类型的交换机
var exchangeName = "direct_exchange";
channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

// 发布消息到交换机，并根据不同的路由键选择发送到不同的队列中
var messageA = "发送到 queue A 的消息";
var bodyA = Encoding.UTF8.GetBytes(messageA);
channel.BasicPublish(exchange: exchangeName, routingKey: "key_A", basicProperties: null, body: bodyA);

var messageB = "发送到 queue B 的消息";
var bodyB = Encoding.UTF8.GetBytes(messageB);
channel.BasicPublish(exchange: exchangeName, routingKey: "key_B", basicProperties: null, body: bodyB);

var messageC = "发送到 queue C 的消息";
var bodyC = Encoding.UTF8.GetBytes(messageC);
channel.BasicPublish(exchange: exchangeName, routingKey: "key_C", basicProperties: null, body: bodyC);

Console.WriteLine("按 [enter] 键退出.");
Console.ReadLine();