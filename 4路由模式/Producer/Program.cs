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

// 声明直接交换机
channel.ExchangeDeclare(
    exchange: "direct_logs",
    type: ExchangeType.Direct);

// 发送消息到交换机
for (int i = 0; i < 10; i++)
{
    string message1 = $"orange消息 {i}";
    string message2 = $"black消息 {i}";
    string message3 = $"green消息 {i}";
    byte[] body1 = Encoding.UTF8.GetBytes(message1);
    byte[] body2 = Encoding.UTF8.GetBytes(message2);
    byte[] body3 = Encoding.UTF8.GetBytes(message3);

    // 将消息同时发送到两个队列，分别使用不同的 Routing Key
    channel.BasicPublish(
        exchange: "direct_logs", // 指定交换机名称
        routingKey: "orange", // 指定 Routing Key
        basicProperties: null,
        body: body1);

    channel.BasicPublish(
        exchange: "direct_logs", // 指定交换机名称
        routingKey: "black", // 指定 Routing Key
        basicProperties: null,
        body: body2);

    channel.BasicPublish(
        exchange: "direct_logs", // 指定交换机名称
        routingKey: "green", // 指定 Routing Key
        basicProperties: null,
        body: body3);

    Console.WriteLine(" [生产者] 发送消息：{0}，{1}，{2}", message1, message2, message3);
}

Console.WriteLine("按[Enter]键退出");
Console.ReadLine();
