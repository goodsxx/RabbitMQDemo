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

// 创建连接对象
using var connection = factory.CreateConnection();
// 创建信道对象
using var channel = connection.CreateModel();
    
var exchangeName = "logs"; // 定义交换机名称
var message = "Hello, world!"; // 定义消息内容
var body = Encoding.UTF8.GetBytes(message); // 将消息内容转换成字节数组

// 声明一个 fanout 类型的交换机，用于广播消息
channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

// 发布消息到交换机中
channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: null, body: body);

Console.WriteLine(" [生产者] 发送： {0}", message);

Console.WriteLine(" 按 [enter] 键退出");
Console.ReadLine();

