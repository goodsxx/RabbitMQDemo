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
channel.QueueDeclare(queue: "work_queue",
                     durable: true, // 设置队列为持久化
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

Random random = new();

var totalTimes = 0;//总耗时
var num = 0;//处理消息数

// 创建一个事件基本消费者
var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (model, ea) =>
{
    ReadOnlyMemory<byte> body = ea.Body.ToArray();
    string message = Encoding.UTF8.GetString(body.Span);
    Console.WriteLine($"[消费者2] 收到消息：{message}");

    // 模拟耗时的任务处理
    var time = random.Next(0, 5000);
    await Task.Delay(time);

    Console.WriteLine($"[消费者2] 完成: 耗时{time}ms");

    // 当消费者完成任务后，手动确认消息已经被消费
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

    totalTimes += time;
    num += 1;

    Console.WriteLine($"[消费者2] 目前处理{num}条消息，共耗时{totalTimes}ms");
};
// 启动消费者
channel.BasicConsume(queue: "work_queue",
                     autoAck: false, // 关闭自动确认消息消费
                     consumer: consumer); // 指定消费者
Console.WriteLine("按[Enter]键退出");
Console.ReadLine();