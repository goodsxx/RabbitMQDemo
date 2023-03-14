using RabbitMQ.Client;
using System.Text;

// 创建ConnectionFactory实例，设置RabbitMQ节点的主机名
ConnectionFactory factory = new()
{
    HostName = "192.168.3.100",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};
using var connection = factory.CreateConnection(); // 创建连接
using var channel = connection.CreateModel(); // 创建通道

// 声明队列，如果该队列不存在，则会自动创建
channel.QueueDeclare(queue: "work_queue",
                     durable: true, //设置队列为持久化
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
// 设置消息为持久化
var properties = channel.CreateBasicProperties();
properties.Persistent = true;

for (int i = 0; i < 10; i++)
{
    string message = $"任务消息 {i}"; // 待发送的消息
    var body = Encoding.UTF8.GetBytes(message); // 将消息转换成字节数组

    // 发布消息到队列中，exchange参数为空表示默认交换器
    channel.BasicPublish(exchange: "",
                         routingKey: "work_queue", // 消息的路由键为work_queue
                         basicProperties: properties,
                         body: body);
    Console.WriteLine("[生产者] 发送消息：{0}", message); // 输出发送的消息内容
}
Console.WriteLine("按[Enter]键退出");
Console.ReadLine(); // 阻塞等待用户按回车键
