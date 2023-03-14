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

// 声明主题交换机
channel.ExchangeDeclare(
    exchange: "topic_logs",
    type: ExchangeType.Topic);

// 发送消息到交换机
string key1 = "quick.orange.rabbit";
string key2 = "lazy.orange.elephant";
string key3 = "quick.orange.fox";
string key4 = "quick.brown.fox";
string key5 = "orange";
string key6 = "quick.orange.new.rabbit";
string key7 = "lazy.orange.new.rabbit";
byte[] body1 = Encoding.UTF8.GetBytes($"{key1} 消息");
byte[] body2 = Encoding.UTF8.GetBytes($"{key2} 消息");
byte[] body3 = Encoding.UTF8.GetBytes($"{key3} 消息");
byte[] body4 = Encoding.UTF8.GetBytes($"{key4} 消息");
byte[] body5 = Encoding.UTF8.GetBytes($"{key5} 消息");
byte[] body6 = Encoding.UTF8.GetBytes($"{key6} 消息");
byte[] body7 = Encoding.UTF8.GetBytes($"{key7} 消息");

// 将消息发送到不同的队列，根据 Routing Key 进行匹配
channel.BasicPublish(
    exchange: "topic_logs", // 指定交换机名称
    routingKey: key1, // 指定 Routing Key
    basicProperties: null,
    body: body1);
Console.WriteLine(" [生产者] 发送消息：{0}", key1);

channel.BasicPublish(
    exchange: "topic_logs", // 指定交换机名称
    routingKey: key2, // 指定 Routing Key
    basicProperties: null,
    body: body2);
Console.WriteLine(" [生产者] 发送消息：{0}", key2);

channel.BasicPublish(
    exchange: "topic_logs", // 指定交换机名称
    routingKey: key3, // 指定 Routing Key
    basicProperties: null,
    body: body3);
Console.WriteLine(" [生产者] 发送消息：{0}", key3);

channel.BasicPublish(
    exchange: "topic_logs", // 指定交换机名称
    routingKey: key4, // 指定 Routing Key
    basicProperties: null,
    body: body4);
Console.WriteLine(" [生产者] 发送消息：{0}", key4);

channel.BasicPublish(
    exchange: "topic_logs", // 指定交换机名称
    routingKey: key5, // 指定 Routing Key
    basicProperties: null,
    body: body5);
Console.WriteLine(" [生产者] 发送消息：{0}", key5);

channel.BasicPublish(
    exchange: "topic_logs", // 指定交换机名称
    routingKey: key6, // 指定 Routing Key
    basicProperties: null,
    body: body6);
Console.WriteLine(" [生产者] 发送消息：{0}", key6);

channel.BasicPublish(
    exchange: "topic_logs", // 指定交换机名称
    routingKey: key7, // 指定 Routing Key
    basicProperties: null,
    body: body7);
Console.WriteLine(" [生产者] 发送消息：{0}", key7);

Console.WriteLine("按 [enter] 键退出");
Console.ReadLine();

