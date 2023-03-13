using RabbitMQ.Client.Events;
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
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var exchangeName = "logs"; // 定义交换机名称

// 声明一个 fanout 类型的交换机，用于广播消息
channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

// 创建一个临时队列，并绑定到指定的交换机上
var queueName = channel.QueueDeclare().QueueName;
channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

Console.WriteLine("[消费者2] 等待队列.");

// 构造一个消费者对象，并设置 Received 事件回调函数
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($"[消费者2] Received '{message}'");
    //成功时手动确认消息
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    //失败时打回队列
    //channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
};

// 启动消费者，并指定要消费的队列
channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

Console.WriteLine("按 [enter] 键退出");
Console.ReadLine();
