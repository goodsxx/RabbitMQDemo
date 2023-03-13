using RabbitMQ.Client;
using System.Text;

// 创建连接工厂对象，指定主机名和登录凭据信息
ConnectionFactory factory = new() 
{ 
    HostName = "192.168.3.100", 
    Port=5672, 
    UserName = "guest", 
    Password = "guest" 
};

// 创建连接对象
using var connection = factory.CreateConnection();
// 创建信道对象
using var channel = connection.CreateModel();
// 声明队列，如果不存在就创建
channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

// 发送消息到队列中
string message = "Hello World!";
var body = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
Console.WriteLine(" [生产者] 发送： {0}", message);

Console.WriteLine(" 按 [enter] 键退出");
Console.ReadLine();
