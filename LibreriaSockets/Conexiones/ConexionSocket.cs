using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibreriaSockets.Conexiones
{
    public class ConexionSocket
    {
        private Socket _cliente;
        private Task _tareaEntrada;
        private CancellationToken _cancellationtoken;
        private readonly List<string> _mensajesEntrada;
        private readonly string _infoCliente;


        private readonly Guid _identificador;


        public string InfoCliente { get { return _infoCliente; } }
        public Guid Id { get { return _identificador; } }

        public Socket Cliente
        {
            get { return _cliente; }
            set { _cliente = value; }
        }

        public Task TareaCanalEntrada { get { return _tareaEntrada; } }

        public ConexionSocket(Socket socket, CancellationToken token)
        {
            _mensajesEntrada = new List<string>();
            _cancellationtoken = token;
            _cliente = socket;
            _infoCliente = socket.RemoteEndPoint.ToString();
            _identificador = new Guid();
        }

        public IEnumerable<string> GetMensajes()
        {
            foreach (var mensaje in _mensajesEntrada)
            {
                yield return mensaje;
            }
        }

       

        public void EscucharCanalEntrada()
        {
           
            try
            {

                byte[] serverBuffer = new byte[10025];
                int bytes = 0;
                serverBuffer = new byte[10025];
                bytes = 0;
                bytes = _cliente.Receive(serverBuffer, serverBuffer.Length, 0);
                _cancellationtoken.ThrowIfCancellationRequested();
                var texto = Encoding.ASCII.GetString(serverBuffer, 0, bytes);
                ///si recibio algo por le socket
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    ///TODO: HACER ALGO...............
                    ///
                    ///
                    var file = new FileInfo("mensajes.txt");
                    using (var stream = file.Open(FileMode.Append, FileAccess.Write))
                    {
                        using (var wr = new System.IO.StreamWriter(stream))
                        {
                            wr.WriteLine($"[{DateTime.Now}] -  Mensaje {texto} from { _infoCliente}");
                        }
                    }
                }
                Cliente.Close();
            }
            catch (Exception ex)
            {
                DesconectarCliente();
            }

        }

        public void DesconectarCliente()
        {
            try
            {
             
                _cliente.Close();
                _cliente.Dispose();
                _mensajesEntrada.Clear();
            }
            catch (Exception) { }
        }

        public void EnviarMensaje(string mensaje)
        {
            if (_cliente.Connected)
            {
                var serverBuffer = Encoding.ASCII.GetBytes(mensaje);
                _cliente.Send(serverBuffer);

            }
        }

    }

}
