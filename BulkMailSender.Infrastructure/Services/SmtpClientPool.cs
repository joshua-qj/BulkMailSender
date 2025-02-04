using BulkMailSender.Application.Interfaces.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Collections.Concurrent;

namespace BulkMailSender.Infrastructure.Services {
    public class SmtpClientPool : ISmtpClientPool {
        private readonly ConcurrentQueue<SmtpClient> _pool;
        private readonly int _maxPoolSize;
        private readonly SemaphoreSlim _semaphore;
        private readonly string _serverName;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public SmtpClientPool(int maxPoolSize, string serverName, int port, string username, string password) {

            _pool = new ConcurrentQueue<SmtpClient>();
            _maxPoolSize = maxPoolSize;
            _semaphore = new SemaphoreSlim(maxPoolSize);
            _serverName = serverName;
            _port = port;
            _username = username;
            _password = password;

        }
        public async Task InitializePoolAsync() {
            var tasks = new List<Task>(_maxPoolSize);

            for (int i = 0; i < _maxPoolSize; i++) {
                tasks.Add(Task.Run(async () => {
                    try {
                        var client = new SmtpClient();
                        await client.ConnectAsync(_serverName, _port, SecureSocketOptions.StartTls);
                        await client.AuthenticateAsync(_username, _password);
                        Console.WriteLine($"Preloaded smtpClient HashCode {client?.GetHashCode()} at {DateTime.Now:HH:mm:ss.fff}");
                        _pool.Enqueue(client);
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Error initializing SMTP client: {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

        private readonly object _lock = new object();
        public async Task<SmtpClient> GetClientAsync() {
            await _semaphore.WaitAsync(); // Ensure only N clients are active at a time
            Console.WriteLine($"[POOL] semaphore count : {_semaphore.CurrentCount}");

            if (_pool.TryDequeue(out var smtpClient)) {
                if (!smtpClient.IsConnected) {
                    await smtpClient.ConnectAsync(_serverName, _port, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(_username, _password);
                }
                Console.WriteLine($"[POOL] Giving out client HashCode {smtpClient.GetHashCode()}");
                return smtpClient;
            }

            // If no client is available, create a new one (lazy creation)
            var newClient = new SmtpClient();
            await newClient.ConnectAsync(_serverName, _port, SecureSocketOptions.StartTls);
            await newClient.AuthenticateAsync(_username, _password);

            Console.WriteLine($"[POOL] Created new smtpClient HashCode {newClient.GetHashCode()}");
            return newClient;
        }
        public void ReturnClient(SmtpClient client) {
            if (client == null) {
                var newClient = new SmtpClient();
                newClient.ConnectAsync(_serverName, _port, SecureSocketOptions.StartTls).Wait();
                newClient.AuthenticateAsync(_username, _password).Wait();
                _pool.Enqueue(newClient);
                return;
            }
            int clientId = client.GetHashCode();


            Console.WriteLine($"[POOL] Returning client HashCode {clientId}");

            // Ensure it's still connected before putting it back in the pool
            if (client.IsConnected) {
                _pool.Enqueue(client);
            } else {

                client.ConnectAsync(_serverName, _port, SecureSocketOptions.StartTls).Wait();
                client.AuthenticateAsync(_username, _password).Wait();
                _pool.Enqueue(client);
            }

            _semaphore.Release();
        }

        public void Dispose() {
            while (_pool.TryDequeue(out var client)) {
                client.Disconnect(true);
                client.Dispose();
            }

            _semaphore.Dispose();
        }
    }
}