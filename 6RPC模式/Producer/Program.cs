using Producer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("[RPC 客户端] 正在请求 RPC 服务端");
        // 创建 RpcClient 对象，并使用它发起 RPC 请求。
        await InvokeAsync("30");
        Console.ReadLine();
    }

    // 定义发起 RPC 请求的方法，其中包括了创建 RpcClient 对象、发起请求并处理响应的完整流程。
    private static async Task InvokeAsync(string n)
    {
        using var rpcClient = new RpcClient();
        var response = await rpcClient.CallAsync(n);
        Console.WriteLine($"[RPC 客户端] 请求完成，回调结果：{response}");
    }
}