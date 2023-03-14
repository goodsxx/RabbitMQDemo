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
using var connection = factory.CreateConnection(); // 创建连接
using var channel = connection.CreateModel(); // 创建通道

// 声明队列，如果该队列不存在，则会自动创建
channel.QueueDeclare(queue: "hello",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

// 创建一个事件基本消费者
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    ReadOnlyMemory<byte> body = ea.Body.ToArray(); // 获取消息体的字节数组
    string message = Encoding.UTF8.GetString(body.Span); // 将字节数组转换成字符串
    Console.WriteLine("[消费者] 收到消息：{0}", message); // 输出接收到的消息
    
    //成功时手动确认消息
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    //失败时打回队列
    //channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
};

// 启动消费者
channel.BasicConsume(queue: "hello",
                     autoAck: false, // 是否自动确认消息已经被消费
                     consumer: consumer); // 指定消费者

Console.WriteLine("按任意键退出");
Console.ReadLine(); 