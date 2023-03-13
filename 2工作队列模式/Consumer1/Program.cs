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
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// 声明一个队列，如果不存在则创建
var queueName = "task_queue";
channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

// 设置每个消费者最多只能处理一条消息，避免某些消费者被过度负载而导致其他消费者处于空闲状态
channel.BasicQos(0, 1, false);

Console.WriteLine(" [消费者1] 等待消息.");

// 创建一个工作者队列，每个消息都会被多个工作者中的一个处理
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [消费者1] 接收： {0}", message);

    int dots = message.Split('.').Length - 1;
    System.Threading.Thread.Sleep(dots * 1000); // 模拟任务处理时间

    Console.WriteLine(" [消费者1] 完成");

    // 手动确认消息已经处理完成
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};

// 订阅队列并开始消费消息
channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

Console.WriteLine(" 按 [enter] 键退出");
Console.ReadLine();