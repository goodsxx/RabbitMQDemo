using RabbitMQ.Client;
using System.Text;

// 创建ConnectionFactory实例，设置RabbitMQ节点的主机名
ConnectionFactory factory = new() 
{ 
    HostName = "192.168.3.100", 
    Port=5672, 
    UserName = "guest", 
    Password = "guest" 
};
using var connection = factory.CreateConnection(); // 创建连接
using var channel = connection.CreateModel(); // 创建通道

// 声明队列，如果该队列不存在，则会自动创建
channel.QueueDeclare(queue: "hello",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

string message = "Hello World!"; // 待发送的消息
var body = Encoding.UTF8.GetBytes(message); // 将消息转换成字节数组

// 发布消息到队列中，exchange参数为空表示默认交换器
channel.BasicPublish(exchange: "",
                     routingKey: "hello", // 消息的路由键为hello
                     basicProperties: null,
                     body: body);
Console.WriteLine("[生产者] 发送消息：{0}", message); // 输出发送的消息内容
Console.ReadLine();
