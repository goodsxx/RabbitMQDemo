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

// 发送消息到交换机
for (int i = 0; i < 10; i++)
{
    string message = $"Log 消息 {i}";
    byte[] body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchange: "logs", // 指定交换机名称
        routingKey: string.Empty, // 发送到 fanout 类型的交换机时，routingKey 不起作用，可以设置为空字符串
        basicProperties: null,
        body: body);

    Console.WriteLine(" [生产者] 发送消息：{0}", message);
}

Console.WriteLine("按[Enter]键退出");
Console.ReadLine();

