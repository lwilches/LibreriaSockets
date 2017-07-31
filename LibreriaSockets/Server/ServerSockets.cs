using LibreriaSockets.Conexiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibreriaSockets
{
    public class Server
    {
        private TcpListener _listener;
        private IPAddress _ip;
        private CancellationTokenSource _tokenSource;
        private static object objLock = new object();
       

        
        public Server()
        {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    _ip = ip;
                    break;
                }
            }

        }

        public Server(CancellationTokenSource _tokenSource)
            : this()
        {
            this._tokenSource = _tokenSource;
        }

      

        public void IniciarServidor(int puerto)
        {
            try
            {


                var token = _tokenSource.Token;

                _listener = new TcpListener(_ip, puerto);
                _listener.Start();
                while (true)
                {

                    var cliente = _listener.AcceptSocket();
                    token.ThrowIfCancellationRequested();
                    lock (objLock)
                    {
                        var conexion = new ConexionSocket(cliente, token);
                        conexion.EscucharCanalEntrada();
                        cliente.Close();
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                _listener.Stop();
            }
            catch (Exception ex)
            {
            }
        }

        public void DetenerServicio()
        {
            _tokenSource.Cancel();
            _listener.Stop();
        }

        public Socket GetPeticion()
        {

            return _listener.AcceptSocket();
        }

        public string GetIp()
        {
            return _ip.ToString();
        }

    }
}
