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

// 创建连接对象
using var connection = factory.CreateConnection();
    // 创建信道对象
using var channel = connection.CreateModel();
// 声明队列，如果不存在就创建
channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

// 创建消费者对象
var consumer = new EventingBasicConsumer(channel);

// 消息接收事件处理
consumer.Received += (model, ea) =>
{
    byte[] body = ea.Body.ToArray();             // 获取消息体
    string message = Encoding.UTF8.GetString(body);   // 将消息体转换成字符串
    Console.WriteLine(" [消费者] 接收： {0}", message);
};

// 订阅队列并开始消费消息
channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

Console.WriteLine("按 [enter] 键退出");
Console.ReadLine();